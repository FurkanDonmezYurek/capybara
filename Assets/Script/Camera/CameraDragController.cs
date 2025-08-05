using DG.Tweening;
using UnityEngine;

public class CameraDragController : MonoBehaviour
{
    [SerializeField] private float dragSpeed = 5f;

    [SerializeField] private Vector2 MinMaxX ;
    [SerializeField] private Vector2 MinMaxZ ;

    private Vector3 lastMousePosition;
    private Vector3 tutorialStartPosition;

    private bool isDragging = false;
    private int isTurorial ;
    private void Start()
    {
        isTurorial=PlayerPrefs.GetInt("HasSeenIdleTutorial", 0);
    }
    void Update()
    {
        if (isTurorial == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
                IdleUIManager.Instance.HideTutorialPanel();
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;

                if (isTurorial == 0)
                {
                    transform.DOMove(tutorialStartPosition, 0.5f)
                        .SetEase(Ease.InOutQuad)
                        .OnComplete(() =>
                        {
                            if (!IdleUIManager.Instance.IsStartLevelPanelActive() && !PlayerPrefs.HasKey("HasSeenIdleTutorial"))
                                IdleUIManager.Instance.ShowTutorial();
                        });
                }
            }


            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;

                Vector3 move = dragSpeed * Time.deltaTime * new Vector3(-delta.x, 0, -delta.y);

                Vector3 newPosition = transform.position + move;

                newPosition.x = Mathf.Clamp(newPosition.x, MinMaxX.x, MinMaxX.y);
                newPosition.z = Mathf.Clamp(newPosition.z, MinMaxZ.x, MinMaxZ.y);

                transform.position = newPosition;

                lastMousePosition = Input.mousePosition;
            }
        }
        else
        {
            if (IdleUIManager.Instance.IsStartLevelPanelActive())
            {
                isDragging = false;
                return;
            }
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;

                Vector3 move = dragSpeed * Time.deltaTime * new Vector3(-delta.x, 0, -delta.y);

                Vector3 newPosition = transform.position + move;

                newPosition.x = Mathf.Clamp(newPosition.x, MinMaxX.x, MinMaxX.y);
                newPosition.z = Mathf.Clamp(newPosition.z, MinMaxZ.x, MinMaxZ.y);

                transform.position = newPosition;

                lastMousePosition = Input.mousePosition;
            }
        }

    }
    public void SetTutorialStartPosition(Vector3 pos)
    {
        if (tutorialStartPosition == Vector3.zero)
        {
            tutorialStartPosition = new Vector3(pos.x, pos.y - 3f, pos.z);
        }
    }

}
