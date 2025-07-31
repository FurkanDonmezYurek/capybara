using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject splashScreenPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject shopPanel;

    // Tüm panelleri kapatır
    private void HideAllPanels()
    {
        splashScreenPanel?.SetActive(false);
        mainMenuPanel?.SetActive(false);
        mapPanel?.SetActive(false);
        winPanel?.SetActive(false);
        losePanel?.SetActive(false);
        settingsPanel?.SetActive(false);
        shopPanel?.SetActive(false);
    }

    // Ayarlar panelini gösterir
    public void ShowSettingsPanel()
    {
        HideAllPanels();
        settingsPanel?.SetActive(true);
    }

    // Mağaza panelini gösterir
    public void ShowShopPanel()
    {
        HideAllPanels();
        shopPanel?.SetActive(true);
    }

    // Ana menü panelini gösterir (örnek ekledim)
    public void ShowMainMenuPanel()
    {
        HideAllPanels();
        mainMenuPanel?.SetActive(true);
    }
}
