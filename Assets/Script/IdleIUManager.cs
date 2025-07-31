using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class IdleUIManager : MonoBehaviour
{
    public static IdleUIManager Instance;

    [Header("Start Level Panel")]
    [SerializeField] private GameObject startLevelPanel;
    [SerializeField] private CanvasGroup startLevelCG;

    [SerializeField] private Transform header;
    [SerializeField] private Transform levelImage;
    [SerializeField] private TextMeshProUGUI selectedLevelText;

    [SerializeField] private Button playButton;
    [SerializeField] private Transform playButtonTransform;

    [SerializeField] private Button closeButton;

    private int selectedLevelIndex;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        closeButton.onClick.AddListener(CloseStartLevelPanel);
    }

    public void OpenStartLevelPanel(int levelIndex)
    {
        selectedLevelIndex = levelIndex;
        selectedLevelText.text = "LEVEL " + (levelIndex);

        startLevelPanel.SetActive(true);
        startLevelCG.alpha = 0f;

        UIAnimator.FadeIn(startLevelCG);
        UIAnimator.MoveFromY(header, 200f, 0.4f, Ease.OutBack, 0f);
        UIAnimator.ScaleIn(levelImage, 0.4f, 0.1f);
        UIAnimator.MoveFromX(selectedLevelText.transform, -800f, 0.4f, Ease.OutExpo, 0.2f);
        UIAnimator.ScaleIn(playButtonTransform, 0.4f, 0.35f);
    }

    public void CloseStartLevelPanel()
    {
        HidePanelWithAnimation(startLevelCG, header, startLevelPanel);
    }
    private void HidePanelWithAnimation(CanvasGroup cg, Transform scaleTarget, GameObject panelGO)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(0f, 0.3f));
        seq.Join(scaleTarget.DOScale(0.8f, 0.3f).SetEase(Ease.InBack));
        seq.OnComplete(() => panelGO.SetActive(false));
    }

    private void OnPlayButtonClicked()
    {
        //TODO: Start the selected level
        SceneManager.LoadScene(1);
    }
}
