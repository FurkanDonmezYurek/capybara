using UnityEngine;
using System;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int Coin { get; private set; }

    public Action<int> OnCoinChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        Coin = PlayerPrefs.GetInt("Coin", 0);
    }

    public void AddCoin(int amount)
    {
        Coin += amount;
        PlayerPrefs.SetInt("Coin", Coin);
        OnCoinChanged?.Invoke(Coin);
    }

    public bool SpendCoin(int amount)
    {
        if (Coin < amount) return false;

        Coin -= amount;
        PlayerPrefs.SetInt("Coin", Coin);
        OnCoinChanged?.Invoke(Coin);
        return true;
    }
}
