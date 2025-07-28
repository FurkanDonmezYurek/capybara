using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LevelButtonHandler : MonoBehaviour
{
    public GameObject popupPanel;
    public TMPro.TextMeshProUGUI levelText;
    public TMPro.TextMeshProUGUI rewardText;

    public int levelNumber;
    public int rewardAmount;

    public void ShowPopup()
    {
        popupPanel.SetActive(true);
        levelText.text = "Level " + levelNumber;
        rewardText.text = "Ödül: " + rewardAmount + " coin";
    }
}