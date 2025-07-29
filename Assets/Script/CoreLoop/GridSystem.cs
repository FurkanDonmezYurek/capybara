using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Seat Settings")]
    public GameObject seatPrefab;
    public int rows = 7;
    public int columns = 6;
    public float spacing = 1.2f;

    [Header("Grouping Settings")]
    public int groupWidth = 3;
    public int groupHeight = 1;
    public float groupSpacingX = 1.0f;
    public Transform groupsParent;

    [ContextMenu("Generate Grid and Groups")] //for run in the editor
    public void GenerateGrid()
    {
        var seats = new Seat[columns, rows];

        int groupCountX = columns / groupWidth;
        int groupCountY = rows / groupHeight;

        for (int groupY = 0; groupY < groupCountY; groupY++)
        {
            for (int groupX = 0; groupX < groupCountX; groupX++)
            {
                List<Seat> groupSeats = new();
                GameObject gObj = new GameObject($"SeatGroup_{groupY}_{groupX}");
                gObj.transform.SetParent(groupsParent);

                for (int dy = 0; dy < groupHeight; dy++)
                {
                    for (int dx = 0; dx < groupWidth; dx++)
                    {
                        int x = groupX * groupWidth + dx;
                        int y = groupY * groupHeight + dy;

                        Vector3 groupOffset = new Vector3(groupX * groupSpacingX, 0, 0);
                        Vector3 pos = new Vector3(x * spacing, 0, -y * spacing) + groupOffset;

                        GameObject obj =
                            PrefabUtility.InstantiatePrefab(seatPrefab, transform) as GameObject; //for run in the editor
                        obj.transform.localPosition = pos;

                        Seat seat = obj.GetComponent<Seat>();
                        seat.gridPosition = new Vector2Int(x, y);
                        seats[x, y] = seat;

                        seat.transform.SetParent(gObj.transform);
                        groupSeats.Add(seat);
                    }
                }

                SeatGroup group = gObj.AddComponent<SeatGroup>();
                group.seatsInGroup = groupSeats;
                for (int i = 0; i < group.seatsInGroup.Count; i++)
                {
                    group.seatsInGroup[i].groupOfSeat = group;
                }
                group.groupY = groupY;
                group.groupX = groupX;
            }
        }
    }

    // Adds seats in order to the current grid.
    [ContextMenu("Add Seat Group")]
    public void AddSeatGroup()
    {
        if (seatPrefab == null || groupsParent == null)
        {
            Debug.LogError("Seat prefab or groupsParent is not set!");
            return;
        }

        int groupCountX = columns / groupWidth;
        int currentGroupIndex = groupsParent.childCount;
        int groupX = currentGroupIndex % groupCountX;
        int groupY = currentGroupIndex / groupCountX;

        // Eğer satır sayısı yetmiyorsa, büyüt
        int requiredRows = (groupY + 1) * groupHeight;
        if (requiredRows > rows)
            rows = requiredRows;

        List<Seat> groupSeats = new();
        GameObject gObj = new GameObject($"SeatGroup_{groupY}_{groupX}");
        gObj.transform.SetParent(groupsParent);

        for (int dy = 0; dy < groupHeight; dy++)
        {
            for (int dx = 0; dx < groupWidth; dx++)
            {
                int x = groupX * groupWidth + dx;
                int y = groupY * groupHeight + dy;

                // Eğer sütun yetmiyorsa büyüt
                if (x >= columns)
                    columns = x + 1;

                Vector3 groupOffset = new Vector3(groupX * groupSpacingX, 0, 0);
                Vector3 pos = new Vector3(x * spacing, 0, -y * spacing) + groupOffset;

#if UNITY_EDITOR
                GameObject obj = PrefabUtility.InstantiatePrefab(seatPrefab, transform) as GameObject;
#else
            GameObject obj = Instantiate(seatPrefab, transform);
#endif
                obj.transform.localPosition = pos;

                Seat seat = obj.GetComponent<Seat>();
                seat.gridPosition = new Vector2Int(x, y);
                seat.transform.SetParent(gObj.transform);
                groupSeats.Add(seat);
            }
        }

        SeatGroup group = gObj.AddComponent<SeatGroup>();
        group.seatsInGroup = groupSeats;
        group.groupX = groupX;
        group.groupY = groupY;

        foreach (var seat in groupSeats)
            seat.groupOfSeat = group;

        Debug.Log($"SeatGroup_{groupY}_{groupX} added. New grid size: {columns}x{rows}");

        // Update SeatGroup cache 
        if (Application.isPlaying)
        {
            GameManager.Instance.ClearSeatGroupCache();
            GameManager.Instance.InitializeSeatGroupsCache();
        }

    }


    public void ClearGrid()
    {
        if (groupsParent == null) return;

        for (int i = groupsParent.childCount - 1; i >= 0; i--)
        {
            Transform child = groupsParent.GetChild(i);
            if (child != null)
                DestroyImmediate(child.gameObject);
        }
    }


    public Seat GetSeatAtPosition(Vector2Int gridPos)
    {
        foreach (var group in FindObjectsOfType<SeatGroup>())
        {
            foreach (var seat in group.seatsInGroup)
            {
                if (seat.gridPosition == gridPos)
                    return seat;
            }
        }
        return null;
    }

    public void SetGridParameters(int rows, int columns, int groupWidth, int groupHeight)
    {
        this.rows = rows;
        this.columns = columns;
        this.groupWidth = groupWidth;
        this.groupHeight = groupHeight;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddSeatGroup();
        }
    }

}
