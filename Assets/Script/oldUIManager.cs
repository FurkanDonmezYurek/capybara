using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oldUIManager : MonoBehaviour
{
    public GameObject splashScreenPanel;
    public GameObject mainMenuPanel;
    public GameObject mapPanel;
    public GameObject loadingPanel;
    public GameObject winPanel;
    public GameObject losePanel;

    private void Start()
    {
        ShowSplash();
    }

    public void ShowSplash()
    {
        splashScreenPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        mapPanel.SetActive(false);
        loadingPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        Invoke("ShowMainMenu", 2f); // 2 saniye sonra menüye geç
    }

    public void ShowMainMenu()
    {
        splashScreenPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenMap()
    {
        mainMenuPanel.SetActive(false);
        mapPanel.SetActive(true);
    }

    public void ShowWin()
    {
        winPanel.SetActive(true);
    }

    public void ShowLose()
    {
        losePanel.SetActive(true);
    }
}