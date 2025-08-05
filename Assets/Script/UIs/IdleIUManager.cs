using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IdleUIManager : MonoBehaviour
{
    public static IdleUIManager Instance;

    #region === Serialized Fields ===
    [SerializeField] private List<UIPanelData> uiPanelsData;

    #region Start Level Panel
    [Header("Start Level Panel")]
    [SerializeField] private GameObject startLevelPanel;
    [SerializeField] private CanvasGroup startLevelCG;
    [SerializeField] private Transform header;
    [SerializeField] private Transform levelImage;
    [SerializeField] private TextMeshProUGUI selectedLevelText;
    [SerializeField] private Button playButton;
    [SerializeField] private Transform playButtonTransform;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image regionLoopIconImage;
    [SerializeField] private RectTransform regionLoopIconTransform;

    private Tween loopTween;
    private int selectedLevelIndex;
    #endregion

    #region Coin Processing
    [Header("Coin Buy Panel")]
    [SerializeField] private RectTransform currencyPanel;
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject coinBuyPanel;
    [SerializeField] private CanvasGroup coinBuyCG;
    [SerializeField] private Transform coinBuyHeader;
    [SerializeField] private Transform coinBuyImage;
    [SerializeField] private Transform coinBuyText;
    [SerializeField] private Transform coinBuyButton;
    [SerializeField] private Button coinBuyButtonNoConnection;
    private Tween coinCurrencyPanelTween;

    [Header("Coin Fly Effect")]
    [SerializeField] private GameObject coinFlyPrefab;
    [SerializeField] private Transform coinFlyLastPosition;
    [SerializeField] private Transform coinFlyTarget;
    [SerializeField] private int coinFlyCount = 5;
    [SerializeField] private float coinFlyInterval = 0.075f;
    [SerializeField] private float coinFlyDuration = 1f;
    [SerializeField] private float coinFlyOffset = 30f;

    private Tween coinTween;
    #endregion

    #region Settings Panel
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

    private bool isSoundOn;
    public bool isVibrationOn;
    #endregion

    #region Region Elements
    [Header("Region Progress Bar")]
    [SerializeField] private Image regionProgressFill;
    [SerializeField] private TextMeshProUGUI regionProgressText;

    [Header("Region Header Progress")]
    [SerializeField] private TextMeshProUGUI regionNameText;
    [SerializeField] private string[] regionNames;
    [SerializeField] private CanvasGroup centerRegionCG;
    [SerializeField] private Image regionIconImage;
    [SerializeField] private List<Sprite> regionSprites;
    [SerializeField] private float centerRegionNameY = -1250f;
    [SerializeField] private float regionIconMoveX = -200f;
    [SerializeField] private float regionFadeDuration = 2f;
    [SerializeField] private float regionDelayBeforeFade = 0.75f;
    private const float REGION_NAME_FONT_SCALE = 2f;
    #endregion

    #region Shop Panel
    [Header("Shop Panel")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private CanvasGroup shopCG;
    [SerializeField] private Transform shopContent;
    #endregion

    #region Cloud Transition
    [Header("Cloud Transition")]
    [SerializeField] private GameObject cloudTransitionPanel;
    [SerializeField] private RectTransform leftCloud;
    [SerializeField] private RectTransform rightCloud;
    #endregion

    #region Tutorial
    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private CanvasGroup tutorialCG;
    [SerializeField] private RectTransform tutorialHighlightCircle;
    [SerializeField] private float tutorialDelayBeforeAutoClose = 1.5f;
    [SerializeField] private Vector3 tutorialCircleOffset = new(0, 50, 0);
    [SerializeField] private Image fingerImage;
    [SerializeField] private float tutorialVerticalOffset = -5f;
    private const float tutorialFadeDuration = 2.5f;
    private const float tutorialPulseScaleUp = 1.2f;
    private const float tutorialPulseScaleDown = 1.0f;
    private const float tutorialPulseFadeIn = 0.9f;
    private const float tutorialPulseFadeOut = 0.6f;

    private Tween tutorialPulseTween;
    private bool tutorialActive = false;
    #endregion

    #endregion

    #region === Unity Lifecycle ===
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    private void Start()
    {
        SetupButtonListeners();
        LoadSettingsPreferences();
        BindEvents();
        InitializeUI();
        HandleTransitions();
    }
    #endregion

    #region === Initialization ===
    private void SetupButtonListeners()
    {
        playButton.onClick.AddListener(OnPlayButtonClicked);
        closeButton.onClick.AddListener(HideAllPanels);

        soundToggleButton.onClick.AddListener(ToggleSound);
        vibrationToggleButton.onClick.AddListener(ToggleVibration);
    }
    private void BindEvents()
    {
        CurrencyManager.Instance.OnCoinChanged += AnimateCoinChange;
    }
    private void InitializeUI()
    {
        AnimateCoinChange(CurrencyManager.Instance.Coin);
        UpdateRegionProgressBar(VehicleManager.Instance.GetCurrentLevelIndex());

        UpdateToggleVisual(soundToggleIcon, soundToggleBackground, isSoundOn, iconSounds, bgSettingsButtons);
        UpdateToggleVisual(vibrationToggleIcon, vibrationToggleBackground, isVibrationOn, iconVibrations, bgSettingsButtons);
    }
    private void HandleTransitions()
    {
        PlayCloudTransition(CloudTransitionType.Open);

        if (!PlayerPrefs.HasKey("HasSeenIdleTutorial"))
            ShowTutorial();
    }
    #endregion

    #region === Panel On/Off ===
    public void HideAllPanels()
    {
        HapticsManager.Instance.PlayUIFeedback("UI_Click", HapticsManager.Instance.PlayMediumImpactVibration);

        foreach (var panelData in uiPanelsData)
        {
            if (panelData.panelGO.activeSelf)
                HidePanelWithAnimation(panelData.canvasGroup, panelData.scaleTarget, panelData.panelGO);
        }
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

    #region === Start Level ===
    public void OpenStartLevelPanel()
    {
        PrepareTutorialAndUI();
        SetupStartLevelPanel();
        AnimateStartLevelPanel();
    }

    private void PrepareTutorialAndUI()
    {
        SetTutorialAsSeen();
        HideTutorialPanel();
        HideAllPanels();
        AudioManager.Instance?.PlaySFX("OpeningImportantPanel");
    }
    private void SetupStartLevelPanel()
    {
        selectedLevelIndex = VehicleManager.Instance.GetCurrentLevelIndex();
        selectedLevelText.text = $"LEVEL {selectedLevelIndex + 1}";
        startLevelPanel.SetActive(true);
        startLevelCG.alpha = 0f;
        StartLoopVehicleIconAnimation(selectedLevelIndex / 5);
    }
    private void AnimateStartLevelPanel()
    {
        UIAnimator.FadeIn(startLevelCG);
        UIAnimator.MoveFromY(header, 200f, 0.4f, Ease.OutBack, 0f);
        UIAnimator.ScaleIn(levelImage, 0.4f, 0.1f);
        UIAnimator.MoveFromX(selectedLevelText.transform, -800f, 0.4f, Ease.OutExpo, 0.2f);
        UIAnimator.ScaleIn(playButtonTransform, 0.4f, 0.35f);
    }

    private void OnPlayButtonClicked()
    {
        PlayCloudTransition(CloudTransitionType.Close);
    }
    public bool IsStartLevelPanelActive() => startLevelPanel.activeSelf;
    public void StartLoopVehicleIconAnimation(int regionIndex)
    {
        regionLoopIconImage.sprite = regionIndex < regionSprites.Count ? regionSprites[regionIndex] : null;
        regionLoopIconImage.DOFade(1f, 0.5f);

        loopTween?.Kill();

        float distance = 150f;
        float duration = 5f;

        RectTransform rt = regionLoopIconTransform;

        rt.anchoredPosition = new Vector2(-distance, rt.anchoredPosition.y);

        loopTween = DOTween.Sequence()
            .AppendCallback(() => rt.localRotation = Quaternion.Euler(0, 180f, 0))
            .Append(rt.DOAnchorPosX(distance, duration).SetEase(Ease.InOutSine))
            .AppendCallback(() => rt.localRotation = Quaternion.Euler(0, 0f, 0))
            .Append(rt.DOAnchorPosX(-distance, duration).SetEase(Ease.InOutSine))
            .SetLoops(-1);
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
    }
    public void ToggleSound()
    {
        HapticsManager.Instance.PlayUIFeedback("UI_Click", HapticsManager.Instance.PlaySelectionImpactVibration);

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
        UpdateToggleVisual(soundToggleIcon, soundToggleBackground, isSoundOn, iconSounds, bgSettingsButtons);
    }
    public void ToggleVibration()
    {
        HapticsManager.Instance.PlayUIFeedback("UI_Click", HapticsManager.Instance.PlaySelectionImpactVibration);

        isVibrationOn = !isVibrationOn;
        PlayerPrefs.SetInt("Vibration", isVibrationOn ? 1 : 0);
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

    #region === Coin Buy ===
    public void ShowCoinBuyPanel()
    {
        HideAllPanels();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("OpeningImportantPanel");

        coinBuyPanel.SetActive(true);

        bool hasInternet = Application.internetReachability != NetworkReachability.NotReachable;

        UIAnimator.FadeIn(coinBuyCG);
        UIAnimator.ScaleIn(coinBuyHeader);
        UIAnimator.ScaleIn(coinBuyImage, 0.3f, 0.3f);
        UIAnimator.MoveFromX(coinBuyText, -1000, 0.3f, Ease.OutExpo, 0.6f);

        coinBuyButton.gameObject.SetActive(hasInternet);
        coinBuyButtonNoConnection.gameObject.SetActive(!hasInternet);

        Transform buttonToAnimate = hasInternet ? coinBuyButton.transform : coinBuyButtonNoConnection.transform;
        UIAnimator.ScaleIn(buttonToAnimate, 0.3f, 0.9f);
    }
    public void BuyCoins(int coinAmount)
    {
        //TODO: Rewarded Ad Integration

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("AddCoin");

        CurrencyManager.Instance.AddCoin(coinAmount);

        PlayCoinFlyEffect(new Vector3(0f, 20, 0));

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

    #region === Coin Fly Effect Process ===
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
            HapticsManager.Instance.PlayRigidImpactVibration();
            RectTransform coinRT = coin.GetComponent<RectTransform>();
            coinRT.anchoredPosition = uiStartPos;
            coinRT.localScale = Vector3.one;

            Vector2 randomOffset = Random.insideUnitCircle * coinFlyOffset;

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

    #region === Region Progress Bar ===
    public void UpdateRegionProgressBar(int currentLevel)
    {
        int levelsPerRegion = 5;

        int regionIndex = currentLevel / levelsPerRegion;
        int progressInRegion = currentLevel % levelsPerRegion;

        float fillAmount = progressInRegion / (float)levelsPerRegion;
        regionProgressFill.fillAmount = fillAmount;
        regionProgressText.text = $"{progressInRegion} / {levelsPerRegion}";

        if (progressInRegion == 0 && currentLevel != 0)
        {
            ShowCenterRegionName(regionIndex);
        }
        else
        {
            ShowSideRegionName(regionIndex);
        }
    }
    private void ShowCenterRegionName(int regionIndex)
    {
        RectTransform rt = regionNameText.GetComponent<RectTransform>();
        RectTransform iconRT = regionIconImage.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(0f, centerRegionNameY);

        SetRegionVisuals(regionIndex);

        float originalFontSize = regionNameText.fontSize;
        regionNameText.fontSize = originalFontSize * REGION_NAME_FONT_SCALE;

        iconRT.anchoredPosition = new Vector2(regionIconMoveX, iconRT.anchoredPosition.y);
        regionNameText.DOFade(0f, 0f);
        regionIconImage.DOFade(0f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(regionDelayBeforeFade);

        seq.AppendCallback(() =>
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("NewRegionIdle");
                HapticsManager.Instance.PlaySuccessVibration();
            }
        });

        seq.Append(regionNameText.DOFade(1f, regionFadeDuration));
        seq.Join(regionIconImage.DOFade(1f, regionFadeDuration));

        seq.Join(iconRT.DOAnchorPosX(200f, regionFadeDuration).SetEase(Ease.Linear));

        seq.AppendInterval(0.5f);

        seq.Append(regionNameText.DOFade(0f, regionFadeDuration));
        seq.Join(regionIconImage.DOFade(0f, regionFadeDuration));

        seq.OnComplete(() =>
        {
            regionNameText.fontSize = originalFontSize;
        });
    }
    private void ShowSideRegionName(int regionIndex)
    {
        SetRegionVisuals(regionIndex);

        regionNameText.DOFade(0f, 0f);
        regionIconImage.DOFade(0f, 0f);

        RectTransform iconRT = regionIconImage.GetComponent<RectTransform>();

        iconRT.anchoredPosition = new Vector2(-100f, iconRT.anchoredPosition.y);

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(1f);

        seq.Append(regionNameText.DOFade(1f, 2f));
        seq.Join(regionIconImage.DOFade(1f, 2f));

        seq.Join(iconRT.DOAnchorPosX(100f, 2f).SetEase(Ease.Linear));

        seq.AppendInterval(0.5f);

        seq.Append(regionNameText.DOFade(0f, 2f));
        seq.Join(regionIconImage.DOFade(0f, 2f));
    }
    private void SetRegionVisuals(int regionIndex)
    {
        regionNameText.text = regionIndex < regionNames.Length ? regionNames[regionIndex] : "Unknown";
        regionIconImage.sprite = regionIndex < regionSprites.Count ? regionSprites[regionIndex] : null;
    }
    #endregion

    #region === Shop Panel ===
    public void ShowShopPanel()
    {
        HideAllPanels();
        shopPanel.SetActive(true);
        shopCG.alpha = 0;
        shopContent.localScale = Vector3.zero;

        UIAnimator.FadeIn(shopCG);
        UIAnimator.ScaleIn(shopContent);
    }
    #endregion

    #region === Cloud Transition ===
    public enum CloudTransitionType
    {
        Open,
        Close
    }

    public void PlayCloudTransition(CloudTransitionType type)
    {
        AudioManager.Instance?.PlaySFX("CloudEffect");
        HapticsManager.Instance?.PlaySoftImpactVibration();

        cloudTransitionPanel.SetActive(true);

        Vector2 leftTarget, rightTarget;

        Vector2 leftInitial = leftCloud.anchoredPosition;
        Vector2 rightInitial = rightCloud.anchoredPosition;

        if (type == CloudTransitionType.Open)
        {
            leftCloud.anchoredPosition = Vector2.zero;
            rightCloud.anchoredPosition = Vector2.zero;
            leftTarget = leftInitial;
            rightTarget = rightInitial;
        }
        else
        {
            leftTarget = Vector2.zero;
            rightTarget = Vector2.zero;
        }

        Sequence seq = DOTween.Sequence();
        seq.Append(leftCloud.DOAnchorPos(leftTarget, 0.8f).SetEase(type == CloudTransitionType.Open ? Ease.OutQuad : Ease.InOutQuad));
        seq.Join(rightCloud.DOAnchorPos(rightTarget, 0.8f).SetEase(type == CloudTransitionType.Open ? Ease.OutQuad : Ease.InOutQuad));

        if (type == CloudTransitionType.Open)
        {
            seq.AppendCallback(() => cloudTransitionPanel.SetActive(false));
        }
        else
        {
            seq.AppendCallback(() => SceneManager.LoadScene(1));
        }
    }
    #endregion

    #region === Tutorial ===
    public void ShowTutorial()
    {
        PrepareTutorialPanel();
        PositionTutorialHighlight();
        PlayTutorialAnimations();
    }
    private void PrepareTutorialPanel()
    {
        if (Camera.main.TryGetComponent<CameraDragController>(out var cam))
        {
            cam.SetTutorialStartPosition(Camera.main.transform.position);
        }

        tutorialActive = true;

        tutorialPanel.SetActive(true);
        tutorialCG.alpha = 0f;
        tutorialCG.blocksRaycasts = true;
        tutorialCG.interactable = false;

        tutorialCG.DOFade(1f, tutorialFadeDuration);
    }
    private void PositionTutorialHighlight()
    {
        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, playButton.transform.position);
        RectTransform canvasRect = tutorialHighlightCircle.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos + tutorialCircleOffset,
            Camera.main,
            out Vector2 localPos
        );

        float verticalUIOffset = tutorialVerticalOffset * 100f;
        localPos.y += verticalUIOffset;

        tutorialHighlightCircle.anchoredPosition = localPos;

        tutorialHighlightCircle.localScale = Vector3.one;
        tutorialHighlightCircle.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);
    }
    private void PlayTutorialAnimations()
    {
        tutorialPulseTween = DOTween.Sequence()
            .Append(tutorialHighlightCircle.DOScale(tutorialPulseScaleUp, 0.6f).SetEase(Ease.OutQuad))
            .Join(tutorialHighlightCircle.GetComponent<Image>().DOFade(tutorialPulseFadeIn, 0.6f))
            .Append(tutorialHighlightCircle.DOScale(tutorialPulseScaleDown, 0.6f).SetEase(Ease.InQuad))
            .Join(tutorialHighlightCircle.GetComponent<Image>().DOFade(tutorialPulseFadeOut, 0.6f))
            .SetLoops(-1)
            .SetUpdate(true);

        fingerImage.gameObject.SetActive(true);
        DOTween.Sequence()
            .Append(fingerImage.transform.DOScale(tutorialPulseFadeIn, 0.55f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true));
    }
    public void HideTutorialPanel()
    {
        tutorialActive = false;

        tutorialCG.blocksRaycasts = false;

        if (tutorialPulseTween != null && tutorialPulseTween.IsActive())
            tutorialPulseTween.Kill();

        tutorialHighlightCircle.DOScale(tutorialPulseFadeOut, 0.3f).SetEase(Ease.InBack);

        tutorialCG.DOFade(0f, 0.5f).OnComplete(() =>
        {
            tutorialPanel.SetActive(false);
        });

        fingerImage.transform.DOKill();
        fingerImage.gameObject.SetActive(false);

    }
    public void SetTutorialAsSeen()
    {
        PlayerPrefs.SetInt("HasSeenIdleTutorial", 1);
        PlayerPrefs.Save();
    }
    #endregion
}
