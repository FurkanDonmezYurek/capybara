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
    public Difficulty difficulty;
    public bool isLocked = false;

    [Header("Grid Structure")]
    public int rows;
    public int columns;

    [Header("Group Structure")]
    public int groupWidth;
    public int groupHeight;

    [System.Serializable]
    public class CapybaraInfo
    {
        public Vector2Int gridPosition;
        public Color color;
    }

    [Header("Capybaras")]
    public CapybaraInfo[] capybaras;
}
