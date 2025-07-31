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

    [Header("Coin Buy Panel")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject coinBuyPanel;
    [SerializeField] private CanvasGroup coinBuyCG;
    [SerializeField] private Transform coinBuyHeader;
    [SerializeField] private Transform coinBuyImage;
    [SerializeField] private Transform coinBuyText;
    [SerializeField] private Transform coinBuyButton;
    [SerializeField] private Button coinBuyButtonNoConnection;

    [Header("Settings Panel")]
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private CanvasGroup settingsCG;
    [SerializeField] private Transform settingsContent;
    [SerializeField] private Button soundToggleButton;
    [SerializeField] private Image soundToggleIcon;
    [SerializeField] private Image soundToggleBackground;
    [SerializeField] private Button vibrationToggleButton;
    [SerializeField] private Image vibrationToggleIcon;
    [SerializeField] private Image vibrationToggleBackground;
    [SerializeField] private Sprite[] iconSounds;
    [SerializeField] private Sprite[] iconVibrations;
    [SerializeField] private Sprite[] bgSettingsButtons;

    private Tween coinTween;
    public int selectedLevelIndex;
    private bool isSoundOn;
    private bool isVibrationOn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        closeButton.onClick.AddListener(HideAllPanels);

        soundToggleButton.onClick.AddListener(ToggleSound);
        vibrationToggleButton.onClick.AddListener(ToggleVibration);

        isSoundOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;

        CurrencyManager.Instance.OnCoinChanged += UpdateCoin;
        UpdateCoin(CurrencyManager.Instance.Coin);

        UpdateSoundToggleVisual();
        UpdateVibrationToggleVisual();
    }

    #region === Panel On/Off ===

    public void HideAllPanels()
    {
        if (startLevelPanel.activeSelf)
            HidePanelWithAnimation(startLevelCG, playButtonTransform, startLevelPanel);

        if (coinBuyPanel.activeSelf)
            HidePanelWithAnimation(coinBuyCG, coinBuyHeader, coinBuyPanel);

        if (settingsPanel.activeSelf)
            HidePanelWithAnimation(settingsCG, settingsContent, settingsPanel);
    }

    private void HidePanelWithAnimation(CanvasGroup cg, Transform scaleTarget, GameObject panelGO)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(0f, 0.3f));
        seq.Join(scaleTarget.DOScale(0.8f, 0.3f).SetEase(Ease.InBack));
        seq.OnComplete(() => panelGO.SetActive(false));
    }

    #endregion

    #region === Start Level ===

    public void OpenStartLevelPanel()
    {
        HideAllPanels();

        //selectedLevelIndex = GameManager.Instance.levelManager.GetCurrentLevelIndex();

        selectedLevelText.text = "LEVEL " + (selectedLevelIndex + 1);

        startLevelPanel.SetActive(true);
        startLevelCG.alpha = 0f;

        UIAnimator.FadeIn(startLevelCG);
        UIAnimator.MoveFromY(header, 200f, 0.4f, Ease.OutBack, 0f);
        UIAnimator.ScaleIn(levelImage, 0.4f, 0.1f);
        UIAnimator.MoveFromX(selectedLevelText.transform, -800f, 0.4f, Ease.OutExpo, 0.2f);
        UIAnimator.ScaleIn(playButtonTransform, 0.4f, 0.35f);
    }



    private void OnPlayButtonClicked()
    {
        HideAllPanels();
        int LevelIndex = PlayerPrefs.GetInt("Level", 0);
        LevelIndex++;
        PlayerPrefs.SetInt("Level", LevelIndex);
        //GameManager.Instance.LevelStart();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }



    #endregion

    #region === Settings ===

    public void ShowSettingsPanel()
    {
        HideAllPanels();
        settingsPanel.SetActive(true);
        settingsCG.alpha = 0;
        settingsContent.localScale = Vector3.zero;

        UIAnimator.FadeIn(settingsCG);
        UIAnimator.ScaleIn(settingsContent);

        UpdateSoundToggleVisual();
        UpdateVibrationToggleVisual();
    }

    public void ToggleSound()
    {
        isSoundOn = !isSoundOn;
        PlayerPrefs.SetInt("Sound", isSoundOn ? 1 : 0);
        UpdateSoundToggleVisual();
    }

    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
        UpdateVibrationToggleVisual();
    }

    private void UpdateSoundToggleVisual()
    {
        soundToggleIcon.sprite = isSoundOn ? iconSounds[1] : iconSounds[0];
        soundToggleBackground.sprite = isSoundOn ? bgSettingsButtons[1] : bgSettingsButtons[0];
    }

    private void UpdateVibrationToggleVisual()
    {
        vibrationToggleIcon.sprite = isVibrationOn ? iconVibrations[1] : iconVibrations[0];
        vibrationToggleBackground.sprite = isVibrationOn ? bgSettingsButtons[1] : bgSettingsButtons[0];
    }

    #endregion

    #region === Coin Buy ===

    public void ShowCoinBuyPanel()
    {
        HideAllPanels();
        coinBuyPanel.SetActive(true);

        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        UIAnimator.FadeIn(coinBuyCG);
        UIAnimator.ScaleIn(coinBuyHeader);
        UIAnimator.ScaleIn(coinBuyImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(coinBuyText, -1000, 0.3f, Ease.OutExpo, 0.6f);

        if (hasInternet)
        {
            coinBuyButton.gameObject.SetActive(true);
            coinBuyButtonNoConnection.gameObject.SetActive(false);
            UIAnimator.ScaleIn(coinBuyButton, 0.3f, 0.9f);
        }
        else
        {
            coinBuyButton.gameObject.SetActive(false);
            coinBuyButtonNoConnection.gameObject.SetActive(true);
            UIAnimator.ScaleIn(coinBuyButtonNoConnection.transform, 0.3f, 0.9f);
        }
    }

    public void UpdateCoin(int amount)
    {
        AnimateCoinChange(amount);
    }

    public void BuyCoins(int coinAmount)
    {
        //TODO: Rewarded Ad Integration

        CurrencyManager.Instance.AddCoin(coinAmount);

        HideAllPanels();
    }

    public void AnimateCoinChange(int newCoinAmount)
    {
        if (coinTween != null && coinTween.IsActive()) coinTween.Kill();

        int current = int.Parse(coinText.text);

        coinText.transform.localScale = Vector3.one * 1.2f;
        coinText.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);

        coinTween = DOTween.To(() => current, x =>
        {
            current = x;
            coinText.text = current.ToString();
        }, newCoinAmount, 0.5f).SetEase(Ease.OutQuad);
    }


    #endregion
}
