using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelEditorWindow : EditorWindow
{
    private int rows = 7;
    private int columns = 6;
    private int groupWidth = 3;
    private int groupHeight = 1;
    private float horizontalSpacing = 1.2f;
    private float verticalSpacing = 2.0f; // Default value, can be adjusted in the editor
    private GameObject[] capybaraPrefabs;
    private GameObject[] environmentPrefabs;
    private int selectedEnvironmentIndex = 0;
    private GameObject selectedEnvironmentPrefab;

    private int selectedCapybaraIndex = 0; // Dropdown için index
    private CapybaraColors colorPaletteAsset;
    private int selectedColorIndex = 0;
    private bool isFrozen = false;
    public GameObject selectedCapybaraPrefab;
    private Color selectedColor = Color.white; // Capybara için seçilen renk

    private GridSystem gridSystem;
    private bool isGridGenerated = false;

    // Slider için cellSize değişkeni
    public static float cellSize = 40f; // Hücre boyutu
    private string levelName = "New Level";
    private Difficulty difficulty = Difficulty.Easy;
    private bool isLocked = false;

    private Vector2 scrollPosition;

    private bool isTemporary = true;
    private List<GameObject> placedCapybaras = new List<GameObject>();

    [MenuItem("LevelDesign/Level Editor")]
    public static void ShowWindow()
    {
        GetWindow<LevelEditorWindow>("Level Editor");
    }

    void OnDisable()
    {
        if (isTemporary)
        {
            EditorApplication.delayCall += () =>
            {
                CleanHierarchy();
            };
        }
    }

    private void OnEnable()
    {
        // Resources/Capybaras klasöründeki tüm Capybara prefabs'larını al
        capybaraPrefabs = Resources.LoadAll<GameObject>("Capybaras");
        colorPaletteAsset = Resources.Load<CapybaraColors>("CapybaraColors");
        environmentPrefabs = Resources.LoadAll<GameObject>("Level");

        if (environmentPrefabs.Length > 0)
        {
            selectedEnvironmentPrefab = environmentPrefabs[0];
        }


        // Eğer Capybara prefab'ları varsa, ilkini varsayılan olarak seç
        if (capybaraPrefabs.Length > 0)
        {
            selectedCapybaraPrefab = capybaraPrefabs[0];
        }
    }

    private void OnGUI()
    {
        CheckGrid();

        // Grid Settings kısmı
        GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
        rows = EditorGUILayout.IntField("Rows", rows);
        columns = EditorGUILayout.IntField("Columns", columns);
        groupWidth = EditorGUILayout.IntField("Group Width", groupWidth);
        groupHeight = EditorGUILayout.IntField("Group Height", groupHeight);
        horizontalSpacing = EditorGUILayout.FloatField("Horizontal Spacing", horizontalSpacing);
        verticalSpacing = EditorGUILayout.FloatField("Vertical Spacing", verticalSpacing);

        GUILayout.Space(10);
        GUILayout.Label("Environment Prefab", EditorStyles.boldLabel);

        if (environmentPrefabs != null && environmentPrefabs.Length > 0)
        {
            // Create dropdown from prefab names
            string[] prefabNames = environmentPrefabs.Select(prefab => prefab.name).ToArray();
            selectedEnvironmentIndex = EditorGUILayout.Popup("Select Environment", selectedEnvironmentIndex, prefabNames);

            selectedEnvironmentPrefab = environmentPrefabs[selectedEnvironmentIndex];
        }
        else
        {
            GUILayout.Label("No environment prefabs found in Resources/Level folder!");
        }


        // Slider ile cellSize kontrolü
        GUILayout.Label("Cell Size", EditorStyles.boldLabel);
        cellSize = EditorGUILayout.Slider("Cell Size", cellSize, 1f, 70f); // min=1, max=10

        if (GUILayout.Button("Generate Grid"))
        {
            GenerateGrid();
        }

        if (GUILayout.Button("Delete Grid"))
        {
            CleanHierarchy();
            isGridGenerated = false;
        }

        // Capybara seçimi: Dropdown menüsü
        GUILayout.Space(20);
        GUILayout.Label("Capybara Selection", EditorStyles.boldLabel);

        // Capybara prefab'larının listesinde seçim yapabilmek için dropdown ekliyoruz
        if (capybaraPrefabs.Length > 0)
        {
            List<string> capybaraNames = new List<string>();
            foreach (var capybara in capybaraPrefabs)
            {
                capybaraNames.Add(capybara.name); // Prefab ismini listeye ekle
            }

            selectedCapybaraIndex = EditorGUILayout.Popup(
                "Select Capybara",
                selectedCapybaraIndex,
                capybaraNames.ToArray()
            );
            selectedCapybaraPrefab = capybaraPrefabs[selectedCapybaraIndex]; // Seçilen prefab'ı ata
        }
        else
        {
            GUILayout.Label("No Capybaras found in Resources/Capybaras folder!");
        }

        GUILayout.Label("Capybara Color", EditorStyles.boldLabel);

        if (colorPaletteAsset != null && colorPaletteAsset.colorPalette.Length > 0)
        {
            string[] colorNames = colorPaletteAsset
                .colorPalette.Select((c, i) => $"Color {i + 1}")
                .ToArray();

            selectedColorIndex = EditorGUILayout.Popup("Color", selectedColorIndex, colorNames);
            selectedColor = colorPaletteAsset.colorPalette[selectedColorIndex];
        }
        else
        {
            GUILayout.Label("No color palette found in Resources/CapybaraColors!");
        }

        GUILayout.Label("Capybara Properties", EditorStyles.boldLabel);
        isFrozen = EditorGUILayout.Toggle("Is Frozen", isFrozen);

        // Meta bilgilerini doldurabileceğimiz alan
        GUILayout.Space(20);
        GUILayout.Label("Meta Data", EditorStyles.boldLabel);
        levelName = EditorGUILayout.TextField("Level Name", levelName);
        difficulty = (Difficulty)EditorGUILayout.EnumPopup("Difficulty", difficulty);
        isLocked = EditorGUILayout.Toggle("Is Locked", isLocked);

        if (GUILayout.Button("Export Level"))
        {
            ExportLevel(levelName, difficulty, isLocked);
        }
        isTemporary = EditorGUILayout.Toggle("Is Temporary", isTemporary);

        // Eğer grid oluşturulduysa grid'i görselleştir
        if (isGridGenerated)
        {
            GUILayout.Space(20);
            GUILayout.Label("Click on Seats to Place Capybara", EditorStyles.boldLabel);

            // Scrollable area to display the grid cells
            scrollPosition = GUILayout.BeginScrollView(
                scrollPosition,
                GUILayout.Width(position.width),
                GUILayout.Height(position.height - 250)
            ); // Meta kısmından sonra başlar
            DrawGrid();
            GUILayout.EndScrollView();
        }
    }

    private void CleanHierarchy()
    {
        if (gridSystem != null && gridSystem.groupsParent != null)
        {
            Transform parent = gridSystem.groupsParent;
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Transform child = parent.GetChild(i);
                if (child != null)
                    DestroyImmediate(child.gameObject);
            }
        }

        for (int i = placedCapybaras.Count - 1; i >= 0; i--)
        {
            if (placedCapybaras[i] != null)
                DestroyImmediate(placedCapybaras[i]);
        }

        placedCapybaras.Clear();
    }

    private void GenerateGrid()
    {
        gridSystem = FindObjectOfType<GridSystem>();

        if (gridSystem == null)
        {
            Debug.LogError("GridSystem component is missing in the scene!");
            return;
        }

        // Grid boyutlarını ve grup yapılarını GridSystem'e aktar
        gridSystem.rows = rows;
        gridSystem.columns = columns;
        gridSystem.groupWidth = groupWidth;
        gridSystem.groupHeight = groupHeight;
        gridSystem.horizontalSpacing = horizontalSpacing;
        gridSystem.verticalSpacing = verticalSpacing;

        // Grid'i oluştur
        gridSystem.GenerateGrid();

        isGridGenerated = true;
    }

    private void CheckGrid()
    {
        gridSystem = FindObjectOfType<GridSystem>();

        if (gridSystem == null)
        {
            isGridGenerated = false;
        }

        if (gridSystem.groupsParent.childCount != 0)
        {
            // Grid boyutlarını ve grup yapılarını GridSystem'e aktar
            gridSystem.rows = rows;
            gridSystem.columns = columns;
            gridSystem.groupWidth = groupWidth;
            gridSystem.groupHeight = groupHeight;
            gridSystem.horizontalSpacing = horizontalSpacing;
            gridSystem.verticalSpacing = verticalSpacing;

            isGridGenerated = true;
        }
    }

    // Grid hücrelerini çizme ve tıklama işlemi
    private void DrawGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                Rect cellRect = new Rect(i * cellSize, j * cellSize, cellSize, cellSize);

                // Hücreyi çiz
                GUI.Box(cellRect, $"({i},{j})");

                // Hücreye tıklama işlemi
                if (
                    Event.current.type == EventType.MouseDown
                    && cellRect.Contains(Event.current.mousePosition)
                )
                {
                    // Seçilen Capybara'yı yerleştirme işlemi
                    PlaceCapybaraInScene(i, j);
                    Event.current.Use(); // Mouse tıklamasını işaretle
                }
            }
        }
    }

    // Tıklanan koltuğa Capybara yerleştirme
    private void PlaceCapybaraInScene(int x, int y)
    {
        if (selectedCapybaraPrefab == null)
        {
            Debug.LogError("No Capybara selected!");
            return;
        }

        // GridSystem üzerinden var olan Seat'i bul
        gridSystem = FindObjectOfType<GridSystem>();
        if (gridSystem == null)
        {
            Debug.LogError("GridSystem not found!");
            return;
        }

        Seat seat = gridSystem.GetSeatAtPosition(new Vector2Int(x, y)); // Seat'ı pozisyona göre al

        if (seat == null)
        {
            Debug.LogError($"No seat found at grid position ({x}, {y})");
            return;
        }

        if (!seat.IsEmpty)
        {
            Debug.LogWarning($"Seat at ({x}, {y}) is already occupied!");
            return;
        }

        bool isFat = selectedCapybaraPrefab.GetComponent<Capybara>().Type == CapybaraType.Fat;
        Vector3 spawnPos = seat.transform.position;

        // Capybara'yı seat'in pozisyonuna yerleştir
        GameObject capybaraObj = Instantiate(selectedCapybaraPrefab, spawnPos, Quaternion.identity);
        Capybara capybara = capybaraObj.GetComponent<Capybara>();

        if (capybara != null)
        {
            seat.currentCapybara = capybara;
            capybara.SitSeat(seat);

            if (isFrozen)
            {
                capybara.Freeze();
            }
        }


#if UNITY_EDITOR
        capybaraObj.GetComponent<Capybara>().SetColor(selectedColor);
#else
        capybaraObj.GetComponent<Capybara>().SetColor(selectedColor);
#endif


        placedCapybaras.Add(capybaraObj);
    }

    private void ExportLevel(string levelName, Difficulty difficulty, bool isLocked)
    {
        LevelData newLevel = ScriptableObject.CreateInstance<LevelData>();
        newLevel.levelName = levelName;
        newLevel.difficulty = difficulty;
        newLevel.isLocked = isLocked;
        newLevel.rows = rows;
        newLevel.columns = columns;
        newLevel.groupWidth = groupWidth;
        newLevel.groupHeight = groupHeight;
        newLevel.horizontalSpacing = horizontalSpacing;
        newLevel.verticalSpacing = verticalSpacing;
        newLevel.levelEnvironment = selectedEnvironmentPrefab;

        // Grid ve Capybara bilgilerini al
        List<LevelData.CapybaraInfo> capybaras = new();
        foreach (var seatGroup in FindObjectsOfType<SeatGroup>())
        {
            foreach (var seat in seatGroup.seatsInGroup)
            {
                if (seat.currentCapybara != null)
                {
                    Capybara capy = seat.currentCapybara.GetComponent<Capybara>();

                    LevelData.CapybaraInfo capyInfo = new LevelData.CapybaraInfo
                    {
                        gridPosition = seat.gridPosition,
                        type = capy.Type,
                        isFrozen = capy.IsFrozen,
                        color = seat.currentCapybara.color,
                    };
                    capybaras.Add(capyInfo);
                }
            }
        }

        newLevel.capybaras = capybaras.ToArray();

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Level Data",
            "NewLevel",
            "asset",
            "LevelData"
        );
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(newLevel, path);
            AssetDatabase.SaveAssets();
            Debug.Log($"Level exported to {path}");
        }
    }
}
