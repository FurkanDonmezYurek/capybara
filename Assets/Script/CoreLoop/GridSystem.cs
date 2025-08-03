using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public struct GridBounds
{
    public Vector3 center;
    public Vector3 size;
}

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

    //for AddSeat
    private List<GameObject> inactiveGroups = new(); // boş grupları tutmak için
    private int extraGroupCount = 2; // fazladan oluşturulacak grup sayısı

    // ...

    [ContextMenu("Generate Grid and Groups")] //for run in the editor
    public void GenerateGrid()
    {
        var seats = new Seat[columns, rows];

        int baseGroupCountX = columns / groupWidth;
        int baseGroupCountY = rows / groupHeight;
        int totalGroupCount = (baseGroupCountX * baseGroupCountY) + extraGroupCount;

        groupPositions = new Vector3[
            baseGroupCountY + (extraGroupCount / 2),
            baseGroupCountX + (extraGroupCount / 2)
        ];

        for (int i = 0; i < totalGroupCount; i++)
        {
            int groupY = i / baseGroupCountX;
            int groupX = i % baseGroupCountX;

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
                    Vector3 pos =
                        new Vector3(x * horizontalSpacing, 0, -y * verticalSpacing) + groupOffset;

#if UNITY_EDITOR
                    GameObject obj =
                        PrefabUtility.InstantiatePrefab(seatPrefab, groupsParent) as GameObject;
#else
                    GameObject obj = Instantiate(seatPrefab, groupsParent);
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
            group.groupY = groupY;
            group.groupX = groupX;

            groupPositions[groupY, groupX] = GetSeatGroupCenter(group);

            foreach (var seat in groupSeats)
                seat.groupOfSeat = group;

            // Fazladan olanlar için SetActive(false) yap
            if (i >= baseGroupCountX * baseGroupCountY)
            {
                inactiveGroups.Add(gObj);
            }
        }
    }

    // Adds seats in order to the current grid.
    [ContextMenu("Add Seat Group")]
    public void AddSeatGroup()
    {
        if (inactiveGroups.Count == 0)
        {
            Debug.LogWarning("No inactive groups left to activate!");
            return;
        }

        GameObject groupToActivate = inactiveGroups[0];
        groupToActivate.SetActive(true);
        inactiveGroups.RemoveAt(0);

        Debug.Log($"✅ Activated {groupToActivate.name}");

        if (Application.isPlaying)
        {
            GameManager.Instance.ClearSeatGroupCache();
            GameManager.Instance.InitializeSeatGroupsCache();
            RecalculateGroupPositions();
            InitPathGrid();
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

    public void SetGridParameters(
        int rows,
        int columns,
        int groupWidth,
        int groupHeight,
        float horizontalSpacing = 1.2f,
        float verticalSpacing = 1.2f
    )
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
        int groupCountX = columns / groupWidth;
        int groupCountY = rows / groupHeight + (extraGroupCount / 2);

        int pointColumn = groupCountX - 1;
        int pointRow = groupCountY;

        pathPointsGrid = new Dictionary<string, Dictionary<string, Vector3>>();

        for (int x = 0; x < pointColumn; x++)
        {
            string xKey = x.ToString();
            if (!pathPointsGrid.ContainsKey(xKey))
                pathPointsGrid[xKey] = new Dictionary<string, Vector3>();

            for (int y = 0; y < pointRow; y++)
            {
                string yKey = y.ToString();

                if (x + 1 >= groupPositions.GetLength(1) || y >= groupPositions.GetLength(0))
                {
                    Debug.LogWarning($"❌ Out of bounds at x={x}, y={y}");
                    continue;
                }

                float xPos = (groupPositions[y, x].x + groupPositions[y, x + 1].x) / 2;
                float yPos = groupPositions[y, x].y;
                float zPos = groupPositions[y, x].z;

                Vector3 pointPosition = new Vector3(xPos, yPos, zPos);
                Debug.Log(pointPosition);
                pathPointsGrid[xKey][yKey] = pointPosition;
            }
        }

        GameManager.Instance.InitializeSeatGroupsCache();
        foreach (var item in inactiveGroups)
        {
            item.SetActive(false);
        }
        Debug.Log("Path grid initialized.");
    }

    public GridBounds GetReferenceArea()
    {
        Transform topLeft = GameObject.Find("GridCorner_TopLeft")?.transform;
        Transform topRight = GameObject.Find("GridCorner_TopRight")?.transform;
        Transform bottomLeft = GameObject.Find("GridCorner_BottomLeft")?.transform;
        Transform bottomRight = GameObject.Find("GridCorner_BottomRight")?.transform;

        if (topLeft == null || topRight == null || bottomLeft == null || bottomRight == null)
        {
            Debug.LogError("One or more grid corner points are missing in LevelEnvironment.");
            return default;
        }

        float minX = Mathf.Min(topLeft.position.x, bottomLeft.position.x);
        float maxX = Mathf.Max(topRight.position.x, bottomRight.position.x);
        float minZ = Mathf.Min(bottomLeft.position.z, bottomRight.position.z);
        float maxZ = Mathf.Max(topLeft.position.z, topRight.position.z);

        Vector3 center = new Vector3((minX + maxX) / 2f, 0f, (minZ + maxZ) / 2f);
        Vector3 size = new Vector3((maxX - minX), 0f, (maxZ - minZ));

        return new GridBounds { center = center, size = size };
    }

    public GridBounds GetCurrentGridArea()
    {
        Renderer[] allRenderers = groupsParent.gameObject.GetComponentsInChildren<Renderer>();
        if (allRenderers.Length == 0)
            return default;

        Bounds totalBounds = allRenderers[0].bounds;

        for (int i = 1; i < allRenderers.Length; i++)
            totalBounds.Encapsulate(allRenderers[i].bounds);

        return new GridBounds { center = totalBounds.center, size = totalBounds.size };
    }

    public void FitGridToReferenceArea()
    {
        Debug.Log("⏳ FitGridToReferenceArea started");

        GridBounds refArea = GetReferenceArea();
        // Önce scale'i sıfırla (1,1,1)
        groupsParent.position = Vector3.one;
        groupsParent.transform.localScale = Vector3.one;
        GridBounds gridArea = GetCurrentGridArea();

        Debug.Log($"🎯 RefArea center: {refArea.center}, size: {refArea.size}");
        Debug.Log($"🧱 GridArea center: {gridArea.center}, size: {gridArea.size}");

        if (
            refArea.size.x == 0
            || refArea.size.z == 0
            || gridArea.size.x == 0
            || gridArea.size.z == 0
        )
        {
            Debug.LogWarning("Reference or Grid area invalid.");
            return;
        }

        float scaleX = refArea.size.x / gridArea.size.x;
        float scaleZ = refArea.size.z / gridArea.size.z;

        float uniformScale = Mathf.Min(scaleX, scaleZ);
        groupsParent.localScale = new Vector3(uniformScale, 1f, uniformScale);

        gridArea = GetCurrentGridArea(); // scaled sonrası tekrar hesapla
        Vector3 offset = refArea.center - gridArea.center;
        groupsParent.position += offset;

        // Y hizalama
        GameObject yRef = GameObject.Find("GridYReference");
        if (yRef != null)
        {
            Vector3 pos = groupsParent.position;
            Debug.Log($"🔧 Setting Y from {pos.y} to {yRef.transform.position.y}");
            pos.y = yRef.transform.position.y;
            groupsParent.position = pos;
        }
        else
        {
            Debug.LogWarning("❌ GridYReference not found!");
        }

        RecalculateGroupPositions();
        InitPathGrid();
        Debug.Log("✅ FitGridToReferenceArea finished");
    }

    public void RecalculateGroupPositions()
    {
        int groupCountY = rows / groupHeight;
        int groupCountX = columns / groupWidth;

        groupPositions = new Vector3[
            groupCountY + (extraGroupCount / 2),
            groupCountX + (extraGroupCount / 2)
        ];

        foreach (
            var group in groupsParent.GetComponentsInChildren<SeatGroup>(includeInactive: true)
        )
        {
            groupPositions[group.groupY, group.groupX] = GetSeatGroupCenter(group);
        }

        Debug.Log("✅ groupPositions updated after fitting.");
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
