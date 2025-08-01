using UnityEngine;

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
}

[CreateAssetMenu(fileName = "LevelData", menuName = "CapybaraSort/LevelData")]
public class LevelData : ScriptableObject
{
    [Header("Meta")]
    public string levelName;
    public GameObject levelEnvironment;
    public Difficulty difficulty;
    public float levelTime = 120f; // Default 2 minutes
    public bool isLocked = false;
    [Header("Grid Structure")]
    public int rows;
    public int columns;

    [Header("Group Structure")]
    public int groupWidth;
    public int groupHeight;

    [Header("Seat Settings")]
    public float horizontalSpacing = 1.2f;
    public float verticalSpacing = 1.2f;

    [System.Serializable]
    public class CapybaraInfo
    {
        public Vector2Int gridPosition;
        public CapybaraType type;
        public bool isFrozen;
        public Color color;
    }

    [Header("Capybaras")]
    public CapybaraInfo[] capybaras;
}
