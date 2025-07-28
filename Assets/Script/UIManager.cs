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
}
