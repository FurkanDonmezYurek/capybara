using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    #region === Serialized Fields ===

    [Header("Top Bar Elements")]
    [SerializeField] private RectTransform currencyPanel;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image timerFill;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Image clockIcon;
    [SerializeField] private Sprite normalClockIcon;
    [SerializeField] private Sprite frozenClockIcon;
    [SerializeField] private Button settingsButton;
    private Tween flashTweenTimerText;
    private Tween coinTween;
    private Tween coinCurrencyPanelTween;
    private bool suppressTimerUI = false;
    private Coroutine animatedTimeCoroutine;
    private bool isFinalCountdownSoundPlaying = false;

    [Header("Booster Shortcut")]
    [SerializeField] private Button[] boosterButton;
    private Tween[] boosterButtonTweens;

    [Header("UI Panels")]
    public GameObject levelCompletePanel;
    [SerializeField] public GameObject levelFailPanel;
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

    [Header("Vehicle Unlock Progress")]
    [SerializeField] private GameObject vehicleProgressRoot;
    [SerializeField] private CanvasGroup vehicleProgressCG;
    [SerializeField] private Transform vehicleProgressScaleTarget;
    [SerializeField] private Image vehicleBGImage;
    [SerializeField] private Image vehicleFillImage;
    [SerializeField] private List<Sprite> vehicleBGs;
    [SerializeField] private List<Sprite> vehicleFillSprites;
    [SerializeField] private Image vehicleProgressBarFillImage;
    [SerializeField] private TextMeshProUGUI vehicleProgressText;
    [SerializeField] private int levelsPerVehicle = 5;


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
    [SerializeField] private Transform[] boosterHeader;
    [SerializeField] private Transform[] boosterIcon;
    [SerializeField] private Transform[] boosterText;
    [SerializeField] private Transform boosterButtons;
    [SerializeField] private Button unlockBoosterWithCoinButton;
    [SerializeField] private Button unlockBoosterWithAdsButton;
    [SerializeField] private Button unlockBoosterNoAdAvailableButton;
    #endregion

    #region Seat Booster Process
    [Header("Seat Booster Process Elements")]
    private int seatBoosterCount = 2;
    [SerializeField] private TMP_Text seatBoosterCountText;
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
    [SerializeField] private Transform[] boosterUnlockedIcon;
    [SerializeField] private CanvasGroup[] boosterTextImage;
    #endregion

    #region Booster Frame
    [Header("Booster Frame")]
    [SerializeField] private GameObject boosterFrame;
    [SerializeField] private CanvasGroup boosterFrameCG;
    private Tween boosterFrameTween;
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

    [SerializeField] private Image soundToggleIcon;
    [SerializeField] private Image soundToggleBackground;

    [SerializeField] private Image vibrationToggleIcon;
    [SerializeField] private Image vibrationToggleBackground;

    [Header("Settings Panel - Sprites")]
    [SerializeField] private Sprite[] iconSounds;
    [SerializeField] private Sprite[] iconVibrations;
    [SerializeField] private Sprite[] bgSettingsButtons;

    private bool isSoundOn = false;
    private bool isVibrationOn = false;
    #endregion

    #region Cloud Transition
    [Header("Cloud Transition")]
    [SerializeField] private GameObject cloudTransitionPanel;
    [SerializeField] private RectTransform leftCloud;
    [SerializeField] private RectTransform rightCloud;
    #endregion

    #region Coin Fly Effect
    [Header("Coin Fly Effect")]
    [SerializeField] private GameObject coinFlyPrefab;
    [SerializeField] private Transform coinFlyLastPosition;
    [SerializeField] private Transform coinFlyTarget;
    [SerializeField] private int coinFlyCount = 5;
    [SerializeField] private float coinFlyInterval = 0.075f;
    [SerializeField] private float coinFlyDuration = 1f;
    #endregion

    #region Gameplay Tutorial
    [Header("Gameplay Seat Tutorial")]
    [SerializeField] private GameObject seatTutorialPanel;
    [SerializeField] private CanvasGroup seatTutorialCG;
    [SerializeField] private Image seatTutorialCircle;
    [SerializeField] private List<SeatTutorialStep> seatTutorialSteps;
    [SerializeField] private float seatTutorialMoveDuration = 0.3f;
    [SerializeField] private float pulseScale = 1.2f;

    private int currentSeatTutorialIndex = 0;
    private Tween seatTutorialTween;
    public bool seatTutorialActive = false;
    public bool seatClickedTutorial = false;
    #region Learn Tutorial

    [Header("Learn Tutorial")]
    [SerializeField] private GameObject learnTutorialPanel;
    [SerializeField] private CanvasGroup learnTutorialCG;
    [SerializeField] private Animator tutorialImageAnimator;

    [SerializeField] private RectTransform learnTutorialHeader;
    [SerializeField] private RectTransform learnTutorialContentImage;
    [SerializeField] private RectTransform learnTutorialSkipButton;

    [SerializeField] private List<TutorialData> tutorials;
    [SerializeField] private TMP_Text learnTutorialDescriptionText;
    Dictionary<TutorialType, TutorialData> tutorialDict;
    [SerializeField] private Dictionary<TutorialType, string> tutorialDescriptions = new Dictionary<TutorialType, string>()
        {
            { TutorialType.LearnFreezeCapybara, "Unfreeze a chilly Capybara by matching friends in front or behind it!" },
            { TutorialType.LearnChildCapybara, "Child Capybaras are playful troublemakers moving wherever they please!" },
            { TutorialType.LearnFatCapybara, "Fat Capybaras hog two seats and waddle around slowly." },
            { TutorialType.LearnFreezeBooster, "Stop the clock for a while and keep the fun going!" },
            { TutorialType.LearnSeatBooster, "Pop open extra seats to fit more Capybaras with ease!" }
        };

    #endregion

    #endregion

    #endregion

    #region === Unity Events ===
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        tutorialDict = new Dictionary<TutorialType, TutorialData>();
        foreach (var t in tutorials)
            tutorialDict[t.tutorialType] = t;
    }
    private void Start()
    {
        InitializeSettings();
        PlayCloudOpenTransition();
        SetupListeners();
        UpdateCoin(CurrencyManager.Instance.Coin);
        TryStartTutorial();
    }
    #endregion

    #region === Initialization ===
    private void InitializeSettings()
    {
        LoadSettingsPreferences();

        UpdateToggleVisual(soundToggleIcon, soundToggleBackground, isSoundOn, iconSounds, bgSettingsButtons);
        UpdateToggleVisual(vibrationToggleIcon, vibrationToggleBackground, isVibrationOn, iconVibrations, bgSettingsButtons);

        boosterButtonTweens = new Tween[boosterButton.Length];
    }
    private void SetupListeners()
    {
        CurrencyManager.Instance.OnCoinChanged += UpdateCoin;
        GameTimerManager.Instance.OnTimeChanged += UpdateTimer;
        GameTimerManager.Instance.OnTimeOver += ShowLevelFail;
    }
    private void TryStartTutorial()
    {
        if (!PlayerPrefs.HasKey("HasSeenSeatTutorial"))
        {
            StartTutorial(TutorialType.LearnStartTutorial);
            PlayerPrefs.SetInt("HasSeenSeatTutorial", 1);
        }
        else if (VehicleManager.Instance.GetCurrentLevelIndex()==3)
        {
            StartTutorial(TutorialType.LearnFreezeCapybara);
        }
        else if (VehicleManager.Instance.GetCurrentLevelIndex() == 9)
        {
            StartTutorial(TutorialType.LearnChildCapybara);
        }
        else if (VehicleManager.Instance.GetCurrentLevelIndex() == 12)
        {
            StartTutorial(TutorialType.LearnFatCapybara);
        }


        if (VehicleManager.Instance.GetCurrentLevelIndex() == 5 && !PlayerPrefs.HasKey("HasSeenFreezeTimeTutorial"))
        {
            StartTutorial(TutorialType.LearnFreezeBooster);
            PlayerPrefs.SetInt("HasSeenFreezeTimeTutorial", 1);
            ShowBoosterButton();
        }
        else if (PlayerPrefs.HasKey("HasSeenFreezeTimeTutorial"))
        {
            ShowBoosterButton();
        }
        else
        {
            HideBoosterButton(true);
        }


        if (VehicleManager.Instance.GetCurrentLevelIndex() == 7 && !PlayerPrefs.HasKey("HasSeenSeatBoosterTutorial"))
        {
            StartTutorial(TutorialType.LearnSeatBooster);
            PlayerPrefs.SetInt("HasSeenSeatBoosterTutorial", 1);
            ShowBoosterButton();
        }
        else if (PlayerPrefs.HasKey("HasSeenSeatBoosterTutorial"))
        {
            ShowBoosterButton();
        }
        else
        {
            HideBoosterButton(true);
        }
    }
    #endregion

    #region === Top Bar Updates ===

    public void UpdateCoin(int amount)
    {
        AnimateCoinChange(amount);
    }

    public void UpdateLevel(int level)
    {
        level++;
        levelText.text = "Level " + level;
    }

    public void UpdateTimer(float progress)
    {
        if (suppressTimerUI) return;

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
        if (remaining <= 5f && remaining > 0f)
        {
            if (!isFinalCountdownSoundPlaying)
            {
                isFinalCountdownSoundPlaying = true;
                AudioManager.Instance.PlaySFX("TimerSound");
                if (HapticsManager.Instance != null)
                    HapticsManager.Instance.PlayWarningImpactVibration();
            }
        }
        else
        {
            if (isFinalCountdownSoundPlaying)
            {
                isFinalCountdownSoundPlaying = false;
                AudioManager.Instance.sfxSource.Stop();
            }
        }

    }

    #endregion

    #region === Panel Management ===
    public void HideAllPanels()
    {
        if(HapticsManager.Instance != null)
            HapticsManager.Instance.PlayUIFeedback("UI_Click", HapticsManager.Instance.PlayMediumImpactVibration);


        if (levelCompletePanel.activeSelf)
            HidePanelWithAnimation(levelCompleteCG, levelCompleteHeader, levelCompletePanel);

        if (levelFailPanel.activeSelf)
            HidePanelWithAnimation(levelFailCG, levelFailHeader, levelFailPanel);

        if (boosterPanel.activeSelf)
        {
            for (int i = 0; i < boosterHeader.Length; i++)
            {
                HidePanelWithAnimation(boosterCG, boosterHeader[i], boosterPanel);
            }
        }

        if (coinBuyPanel.activeSelf)
            HidePanelWithAnimation(coinBuyCG, coinBuyHeader, coinBuyPanel);

        if (settingsPanel.activeSelf)
            HidePanelWithAnimation(settingsCG, settingsContent, settingsPanel);
    }
    private void HidePanelWithAnimation(CanvasGroup cg, Transform scaleTarget, GameObject panelGO, float duration = 0.3f, float scale = 0.8f)
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(cg.DOFade(0f, duration));
        if (scaleTarget != null)
            seq.Join(scaleTarget.DOScale(scale, duration).SetEase(Ease.InBack));
        seq.OnComplete(() => panelGO.SetActive(false));
    }
    #endregion

    #region === Panel Display Functions ===

    #region Level Complate
    public void ShowLevelComplete()
    {
        HandleRewardsAndVibrations();
        HandleTimerState();
        PreparePanelUI();

        int currentLevel = GameManager.Instance.levelManager.GetCurrentLevelIndex();
        PlayLevelCompleteSequence(currentLevel + 1);
    }
    private void HandleRewardsAndVibrations()
    {
        BuyCoins(100);

        if (HapticsManager.Instance != null)
            HapticsManager.Instance.PlaySuccessVibration();
    }
    private void HandleTimerState()
    {
        if (GameTimerManager.Instance.isFrozen)
            GameTimerManager.Instance.CancelFreeze();

        UpdateTimer(GameTimerManager.Instance.RemainingTime / GameTimerManager.Instance.totalTime);
        GameTimerManager.Instance.isRunning = false;
    }
    private void PreparePanelUI()
    {
        HideAllPanels();
        levelCompletePanel.SetActive(true);

        vehicleProgressRoot.SetActive(false);
        vehicleProgressCG.alpha = 0f;
        vehicleProgressScaleTarget.localScale = Vector3.one * 0.8f;

        levelCompleteNextButton.localScale = Vector3.zero;

        UIAnimator.FadeIn(levelCompleteCG);
        UIAnimator.ScaleIn(levelCompleteHeader);
        UIAnimator.RotateLoop(levelCompleteShine.transform);
    }
    private void PlayLevelCompleteSequence(int nextLevel)
    {
        DOTween.Sequence()
            .Append(levelCompleteCoinIcon.DOScale(1f, 0.4f).From(0f).SetEase(Ease.OutBack))
            .AppendInterval(0.5f)
            .Append(levelCompleteCoinIcon.DOScale(0f, 0.3f).SetEase(Ease.InBack))
            .AppendCallback(() =>
            {
                vehicleProgressRoot.SetActive(true);
                UpdateVehicleProgress(nextLevel);
            })
            .Append(vehicleProgressCG.DOFade(1f, 0.4f))
            .Join(vehicleProgressScaleTarget.DOScale(1f, 0.4f).SetEase(Ease.OutBack))
            .AppendInterval(0.3f)
            .Append(levelCompleteNextButton.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
    }
    #endregion

    #region Level Fail
    public void ShowLevelFail()
    {
        HapticsManager.Instance.PlayUIFeedback("GameOver", HapticsManager.Instance.PlayFailureImpactVibration);
        ResetFlashingTimerText();

        HideAllPanels();
        levelFailPanel.SetActive(true);

        SetupLevelFailButtons();
        AnimateLevelFailUI();
    }
    private void ResetFlashingTimerText()
    {
        if (flashTweenTimerText != null)
        {
            flashTweenTimerText.Kill();
            flashTweenTimerText = null;
            timerText.transform.localScale = Vector3.one;
        }
    }
    private void SetupLevelFailButtons()
    {
        //bool isAdReady = AdManager.Instance.IsRewardedAdReady(); //TODO: Replace with actual ad check
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        levelFailPlayOnButton.gameObject.SetActive(hasInternet);
        levelFailNoConnectionButton.gameObject.SetActive(!hasInternet);
    }
    private void AnimateLevelFailUI()
    {
        UIAnimator.FadeIn(levelFailCG);
        UIAnimator.ScaleIn(levelFailHeader);
        UIAnimator.ScaleIn(levelFailPlusTimeImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(levelFailText, -1000, 0.3f, Ease.OutExpo, 0.6f);
        UIAnimator.ScaleIn(levelFailButtons, 0.3f, 0.6f);
    }
    #endregion

    #region Coin Buy Panel
    public void ShowCoinBuyPanel()
    {
        HideAllPanels();
        AudioManager.Instance.PlaySFX("OpeningImportantPanel");
        coinBuyPanel.SetActive(true);

        AnimateCoinBuyUI();
        SetupCoinBuyButtons();
    }
    private void AnimateCoinBuyUI()
    {
        UIAnimator.FadeIn(coinBuyCG);
        UIAnimator.ScaleIn(coinBuyHeader);
        UIAnimator.ScaleIn(coinBuyImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(coinBuyText, -1000, 0.3f, Ease.OutExpo, 0.6f);
    }
    private void SetupCoinBuyButtons()
    {
        //bool isAdReady = AdManager.Instance.IsRewardedAdReady(); //TODO
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

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
    #endregion

    #region Booster Process

    #region Booster Panel
    public void ShowBoosterPanel(bool isFreezeBooster)
    {
        MoveToCurrentSeatStep();
        HideAllPanels();
        AudioManager.Instance.PlaySFX("OpeningImportantPanel");

        boosterPanel.SetActive(true);

        SetupBoosterButtons();
        AnimateBoosterPanel(isFreezeBooster);
    }
    private void SetupBoosterButtons()
    {
        bool hasEnoughCoin = CurrencyManager.Instance.Coin >= 100;
        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        unlockBoosterWithCoinButton.gameObject.SetActive(hasEnoughCoin);

        if (hasInternet)
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
    }
    private void AnimateBoosterPanel(bool isFreezeBooster)
    {
        UIAnimator.FadeIn(boosterCG);

        int activeIndex = isFreezeBooster ? 0 : 1;
        int inactiveIndex = isFreezeBooster ? 1 : 0;

        boosterHeader[activeIndex].gameObject.SetActive(true);
        boosterIcon[activeIndex].gameObject.SetActive(true);
        boosterText[activeIndex].gameObject.SetActive(true);

        boosterHeader[inactiveIndex].gameObject.SetActive(false);
        boosterIcon[inactiveIndex].gameObject.SetActive(false);
        boosterText[inactiveIndex].gameObject.SetActive(false);

        UIAnimator.ScaleIn(boosterHeader[activeIndex]);
        UIAnimator.ScaleIn(boosterIcon[activeIndex], 0.3f, 0.2f);
        UIAnimator.MoveFromX(boosterText[activeIndex], -1000, 0.3f, Ease.OutExpo, 0.4f);

        UIAnimator.ScaleIn(boosterButtons, 0.3f, 0.6f);
    }
    #endregion

    #region Unlocked Booster Panel
    public void ShowBoosterUnlockedPanel(bool isFreezeBooster)
    {
        HideAllPanels();
        boosterUnlockedPanel.SetActive(true);

        boosterUnlockedCG.alpha = 0;
        boosterUnlockedCG.transform.localScale = Vector3.zero;
        if (isFreezeBooster)
        {
            ShowBoosterFrame();

            AudioManager.Instance.PlaySFX("FreezeBooster");
            if (HapticsManager.Instance != null)
                HapticsManager.Instance.PlayRigidImpactVibration();

            boosterUnlockedIcon[0].gameObject.SetActive(true);
            boosterUnlockedIcon[1].gameObject.SetActive(false);

            boosterTextImage[0].gameObject.SetActive(true);
            boosterTextImage[1].gameObject.SetActive(false);

            boosterUnlockedIcon[0].localScale = Vector3.zero;
            boosterTextImage[0].alpha = 0;

            Vector3 originalTextPos = boosterTextImage[0].transform.localPosition;
            boosterTextImage[0].transform.localPosition = new Vector3(originalTextPos.x, 50, originalTextPos.z);


            Sequence seq = DOTween.Sequence();
            seq.Append(boosterUnlockedCG.DOFade(1f, 0.4f));
            seq.Join(boosterUnlockedCG.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            UIAnimator.RotateLoop(boosterUnlockedBGGlow.transform, 8f);
            UIAnimator.WobbleRotation(boosterUnlockedIcon[0].transform);
            seq.Join(boosterUnlockedIcon[0].DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            seq.Append(boosterTextImage[0].DOFade(1f, 0.3f));
            seq.Join(boosterTextImage[0].transform.DOLocalMoveY(originalTextPos.y, 0.3f).SetEase(Ease.OutExpo));
            seq.AppendInterval(0.5f);
            seq.Append(boosterUnlockedCG.DOFade(0f, 0.3f));
            seq.Join(boosterUnlockedCG.transform.DOScale(0.8f, 0.3f));
            seq.OnComplete(() => boosterUnlockedPanel.SetActive(false));
        }
        else
        {
            AudioManager.Instance.PlaySFX("SeatBooster");
            if (HapticsManager.Instance != null)
                HapticsManager.Instance.PlayRigidImpactVibration();
            boosterUnlockedIcon[0].gameObject.SetActive(false);
            boosterUnlockedIcon[1].gameObject.SetActive(true);

            boosterTextImage[0].gameObject.SetActive(false);
            boosterTextImage[1].gameObject.SetActive(true);

            boosterUnlockedIcon[1].localScale = Vector3.zero;
            boosterTextImage[1].alpha = 0;

            Vector3 originalTextPos = boosterTextImage[1].transform.localPosition;
            boosterTextImage[1].transform.localPosition = new Vector3(originalTextPos.x, 50, originalTextPos.z);


            Sequence seq = DOTween.Sequence();
            seq.Append(boosterUnlockedCG.DOFade(1f, 0.4f));
            seq.Join(boosterUnlockedCG.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            UIAnimator.RotateLoop(boosterUnlockedBGGlow.transform, 8f);
            UIAnimator.WobbleRotation(boosterUnlockedIcon[1].transform);
            seq.Join(boosterUnlockedIcon[1].DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            seq.Append(boosterTextImage[1].DOFade(1f, 0.3f));
            seq.Join(boosterTextImage[1].transform.DOLocalMoveY(originalTextPos.y, 0.3f).SetEase(Ease.OutExpo));
            seq.AppendInterval(0.5f);
            seq.Append(boosterUnlockedCG.DOFade(0f, 0.3f));
            seq.Join(boosterUnlockedCG.transform.DOScale(0.8f, 0.3f));
            seq.OnComplete(() => boosterUnlockedPanel.SetActive(false));
        }

    }
    #endregion

    #region Unlocked Play On Panel
    public void ShowPlayOnPanel()
    {
        HideAllPanels();
        HapticsManager.Instance.PlayUIFeedback("OpeningImportantPanel", HapticsManager.Instance.PlayRigidImpactVibration);

        playOnPanel.SetActive(true);

        PreparePlayOnPanel();
        PlayPlayOnPanelAnimation();
    }
    private void PreparePlayOnPanel()
    {
        playOnPanel.SetActive(true);

        playOnCG.alpha = 0;
        playOnCG.transform.localScale = Vector3.zero;
        playOnIcon.localScale = Vector3.zero;
        playOnTextImage.alpha = 0;
    }
    private void PlayPlayOnPanelAnimation()
    {
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
    #endregion

    #region Cloud Transition
    public void PlayCloudOpenTransition()
    {
        if(HapticsManager.Instance!=null)
            HapticsManager.Instance.PlayUIFeedback("CloudEffect", HapticsManager.Instance.PlaySoftImpactVibration);

        cloudTransitionPanel.SetActive(true);

        Vector2 leftStartPos = leftCloud.anchoredPosition;
        Vector2 rightStartPos = rightCloud.anchoredPosition;
        leftCloud.anchoredPosition = Vector2.zero;
        rightCloud.anchoredPosition = Vector2.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(leftCloud.DOAnchorPos(leftStartPos, 0.8f).SetEase(Ease.OutQuad));
        seq.Join(rightCloud.DOAnchorPos(rightStartPos, 0.8f).SetEase(Ease.OutQuad));
        seq.AppendCallback(() =>
        {
            cloudTransitionPanel.SetActive(false);
        });
    }
    public void PlayCloudCloseTransition(int sceneIndex)
    {
        if (HapticsManager.Instance != null)
            HapticsManager.Instance.PlayUIFeedback("CloudEffect", HapticsManager.Instance.PlaySoftImpactVibration);

        cloudTransitionPanel.SetActive(true);

        Vector2 leftStartPos = leftCloud.anchoredPosition;
        Vector2 rightStartPos = rightCloud.anchoredPosition;
        Vector2 closedPos = Vector2.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(leftCloud.DOAnchorPos(closedPos, 0.8f).SetEase(Ease.InOutQuad));
        seq.Join(rightCloud.DOAnchorPos(closedPos, 0.8f).SetEase(Ease.InOutQuad));
        seq.AppendCallback(() =>
        {
            SceneManager.LoadScene(sceneIndex);
        });
    }
    #endregion
    #endregion

    #region === Booster Actions ===

    public void UnlockBoosterWithCoin()
    {
        if (CurrencyManager.Instance.Coin >= 100)
        {
            CurrencyManager.Instance.SpendCoin(100);
            if (boosterHeader[0].gameObject.activeSelf)
            {
                GameTimerManager.Instance.Freeze(10f);
                ShowBoosterUnlockedPanel(true);
            }
            else
            {
                GameManager.Instance.gridSystem.AddSeatGroup();
                if (seatBoosterCount > 0)
                {
                    seatBoosterCount--;
                    UpdateSeatBoosterUI();
                    ShowBoosterUnlockedPanel(false);
                }
            }
        }
        else
        {
            ShowCoinBuyPanel();
        }
    }
    public void UnlockBoosterWithAd()
    {
        //TODO: Rewarded Ad Integration
        if (boosterHeader[0].gameObject.activeSelf)
        {
            GameTimerManager.Instance.Freeze(10f);
            ShowBoosterUnlockedPanel(true);
        }
        else
        {
            GameManager.Instance.gridSystem.AddSeatGroup();
            if (seatBoosterCount > 0)
            {
                seatBoosterCount--;
                UpdateSeatBoosterUI();
                ShowBoosterUnlockedPanel(false);
            }
        }
    }
    public void SetFrozenState(bool frozen)
    {
        if (frozen)
        {
            timerFill.color = Color.cyan;
            clockIcon.sprite = frozenClockIcon;
            HideBoosterButton(false);
        }
        else
        {
            clockIcon.sprite = normalClockIcon;
            ShowBoosterButton();
        }
    }
    private void HideBoosterButton(bool isQuickClose=false)
    {
        if (isQuickClose)
        {
            if (PlayerPrefs.HasKey("HasSeenFreezeTimeTutorial") && !PlayerPrefs.HasKey("HasSeenSeatBoosterTutorial"))
            {
                boosterButtonTweens[1]?.Kill();

                boosterButton[1].interactable = false;
                boosterButton[1].gameObject.SetActive(false);
            }
            else if (!PlayerPrefs.HasKey("HasSeenFreezeTimeTutorial") && PlayerPrefs.HasKey("HasSeenSeatBoosterTutorial"))
            {
                boosterButtonTweens[0]?.Kill();

                boosterButton[0].interactable = false;
                boosterButton[0].gameObject.SetActive(false);
            }
            else
            {
                for (int i = 0; i < boosterButton.Length; i++)
                {
                    boosterButtonTweens[i]?.Kill();

                    boosterButton[i].interactable = false;
                    boosterButton[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < boosterButton.Length; i++)
            {
                boosterButtonTweens[i]?.Kill();

                boosterButtonTweens[i] = boosterButton[i].transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
                    .OnComplete(() =>
                    {
                        boosterButton[i].interactable = false;
                        boosterButton[i].gameObject.SetActive(false);
                    });
            }
        }
    }
    private void ShowBoosterButton()
    {
        if (PlayerPrefs.HasKey("HasSeenFreezeTimeTutorial") && PlayerPrefs.HasKey("HasSeenSeatBoosterTutorial"))
        {
            for (int i = 0; i < boosterButton.Length; i++)
            {
                boosterButton[i].gameObject.SetActive(true);
                boosterButton[i].interactable = true;
                boosterButton[i].transform.localScale = Vector3.zero;

                boosterButtonTweens[i]?.Kill();

                boosterButtonTweens[i] = boosterButton[i].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
            }
        }
        else if(PlayerPrefs.HasKey("HasSeenFreezeTimeTutorial") && !PlayerPrefs.HasKey("HasSeenSeatBoosterTutorial"))
        {
            boosterButton[0].gameObject.SetActive(true);
            boosterButton[0].interactable = true;
            boosterButton[0].transform.localScale = Vector3.zero;

            boosterButtonTweens[0]?.Kill();

            boosterButtonTweens[0] = boosterButton[0].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        }
        else if (!PlayerPrefs.HasKey("HasSeenFreezeTimeTutorial") && PlayerPrefs.HasKey("HasSeenSeatBoosterTutorial"))
        {
            boosterButton[1].gameObject.SetActive(true);
            boosterButton[1].interactable = true;
            boosterButton[1].transform.localScale = Vector3.zero;

            boosterButtonTweens[1]?.Kill();

            boosterButtonTweens[1] = boosterButton[1].transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        }
        else
        {
            HideBoosterButton(true);
        }
    }
    public void ShowBoosterFrame()
    {
        boosterFrameTween?.Kill(true);

        boosterFrameCG.gameObject.SetActive(true);
        boosterFrameCG.alpha = 0f;
        boosterFrameCG.transform.localScale = Vector3.one * 1.2f;

        boosterFrameTween = DOTween.Sequence()
            .Append(boosterFrameCG.DOFade(1f, 1f))
            .Join(boosterFrameCG.transform.DOScale(1f, 1f).SetEase(Ease.OutBack));
    }
    public void HideBoosterFrame()
    {
        boosterFrameTween?.Kill(true);

        boosterFrameTween = DOTween.Sequence()
            .Append(boosterFrameCG.DOFade(0f, 1.5f))
            .Join(boosterFrameCG.transform.DOScale(1.2f, 1.5f).SetEase(Ease.InBack))
            .OnComplete(() => boosterFrameCG.gameObject.SetActive(false));
    }
    private void UpdateSeatBoosterUI()
    {
        seatBoosterCountText.text = seatBoosterCount.ToString();

        if (seatBoosterCount <= 0)
        {
            boosterButtonTweens[1]?.Kill();
            boosterButtonTweens[1] = boosterButton[1].transform.DOScale(0f, 0.3f).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    boosterButton[1].interactable = false;
                    boosterButton[1].gameObject.SetActive(false);
                });
        }
    }
    #endregion

    #region === Vehicle Progress ===
    private void UpdateVehicleProgress(int currentLevel)
    {
        int index = (currentLevel - 1) / levelsPerVehicle;
        int progressInSet = (currentLevel - 1) % levelsPerVehicle;

        if (index >= vehicleBGs.Count || index >= vehicleFillSprites.Count)
            return;

        vehicleBGImage.sprite = vehicleBGs[index];
        vehicleFillImage.sprite = vehicleFillSprites[index];

        float vehicleFillAmount = 1f - (progressInSet + 1) / (float)levelsPerVehicle;
        vehicleFillImage.fillAmount = 1f;
        vehicleFillImage.DOFillAmount(vehicleFillAmount, 0.6f).SetEase(Ease.OutCubic);

        float progressFillAmount = (progressInSet + 1) / (float)levelsPerVehicle;
        vehicleProgressBarFillImage.fillAmount = 0f;
        vehicleProgressBarFillImage.DOFillAmount(progressFillAmount, 0.6f).SetEase(Ease.OutCubic);

        vehicleProgressText.text = $"{progressInSet + 1} / {levelsPerVehicle}";

        if (progressInSet + 1 == levelsPerVehicle)
        {
            AudioManager.Instance.PlaySFX("VehicleComplate");
            if (HapticsManager.Instance != null)
                HapticsManager.Instance.PlaySelectionImpactVibration();
        }

    }
    #endregion

    #region === Play On Actions ===
    public void PlayOnRewarded()
    {
        //TODO: Rewarded Ad Integration

        AnimateTimeAddition(30f);
        ShowPlayOnPanel();
    }

    public void AnimateTimeAddition(float timeToAdd, float duration = 1f)
    {
        if (animatedTimeCoroutine != null)
            StopCoroutine(animatedTimeCoroutine);

        animatedTimeCoroutine = StartCoroutine(AnimateTimeIncreaseCoroutine(timeToAdd, duration));
    }

    private IEnumerator AnimateTimeIncreaseCoroutine(float timeToAdd, float animationDuration)
    {
        float startTime = GameTimerManager.Instance.RemainingTime;
        float targetTime = Mathf.Min(startTime + timeToAdd, GameTimerManager.Instance.totalTime);
        GameTimerManager.Instance.AddTime(timeToAdd, true);

        suppressTimerUI = true;

        timerText.text = "+30s";
        timerText.transform.localScale = Vector3.one * 1.3f;
        timerText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        clockIcon.transform.DOPunchScale(Vector3.one * 0.3f, 0.3f, 4, 0.5f);

        Color originalColor = timerFill.color;
        timerFill.color = Color.cyan;

        yield return new WaitForSeconds(0.5f);

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float currentVisualTime = Mathf.Lerp(startTime, targetTime, t);
            UpdateTimerVisualOnly(currentVisualTime);
            yield return null;
        }

        timerFill.color = originalColor;

        suppressTimerUI = false;
        UpdateTimer(GameTimerManager.Instance.RemainingTime / GameTimerManager.Instance.totalTime);

        animatedTimeCoroutine = null;
    }

    private void UpdateTimerVisualOnly(float displayTime)
    {
        float progress = displayTime / GameTimerManager.Instance.totalTime;
        timerFill.fillAmount = Mathf.Clamp01(progress);

        float remaining = displayTime;
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    #endregion

    #region == Coin Process ===

    #region Buy Coin Actions
    public void BuyCoins(int coinAmount)
    {
        //TODO: Rewarded Ad Integration

        CurrencyManager.Instance.AddCoin(coinAmount);

        AudioManager.Instance.PlaySFX("AddCoin");

        PlayCoinFlyEffect(new Vector3(2.5f, -4, 0));

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

    #region Coin Fly Effect Process
    public void PlayCoinFlyEffect(Vector3 worldStartPos)
    {
        StartCoroutine(SpawnCoinFlyRoutine(worldStartPos));
    }

    private IEnumerator SpawnCoinFlyRoutine(Vector3 worldStartPos)
    {
        Vector3 screenStartPos = Camera.main.WorldToScreenPoint(worldStartPos);
        Vector3 uiStartPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            coinText.transform.parent as RectTransform,
            screenStartPos,
            null,
            out Vector2 localPoint
        );
        uiStartPos = localPoint;

        for (int i = 0; i < coinFlyCount; i++)
        {
            GameObject coin = Instantiate(coinFlyPrefab, coinFlyLastPosition);
            if (HapticsManager.Instance != null)
                HapticsManager.Instance.PlayRigidImpactVibration();
            RectTransform coinRT = coin.GetComponent<RectTransform>();
            coinRT.anchoredPosition = uiStartPos;
            coinRT.localScale = Vector3.one;

            Vector2 randomOffset = Random.insideUnitCircle * 30f;

            Sequence seq = DOTween.Sequence();
            seq.Append(coinRT.DOAnchorPos(((Vector2)coinFlyTarget.localPosition) + randomOffset, coinFlyDuration * 0.5f)
                .SetEase(Ease.OutQuad));
            seq.AppendCallback(() =>
            {
                coinRT.anchoredPosition = coinFlyTarget.localPosition;
                Destroy(coin);
                PlayCoinCurrencyBounce();
            });
            seq.Join(coinRT.DOScale(0.3f, coinFlyDuration));

            yield return new WaitForSeconds(coinFlyInterval);
        }
    }
    private void PlayCoinCurrencyBounce()
    {
        if (coinCurrencyPanelTween != null && coinCurrencyPanelTween.IsActive())
            coinCurrencyPanelTween.Kill();

        currencyPanel.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        coinCurrencyPanelTween = currencyPanel.DOScale(0.9f, 0.1f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                currencyPanel.DOScale(0.8f, 0.2f).SetEase(Ease.InQuad);
            });
    }

    #endregion

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
    }
    public void ToggleSound()
    {
        AudioManager.Instance.PlaySFX("UI_Click");
        if (HapticsManager.Instance != null)
            HapticsManager.Instance.PlaySelectionImpactVibration();
        isSoundOn = !isSoundOn;

        if (isSoundOn)
        {
            AudioManager.Instance.MuteMusic(false);
            AudioManager.Instance.MuteSFX(false);
        }
        else
        {
            AudioManager.Instance.MuteMusic(true);
            AudioManager.Instance.MuteSFX(true);
        }

        PlayerPrefs.SetInt("Sound", isSoundOn ? 1 : 0);
        // TODO: AudioManager.Instance.SetSound(isSoundOn);
        UpdateToggleVisual(soundToggleIcon, soundToggleBackground, isSoundOn, iconSounds, bgSettingsButtons);
    }
    public void ToggleVibration()
    {
        AudioManager.Instance.PlaySFX("UI_Click");
        if (HapticsManager.Instance != null)
            HapticsManager.Instance.PlaySelectionImpactVibration();
        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
        // TODO: VibrationManager.Instance.SetVibration(isVibrationOn);
        UpdateToggleVisual(vibrationToggleIcon, vibrationToggleBackground, isVibrationOn, iconVibrations, bgSettingsButtons);
    }
    private void LoadSettingsPreferences()
    {
        isSoundOn = PlayerPrefs.GetInt("Sound", 1) == 1;
        isVibrationOn = PlayerPrefs.GetInt("Vibration", 1) == 1;
    }
    private void UpdateToggleVisual(Image icon, Image background, bool state, Sprite[] icons, Sprite[] backgrounds)
    {
        icon.sprite = state ? icons[1] : icons[0];
        background.sprite = state ? backgrounds[1] : backgrounds[0];
    }
    #endregion

    #region === Gameplay Tutorial ===
    public void StartTutorial(TutorialType type)
    {
        if (!tutorialDict.ContainsKey(type))
        {
            Debug.LogError($"Tutorial not found: {type}");
            return;
        }
        else
        {
            StartCoroutine(StartTutorialDelayed(type));
        }
    }

    public void EndTutorial(TutorialType type)
    {
        GameTimerManager.Instance.isRunning = true;
        if (type == TutorialType.LearnStartTutorial)
        {
            EndSeatTutorial();
        }
        else
        {
            EndLearnTutorial();
        }
    }
    private IEnumerator StartTutorialDelayed(TutorialType type)
    {
        yield return null; 

        if (type == TutorialType.LearnStartTutorial)
        {
            StartSeatTutorial();
        }
        else
        {
            StartLearnTutorial(type);
        }
    }
    private void StartLearnTutorial(TutorialType type)
    {
        GameTimerManager.Instance.isRunning = false;
        learnTutorialPanel.SetActive(true);

        learnTutorialCG.alpha = 0f;
        learnTutorialPanel.transform.localScale = Vector3.one * 0.8f;

        learnTutorialHeader.localScale = Vector3.one * 0.8f;

        learnTutorialContentImage.localScale = Vector3.one * 0.8f;

        learnTutorialSkipButton.localScale = Vector3.one * 0.8f;

        if (tutorialDescriptions.ContainsKey(type))
            learnTutorialDescriptionText.text = tutorialDescriptions[type];
        else
            learnTutorialDescriptionText.text = "";

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.5f);
        seq.Append(learnTutorialCG.DOFade(1f, 0.5f));
        seq.Join(learnTutorialPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        seq.Join(learnTutorialHeader.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        seq.Join(learnTutorialContentImage.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        seq.Join(learnTutorialSkipButton.DOScale(1f, 0.5f).SetEase(Ease.OutBack));

        tutorialImageAnimator.runtimeAnimatorController = tutorialDict[type].animatorController;
        tutorialImageAnimator.Play("TutorialAnim", 0, 0);
    }

    public void EndLearnTutorial()
    {
        GameTimerManager.Instance.isRunning = true;

        Sequence seq = DOTween.Sequence();

        seq.Append(learnTutorialCG.DOFade(0f, 0.3f));
        seq.Join(learnTutorialPanel.transform.DOScale(0.8f, 0.3f).SetEase(Ease.InBack));

        seq.Join(learnTutorialHeader.DOScale(0.8f, 0.3f).SetEase(Ease.InBack));

        seq.Join(learnTutorialContentImage.DOScale(0.8f, 0.3f).SetEase(Ease.InBack));

        seq.Join(learnTutorialSkipButton.DOScale(0.8f, 0.3f).SetEase(Ease.InBack));

        seq.OnComplete(() => learnTutorialPanel.SetActive(false));
    }

    public void StartSeatTutorial()
    {
        if (seatTutorialSteps == null || seatTutorialSteps.Count == 0) return;

        GameTimerManager.Instance.isRunning = false;
        seatTutorialActive = true;
        currentSeatTutorialIndex = 0;
        seatTutorialPanel.SetActive(true);
        seatTutorialCG.alpha = 0f;

        seatTutorialCG.DOFade(1f, 1.5f).SetEase(Ease.OutQuad);

        MoveToCurrentSeatStep();
    }
    private Seat FindSeatByGridPosition(Vector2Int gridPos)
    {
        Seat[] allSeats = FindObjectsOfType<Seat>();
        foreach (var seat in allSeats)
        {
            if (seat.gridPosition == gridPos)
                return seat;
        }
        return null;
    }
    public void MoveToCurrentSeatStep()
    {
        if (currentSeatTutorialIndex >= seatTutorialSteps.Count)
        {
            EndTutorial(TutorialType.LearnStartTutorial);
            return;
        }

        SeatTutorialStep step = seatTutorialSteps[currentSeatTutorialIndex++];
        Seat seat = FindSeatByGridPosition(step.targetGridPos);

        if (seat == null)
        {
            return;
        }

        Vector3 worldPos = seat.transform.position + step.offset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        seatTutorialTween?.Kill();

        if (currentSeatTutorialIndex == 1) 
        {
            seatTutorialCircle.transform.position = screenPos; 
        }
        else
        {
            seatTutorialCircle.transform.DOMove(screenPos, 0.75f).SetEase(Ease.OutCubic);
        }

        seatTutorialCircle.transform.localScale = Vector3.one;

        seatTutorialTween = DOTween.Sequence()
            .Append(seatTutorialCircle.transform.DOScale(pulseScale, 0.6f).SetEase(Ease.OutQuad))
            .Append(seatTutorialCircle.transform.DOScale(1f, 0.6f).SetEase(Ease.InQuad))
            .SetLoops(-1);
    }
    private void EndSeatTutorial()
    {
        GameTimerManager.Instance.isRunning = true;
        seatTutorialActive = false;
        seatTutorialTween?.Kill();
        seatTutorialCG.DOFade(0f, 0.75f).OnComplete(() =>
        {
            seatTutorialPanel.SetActive(false);
        });
    }
    public SeatTutorialStep GetCurrentSeatTutorialStep()
    {
        if (seatTutorialActive && currentSeatTutorialIndex < seatTutorialSteps.Count)
            return seatTutorialSteps[currentSeatTutorialIndex];
        return null;
    }

    #endregion

    #region === Utilities ===
    public void RestartLevel()
    {
        PlayCloudCloseTransition(SceneManager.GetActiveScene().buildIndex);
    }
    public void NextLevel()
    {
        int LevelIndex = PlayerPrefs.GetInt("Level", 0);
        LevelIndex++;
        PlayerPrefs.SetInt("Level", LevelIndex);
        PlayCloudCloseTransition(0);
    }
    public void ReturnIdleScene()
    {
        PlayCloudCloseTransition(0);
    }

    //public void OpenShopFromCoin()
    //{
    //    OpenShopPanel();
    //}

    //// Gem Artı Butonuna Tıklanıldığında Magaza Sayfasına Yönlendirme
    //public void OpenShopFromGem()
    //{
    //    OpenShopPanel();
    //}

    //// Mağaza Panelini Açma
    //private void OpenShopPanel()
    //{
    //    shopPanel.SetActive(true); // Eğer mağaza bir panelse bunu aktif edebilirsiniz
    //    // Ya da, mağaza sayfasına sahne geçişi yapılabilir:
    //    // SceneManager.LoadScene("ShopScene"); // Eğer mağaza sahnesi varsa
    //}

    #endregion
}

[System.Serializable]
public class SeatTutorialStep
{
    public Vector2Int targetGridPos;
    public Vector3 offset;
}
public enum TutorialType
{
    LearnStartTutorial,
    LearnFreezeBooster,
    LearnSeatBooster,
    LearnFreezeCapybara,
    LearnChildCapybara,
    LearnFatCapybara
}


