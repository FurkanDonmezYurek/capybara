using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public Transform[] regionParents;

    void Start()
    {
        for (int i = 0; i < regionParents.Length; i++)
        {
            for (int j = 0; j < 5; j++) // örneğin her bölgede 5 level
            {
                GameObject btn = Instantiate(levelButtonPrefab, regionParents[i]);
                //btn.GetComponentInChildren<TextMeshPro>().text = $"Level {j + 1}";
                int levelIndex = j + 1;
                //btn.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelIndex));
            }
        }
    }

    void LoadLevel(int index)
    {
        Debug.Log("Load level: " + index);
        // SceneManager.LoadScene(...)
    }
}