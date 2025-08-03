using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTutorialOnClick : MonoBehaviour

{
    public GameObject tutorialPanelToHide; // buraya başla popup’unu ata

    public void HidePanel()
    {
        tutorialPanelToHide.SetActive(false); // popup kapanır
    }
}
