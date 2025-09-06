using UnityEngine;

public class LevelCheckpoint : MonoBehaviour
{
    public bool isOpen = false;
    public GameObject openObje;
    public GameObject closedClosed;

    public int vehicleTypeIndex;

    public float rayDistance = 200f;

    public int LevelIndex;
    public void UpdateVisual()
    {
         openObje.SetActive(isOpen);
         closedClosed.SetActive(!isOpen);
    }
    bool TapBegan()
    {
#if UNITY_IOS || UNITY_ANDROID
        return Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

    Vector3 TapPosition()
    {
#if UNITY_IOS || UNITY_ANDROID
        return Input.GetTouch(0).position;
#else
        return Input.mousePosition;
#endif
    }

    void Update()
    {
        if (!TapBegan() || !isOpen) return;

        Ray ray = Camera.main.ScreenPointToRay(TapPosition());
        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            if (hit.transform == transform)
            {
                Debug.Log("Nesneye Týklandý "+LevelIndex);
                if (!IdleUIManager.Instance.IsStartLevelPanelActive())
                {
                    SetSelectedLevel();
                    IdleUIManager.Instance.ShowLevelCheckPointPanel(LevelIndex);
                }
            }


        }
    }
    public void SetSelectedLevel()
    {
        PlayerPrefs.SetInt("SelectedLevel",LevelIndex);
        PlayerPrefs.Save();
    }
}

