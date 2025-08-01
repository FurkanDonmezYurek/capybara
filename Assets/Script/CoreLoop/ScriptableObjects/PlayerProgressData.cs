using UnityEngine;

[CreateAssetMenu(fileName = "PlayerProgressData", menuName = "CapybaraSort/Player Progress")]
public class PlayerProgressData : ScriptableObject
{
    [Header("Currencies")]
    public int softCurrency = 0;
    public int hardCurrency = 0;

    [Header("Progress")]
    public int maxReachedLevel = 0;

    [Header("Other")]
    public int adsWatched = 0; // E.g., for ad rewards
}

