using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Top Bar Elements")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image timerFill;
    [SerializeField] private Button settingsButton;

    [Header("Booster")]
    [SerializeField] private Button boosterButton;

    [Header("Panels")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject levelFailPanel;
    [SerializeField] private GameObject boosterPanel;
    [SerializeField] private GameObject coinBuyPanel;
    [SerializeField] private GameObject boosterUnlockedPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Level Complete Panel Elements")]
    [SerializeField] private CanvasGroup levelCompleteCG;
    [SerializeField] private Transform levelCompleteHeader;
    [SerializeField] private Transform levelCompleteCoinIcon;
    [SerializeField] private Transform levelCompleteNextButton;
    [SerializeField] private Image levelCompleteShine;

    [Header("Level Fail Panel Elements")]
    [SerializeField] private CanvasGroup levelFailCG;
    [SerializeField] private Transform levelFailHeader;
    [SerializeField] private Transform levelFailPlusTimeImage;
    [SerializeField] private Transform levelFailText;
    [SerializeField] private Transform levelFailButtons;

    [Header("Booster Panel Elements")]
    [SerializeField] private CanvasGroup boosterCG;
    [SerializeField] private Transform boosterHeader;
    [SerializeField] private Transform boosterIcon;
    [SerializeField] private Transform boosterText;
    [SerializeField] private Transform boosterButtons;
    [SerializeField] private Button unlockBoosterWithCoinButton;
    [SerializeField] private Button unlockBoosterWithAdsButton;

    [Header("Coin Buy Panel Elements")]
    [SerializeField] private CanvasGroup coinBuyCG;
    [SerializeField] private Transform coinBuyHeader;
    [SerializeField] private Transform coinBuyImage;
    [SerializeField] private Transform coinBuyText;
    [SerializeField] private Transform coinBuyButton;

    [Header("Booster Unlocked Panel Elements")]
    [SerializeField] private CanvasGroup boosterUnlockedCG;
    [SerializeField] private Image boosterUnlockedBGGlow;
    [SerializeField] private Transform boosterUnlockedIcon;
    [SerializeField] private CanvasGroup boosterTextImage;

    [Header("Settings Panel Elements")]
    [SerializeField] private CanvasGroup settingsCG;
    [SerializeField] private Transform settingsContent;

    [SerializeField] private Button soundToggleButton;
    [SerializeField] private Image soundToggleIcon;       
    [SerializeField] private Image soundToggleBackground; 

    [SerializeField] private Button vibrationToggleButton;
    [SerializeField] private Image vibrationToggleIcon;      
    [SerializeField] private Image vibrationToggleBackground; 

    [SerializeField] private Button restartButton;

    [SerializeField] private Sprite[] iconSounds;
    [SerializeField] private Sprite[] iconVibrations;
    [SerializeField] private Sprite[] bgSettingsButtons;


    private bool isSoundOn = false;
    private bool isVibrationOn = false;



    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        soundToggleButton.onClick.AddListener(ToggleSound);
        vibrationToggleButton.onClick.AddListener(ToggleVibration);
        restartButton.onClick.AddListener(RestartLevel);

        CurrencyManager.Instance.OnCoinChanged += UpdateCoin;
        UpdateCoin(CurrencyManager.Instance.Coin);
    }

    #region Currency & Level
    public void UpdateCoin(int amount) => coinText.text = amount.ToString();
    public void UpdateLevel(int level) => levelText.text = "Level " + level;
    #endregion

    #region Timer
    public void UpdateTimer(float progress)
    {
        timerFill.fillAmount = Mathf.Clamp01(progress);
        timerFill.color = progress < 0.1f ? Color.red :
                          progress < 0.3f ? Color.yellow :
                          Color.green;
    }
    #endregion

    #region Panels
    public void HideAllPanels()
    {
        if (levelCompletePanel.activeSelf)
            HidePanelWithAnimation(levelCompleteCG, levelCompleteHeader, levelCompletePanel);

        if (levelFailPanel.activeSelf)
            HidePanelWithAnimation(levelFailCG, levelFailHeader, levelFailPanel);

        if (boosterPanel.activeSelf)
            HidePanelWithAnimation(boosterCG, boosterHeader, boosterPanel);

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

    public void ShowLevelComplete()
    {
        HideAllPanels();
        levelCompletePanel.SetActive(true);

        UIAnimator.FadeIn(levelCompleteCG);
        UIAnimator.ScaleIn(levelCompleteHeader);
        UIAnimator.ScaleIn(levelCompleteCoinIcon, 0.4f, 0.2f);
        UIAnimator.ScaleIn(levelCompleteNextButton, 0.3f, 0.4f);
        UIAnimator.RotateLoop(levelCompleteShine.transform);
    }

    public void ShowLevelFail()
    {
        HideAllPanels();
        levelFailPanel.SetActive(true);

        UIAnimator.FadeIn(levelFailCG);
        UIAnimator.ScaleIn(levelFailHeader);
        UIAnimator.ScaleIn(levelFailPlusTimeImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(levelFailText, -1000, 0.3f,Ease.OutExpo,0.6f);
        UIAnimator.ScaleIn(levelFailButtons, 0.3f, 0.6f);
    }

    public void ShowCoinBuyPanel()
    {
        HideAllPanels();
        coinBuyPanel.SetActive(true);

        UIAnimator.FadeIn(coinBuyCG);
        UIAnimator.ScaleIn(coinBuyHeader);
        UIAnimator.ScaleIn(coinBuyImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(coinBuyText, -1000, 0.3f, Ease.OutExpo, 0.6f);
        UIAnimator.ScaleIn(coinBuyButton, 0.3f, 0.9f);
    }

    public void BuyCoin(int coinAmount)
    {
        CurrencyManager.Instance.AddCoin(coinAmount);
        HideAllPanels();
    }

    public void ShowBoosterPanel()
    {
        HideAllPanels();
        boosterPanel.SetActive(true);

        bool hasEnoughCoin = CurrencyManager.Instance.Coin >= 100;

        unlockBoosterWithCoinButton.gameObject.SetActive(hasEnoughCoin);
        unlockBoosterWithAdsButton.transform.localPosition = hasEnoughCoin
            ? new Vector3(190, unlockBoosterWithAdsButton.transform.localPosition.y, 0) 
            : new Vector3(0, unlockBoosterWithAdsButton.transform.localPosition.y, 0);  

        UIAnimator.FadeIn(boosterCG);
        UIAnimator.ScaleIn(boosterHeader);
        UIAnimator.ScaleIn(boosterIcon, 0.3f, 0.2f);
        UIAnimator.MoveFromX(boosterText, -1000, 0.3f, Ease.OutExpo, 0.4f);
        UIAnimator.ScaleIn(boosterButtons, 0.3f, 0.6f);
    }


    public void ShowBoosterUnlockedPanel()
    {
        HideAllPanels();
        boosterUnlockedPanel.SetActive(true);

        boosterUnlockedCG.alpha = 0;
        boosterUnlockedCG.transform.localScale = Vector3.zero;
        boosterUnlockedIcon.localScale = Vector3.zero;
        boosterTextImage.alpha = 0;
        Vector3 originalTextPos = boosterTextImage.transform.localPosition;
        boosterTextImage.transform.localPosition = new Vector3(originalTextPos.x, 50, originalTextPos.z);

        Sequence seq = DOTween.Sequence();
        seq.Append(boosterUnlockedCG.DOFade(1f, 0.4f));
        seq.Join(boosterUnlockedCG.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));

        UIAnimator.RotateLoop(boosterUnlockedBGGlow.transform, 8f);

        seq.Join(boosterUnlockedIcon.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
        seq.Append(boosterTextImage.DOFade(1f, 0.3f));
        seq.Join(boosterTextImage.transform.DOLocalMoveY(originalTextPos.y, 0.3f).SetEase(Ease.OutExpo));

        seq.AppendInterval(2.5f);
        seq.Append(boosterUnlockedCG.DOFade(0f, 0.3f));
        seq.Join(boosterUnlockedCG.transform.DOScale(0.8f, 0.3f));
        seq.OnComplete(() => boosterUnlockedPanel.SetActive(false));
    }

    public void UnlockBoosterWithCoin()
    {
        if (CurrencyManager.Instance.Coin >= 100)
        {
            CurrencyManager.Instance.SpendCoin(100);
            ShowBoosterUnlockedPanel();
        }
        else
        {
            ShowCoinBuyPanel();
        }
    }

    public void UnlockBoosterWithAd()
    {
        // TODO: If Ad is successful
        ShowBoosterUnlockedPanel();
    }

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
        // TODO: AudioManager.Instance.SetSound(isSoundOn);
        UpdateSoundToggleVisual();
    }

    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        // TODO: VibrationManager.Instance.SetVibration(isVibrationOn);
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

    public void RestartLevel()
    {
        // TODO: Restart Animation
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }


    #endregion
}
