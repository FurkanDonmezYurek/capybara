using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Seat Settings")]
    public GameObject seatPrefab;
    public int rows = 7;
    public int columns = 6;
    public float horizontalSpacing = 1.2f;
    public float verticalSpacing = 1.2f;

    [Header("Grouping Settings")]
    public int groupWidth = 3;
    public int groupHeight = 1;
    public float groupSpacingX = 1.0f;
    public Transform groupsParent;

    //for path
    public Dictionary<string, Dictionary<string, Vector3>> pathPointsGrid;
    public Vector3[,] groupPositions;

    // ...

    [ContextMenu("Generate Grid and Groups")] //for run in the editor
    public void GenerateGrid()
    {
        // Align the grid system with the "GridSpawn" tagged object before spawning seats
        GameObject gridSpawn = GameObject.FindGameObjectWithTag("GridSpawn");
        if (gridSpawn != null)
        {
            transform.position = gridSpawn.transform.position;
        }
        else
        {
            Debug.LogWarning("No object with tag 'GridSpawn' found in the scene.");
        }

        var seats = new Seat[columns, rows];

        int groupCountX = columns / groupWidth;
        int groupCountY = rows / groupHeight;

        groupPositions = new Vector3[groupCountY, groupCountX];

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
                        Vector3 pos = new Vector3(x * horizontalSpacing, 0, -y * verticalSpacing) + groupOffset;


#if UNITY_EDITOR
                        GameObject obj =
                            PrefabUtility.InstantiatePrefab(seatPrefab, transform) as GameObject; //for run in the editor
#else
                        GameObject obj = Instantiate(seatPrefab, transform);
#endif
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
                groupPositions[groupY, groupX] = GetSeatGroupCenter(group);
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
                Vector3 pos = new Vector3(x * horizontalSpacing, 0, -y * verticalSpacing) + groupOffset;

#if UNITY_EDITOR
                GameObject obj =
                    PrefabUtility.InstantiatePrefab(seatPrefab, transform) as GameObject;
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
        if (groupsParent == null)
            return;

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

    public void SetGridParameters(int rows, int columns, int groupWidth, int groupHeight, float horizontalSpacing = 1.2f, float verticalSpacing = 1.2f)
    {
        this.rows = rows;
        this.columns = columns;
        this.groupWidth = groupWidth;
        this.groupHeight = groupHeight;
        this.horizontalSpacing = horizontalSpacing;
        this.verticalSpacing = verticalSpacing;
    }

    public void InitPathGrid()
    {
        int pointRow = rows / groupHeight;
        int pointColumn = (columns / groupWidth) - 1;
        pathPointsGrid = new Dictionary<string, Dictionary<string, Vector3>>();

        for (int x = 0; x < pointColumn; x++)
        {
            string xKey = x.ToString();
            if (!pathPointsGrid.ContainsKey(xKey))
                pathPointsGrid[xKey] = new Dictionary<string, Vector3>();

            for (int y = 0; y < pointRow; y++)
            {
                string yKey = y.ToString();

                float xPos = (groupPositions[y, x].x + groupPositions[y, x + 1].x) / 2;
                float yPos = groupPositions[y, x].y;
                float zPos = groupPositions[y, x].z;

                Vector3 pointPosition = new Vector3(xPos, yPos, zPos);
                Debug.Log(pointPosition);
                pathPointsGrid[xKey][yKey] = pointPosition;
            }
        }

        Debug.Log("Path grid initialized.");
    }

    Vector3 GetSeatGroupCenter(SeatGroup group)
    {
        if (group == null || group.seatsInGroup == null || group.seatsInGroup.Count == 0)
            return Vector3.zero;

        Vector3 total = Vector3.zero;
        foreach (var seat in group.seatsInGroup)
        {
            total += seat.transform.position;
        }

        return total / group.seatsInGroup.Count;
    }

    private void OnDrawGizmos()
    {
        if (pathPointsGrid == null || pathPointsGrid.Count == 0)
            return;

        Gizmos.color = Color.cyan;

        foreach (var xPair in pathPointsGrid)
        {
            foreach (var yPair in xPair.Value)
            {
                Vector3 point = yPair.Value;
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            AddSeatGroup();
        }
    }
}
