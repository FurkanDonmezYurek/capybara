using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro kullanıyorsanız

public class LevelLockManager : MonoBehaviour
{
    [Header("Level Button Elements")]
    [SerializeField] private Button[] levelButtons; 
    [SerializeField] private GameObject[] lockIcons; 
    [SerializeField] private TextMeshProUGUI[] levelTexts; 

    private int currentLevel = 1; 

    private void Start()
    {
        UpdateLevelLock();
    }

    public void UpdateLevelLock()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i < currentLevel) 
            {
                levelButtons[i].interactable = true;
                lockIcons[i].SetActive(false); 
             
            }
            else
            {
                levelButtons[i].interactable = false;
                lockIcons[i].SetActive(true); 
            
            }
        }
    }

    public void UnlockNextLevel()
    {
        if (currentLevel < levelButtons.Length) 
        {
            currentLevel++;
            UpdateLevelLock(); 
        }
    }
}
