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
}
