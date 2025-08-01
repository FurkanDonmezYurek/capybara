using UnityEngine;

[CreateAssetMenu(fileName = "LevelDatabase", menuName = "CapybaraSort/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    public LevelData[] levels;
}
