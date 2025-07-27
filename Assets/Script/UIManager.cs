using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region === Serialized Fields ===

    [Header("Top Bar Elements")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image clockIcon;
    [SerializeField] private Sprite normalClockIcon;
    [SerializeField] private Sprite frozenClockIcon;
    [SerializeField] private Button settingsButton;
    private Tween flashTweenTimerText;

    [Header("Booster Shortcut")]
    [SerializeField] private Button boosterButton;
    private Tween boosterButtonTween;

    [Header("UI Panels")]
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject levelFailPanel;
    [SerializeField] private GameObject boosterPanel;
    [SerializeField] private GameObject coinBuyPanel;
    [SerializeField] private GameObject boosterUnlockedPanel;
    [SerializeField] private GameObject playOnPanel;
    [SerializeField] private GameObject settingsPanel;

    #region Level Complete Panel
    [Header("Level Complete Panel Elements")]
    [SerializeField] private CanvasGroup levelCompleteCG;
    [SerializeField] private Transform levelCompleteHeader;
    [SerializeField] private Transform levelCompleteCoinIcon;
    [SerializeField] private Transform levelCompleteNextButton;
    [SerializeField] private Image levelCompleteShine;
    #endregion

    #region Level Fail Panel
    [Header("Level Fail Panel Elements")]
    [SerializeField] private CanvasGroup levelFailCG;
    [SerializeField] private Transform levelFailHeader;
    [SerializeField] private Transform levelFailPlusTimeImage;
    [SerializeField] private Transform levelFailText;
    [SerializeField] private Transform levelFailButtons;
    [SerializeField] private Button levelFailPlayOnButton;
    [SerializeField] private Button levelFailNoConnectionButton;
    #endregion

    #region Booster Panel
    [Header("Booster Panel Elements")]
    [SerializeField] private CanvasGroup boosterCG;
    [SerializeField] private Transform boosterHeader;
    [SerializeField] private Transform boosterIcon;
    [SerializeField] private Transform boosterText;
    [SerializeField] private Transform boosterButtons;
    [SerializeField] private Button unlockBoosterWithCoinButton;
    [SerializeField] private Button unlockBoosterWithAdsButton;
    [SerializeField] private Button unlockBoosterNoAdAvailableButton;
    #endregion

    #region Coin Buy Panel
    [Header("Coin Buy Panel Elements")]
    [SerializeField] private CanvasGroup coinBuyCG;
    [SerializeField] private Transform coinBuyHeader;
    [SerializeField] private Transform coinBuyImage;
    [SerializeField] private Transform coinBuyText;
    [SerializeField] private Transform coinBuyButton;
    [SerializeField] private Button coinBuyButtonNoConnection;
    #endregion

    #region Booster Unlocked Panel
    [Header("Booster Unlocked Panel Elements")]
    [SerializeField] private CanvasGroup boosterUnlockedCG;
    [SerializeField] private Image boosterUnlockedBGGlow;
    [SerializeField] private Transform boosterUnlockedIcon;
    [SerializeField] private CanvasGroup boosterTextImage;
    #endregion

    #region Play On Panel
    [Header("Play On Panel Elements")]
    [SerializeField] private CanvasGroup playOnCG;
    [SerializeField] private Image playOnGlow;
    [SerializeField] private Transform playOnIcon;
    [SerializeField] private CanvasGroup playOnTextImage;
    #endregion

    #region Settings Panel
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

    [Header("Settings Panel - Sprites")]
    [SerializeField] private Sprite[] iconSounds;
    [SerializeField] private Sprite[] iconVibrations;
    [SerializeField] private Sprite[] bgSettingsButtons;
    #endregion

    #endregion

    #region === Internal States ===

    private bool isSoundOn = false;
    private bool isVibrationOn = false;
    #endregion

    #region === Unity Events ===

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

        isSoundOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
        UpdateSoundToggleVisual();
        UpdateVibrationToggleVisual();

        CurrencyManager.Instance.OnCoinChanged += UpdateCoin;
        UpdateCoin(CurrencyManager.Instance.Coin);

        GameTimerManager.Instance.OnTimeChanged += UpdateTimer;
        GameTimerManager.Instance.OnTimeOver += ShowLevelFail;

        StartLevel(); //TODO: Replace with actual level start logic
    }

    #endregion

    #region === Top Bar Updates ===

    public void UpdateCoin(int amount) => coinText.text = amount.ToString();

    public void UpdateLevel(int level) => levelText.text = "Level " + level;

    public void UpdateTimer(float progress)
    {
        timerFill.fillAmount = Mathf.Clamp01(progress);

        if (GameTimerManager.Instance.isFrozen)
        {
            timerFill.color = Color.cyan;
        }
        else
        {
            timerFill.color = progress < 0.2f ? Color.red :
                              progress < 0.4f ? Color.yellow :
                              Color.green;
        }

        float remaining = GameTimerManager.Instance.RemainingTime;
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (remaining <= 10f && flashTweenTimerText == null)
        {
            flashTweenTimerText = timerText.transform.DOScale(1.15f, 1f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
        else if (remaining > 10f && flashTweenTimerText != null)
        {
            flashTweenTimerText.Kill();
            flashTweenTimerText = null;
            timerText.transform.localScale = Vector3.one;
        }
    }

    #endregion

    #region === Panel Management ===

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

    #endregion

    #region === Panel Display Functions ===

    public void ShowLevelComplete()
    {
        if (GameTimerManager.Instance.isFrozen)
            GameTimerManager.Instance.CancelFreeze();

        UpdateTimer(GameTimerManager.Instance.RemainingTime / GameTimerManager.Instance.totalTime); //For reset timer color

        GameTimerManager.Instance.isRunning = false; 
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
        if (flashTweenTimerText != null)
        {
            flashTweenTimerText.Kill();
            flashTweenTimerText = null;
            timerText.transform.localScale = Vector3.one;
        }

        HideAllPanels();
        levelFailPanel.SetActive(true);

        //bool isAdReady = AdManager.Instance.IsRewardedAdReady(); //TODO: Ad ready check!!
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        if (hasInternet) //TODO: Replace with actual ad check isAdReady
        {
            levelFailPlayOnButton.gameObject.SetActive(true);
            levelFailNoConnectionButton.gameObject.SetActive(false);
        }
        else
        {
            levelFailPlayOnButton.gameObject.SetActive(false);
            levelFailNoConnectionButton.gameObject.SetActive(true);
        }

        UIAnimator.FadeIn(levelFailCG);
        UIAnimator.ScaleIn(levelFailHeader);
        UIAnimator.ScaleIn(levelFailPlusTimeImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(levelFailText, -1000, 0.3f, Ease.OutExpo, 0.6f);
        UIAnimator.ScaleIn(levelFailButtons, 0.3f, 0.6f);
    }

    public void ShowCoinBuyPanel()
    {
        HideAllPanels();
        coinBuyPanel.SetActive(true);

        //bool isAdReady = AdManager.Instance.IsRewardedAdReady(); //TODO: Ad ready check!!
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        UIAnimator.FadeIn(coinBuyCG);
        UIAnimator.ScaleIn(coinBuyHeader);
        UIAnimator.ScaleIn(coinBuyImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(coinBuyText, -1000, 0.3f, Ease.OutExpo, 0.6f);

        if (hasInternet) //TODO: isAdReady check
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

    public void ShowBoosterPanel()
    {
        HideAllPanels();
        boosterPanel.SetActive(true);

        bool hasEnoughCoin = CurrencyManager.Instance.Coin >= 100;
        //bool isAdReady = AdManager.Instance.IsRewardedAdReady(); //TODO: Ad ready check!!
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        unlockBoosterWithCoinButton.gameObject.SetActive(hasEnoughCoin);

        if (hasInternet) //TODO: Replace with actual ad check
        {
            unlockBoosterWithAdsButton.gameObject.SetActive(true);
            unlockBoosterNoAdAvailableButton.gameObject.SetActive(false);

            unlockBoosterWithAdsButton.transform.localPosition = hasEnoughCoin
                ? new Vector3(190, unlockBoosterWithAdsButton.transform.localPosition.y, 0)
                : new Vector3(0, unlockBoosterWithAdsButton.transform.localPosition.y, 0);
        }
        else
        {
            unlockBoosterWithAdsButton.gameObject.SetActive(false);
            unlockBoosterNoAdAvailableButton.gameObject.SetActive(true);

            unlockBoosterNoAdAvailableButton.transform.localPosition = hasEnoughCoin
                ? new Vector3(190, unlockBoosterNoAdAvailableButton.transform.localPosition.y, 0)
                : new Vector3(0, unlockBoosterNoAdAvailableButton.transform.localPosition.y, 0);
        }

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
        UIAnimator.WobbleRotation(boosterUnlockedIcon.transform);
        seq.Join(boosterUnlockedIcon.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
        seq.Append(boosterTextImage.DOFade(1f, 0.3f));
        seq.Join(boosterTextImage.transform.DOLocalMoveY(originalTextPos.y, 0.3f).SetEase(Ease.OutExpo));
        seq.AppendInterval(0.5f);
        seq.Append(boosterUnlockedCG.DOFade(0f, 0.3f));
        seq.Join(boosterUnlockedCG.transform.DOScale(0.8f, 0.3f));
        seq.OnComplete(() => boosterUnlockedPanel.SetActive(false));
    }

    public void ShowPlayOnPanel()
    {
        //TODO: Add 30 seconds
        GameTimerManager.Instance.AddTime(30f); //TODO: Crate this method in a Function because we will add more different things to add more time

        HideAllPanels();
        playOnPanel.SetActive(true);

        playOnCG.alpha = 0;
        playOnCG.transform.localScale = Vector3.zero;
        playOnIcon.localScale = Vector3.zero;
        playOnTextImage.alpha = 0;

        Vector3 originalTextPos = playOnTextImage.transform.localPosition;
        playOnTextImage.transform.localPosition = new Vector3(originalTextPos.x, 50, originalTextPos.z);

        Sequence seq = DOTween.Sequence();
        seq.Append(playOnCG.DOFade(1f, 0.4f));
        seq.Join(playOnCG.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
        UIAnimator.RotateLoop(playOnGlow.transform, 8f);
        UIAnimator.WobbleRotation(playOnIcon.transform);
        seq.Join(playOnIcon.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
        seq.Append(playOnTextImage.DOFade(1f, 0.3f));
        seq.Join(playOnTextImage.transform.DOLocalMoveY(originalTextPos.y, 0.3f).SetEase(Ease.OutExpo));
        seq.AppendInterval(0.5f);
        seq.Append(playOnCG.DOFade(0f, 0.3f));
        seq.Join(playOnCG.transform.DOScale(0.8f, 0.3f));
        seq.OnComplete(() => playOnPanel.SetActive(false));
    }
    #endregion

    #region === Booster Actions ===

    public void UnlockBoosterWithCoin()
    {
        if (CurrencyManager.Instance.Coin >= 100)
        {
            CurrencyManager.Instance.SpendCoin(100);
            GameTimerManager.Instance.Freeze(10f);
            ShowBoosterUnlockedPanel();
        }
        else
        {
            ShowCoinBuyPanel();
        }
    }

    public void UnlockBoosterWithAd()
    {
        //TODO: Rewarded Ad Integration
        GameTimerManager.Instance.Freeze(10f);
        ShowBoosterUnlockedPanel();
    }

    public void SetFrozenState(bool frozen)
    {
        if (frozen)
        {
            timerFill.color = Color.cyan;
            clockIcon.sprite = frozenClockIcon;
            HideBoosterButton();
        }
        else
        {
            clockIcon.sprite = normalClockIcon;
            ShowBoosterButton();
        }
    }

    private void HideBoosterButton()
    {
        boosterButtonTween?.Kill();

        boosterButtonTween = boosterButton.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                boosterButton.interactable = false;
                boosterButton.gameObject.SetActive(false);
            });
    }

    private void ShowBoosterButton()
    {
        boosterButton.gameObject.SetActive(true);
        boosterButton.interactable = true;

        boosterButton.transform.localScale = Vector3.zero;

        boosterButtonTween?.Kill();

        boosterButtonTween = boosterButton.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }
    #endregion

    #region === Play On Actions ===
    public void PlayOnRewarded()
    {
        //TODO: Rewarded Ad Integration
        ShowPlayOnPanel();
    }
    #endregion

    #region === Buy Coin Actions ===
    public void BuyCoins(int coinAmount)
    {
        //TODO: Rewarded Ad Integration
        CurrencyManager.Instance.AddCoin(coinAmount);
        HideAllPanels();
    }
    #endregion

    #region === Settings and Toggles ===

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
        // TODO: AudioManager.Instance.SetSound(isSoundOn);
        UpdateSoundToggleVisual();
    }

    public void ToggleVibration()
    {
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
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

    #endregion

    #region === Utilities ===

    public void RestartLevel()
    {
        // TODO: Optional restart animation
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

    #region === Debug Methods ===
    //TODO: Remove or comment out these methods in production
    void StartLevel()
    {
        GameTimerManager.Instance.StartTimer(60f);
    }

    #endregion
}
