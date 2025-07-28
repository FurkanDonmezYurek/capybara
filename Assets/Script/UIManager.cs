using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class UIManager : MonoBehaviour


{
    public GameObject loadingPanel;

    public void LoadLevel1()
    {
        loadingPanel.SetActive(true);
        StartCoroutine(LoadLevelAsync("Level1"));
    }

    private IEnumerator LoadLevelAsync(string levelName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(levelName);
    }

public GameObject shopPanel;
public GameObject settingsPanel;

public void OpenShop()
{
    shopPanel.SetActive(true);
}

public void CloseShop()
{
    shopPanel.SetActive(false);
}

public void OpenSettings()
{
    settingsPanel.SetActive(true);
}

public void CloseSettings()
{
    settingsPanel.SetActive(false);
}
}