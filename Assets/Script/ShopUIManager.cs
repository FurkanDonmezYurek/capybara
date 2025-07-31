using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private GameObject shopPanel;

    public void ShowShopPanel()
    {
        shopPanel.SetActive(true);
    

    }

    public void CloseShopPanel()
    {
        shopPanel.SetActive(false);
    }
}
