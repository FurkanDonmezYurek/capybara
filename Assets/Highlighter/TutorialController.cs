using UnityEngine;

public class TutorialController : MonoBehaviour
{
    public GameObject firstPanel;  // Tap to Screen paneli
    public GameObject secondPanel; // Başla butonunu gösteren panel

    private bool step1Completed = false;

    void Update()
    {
        if (!step1Completed && Input.GetMouseButtonDown(0))
        {
            firstPanel.SetActive(false);
            secondPanel.SetActive(true);
            step1Completed = true;
        }
    }
}
