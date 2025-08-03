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

    [Header("Start Level Panel")]
    [SerializeField] public GameObject startLevelPanel;
    [SerializeField] private CanvasGroup startLevelCG;
    [SerializeField] private Transform header;
    [SerializeField] private Transform levelImage;
    [SerializeField] private TextMeshProUGUI selectedLevelText;
    [SerializeField] private Button playButton;
    [SerializeField] private Transform playButtonTransform;
    [SerializeField] private Button closeButton;
    [SerializeField] private Image regionLoopIconImage;
    [SerializeField] private RectTransform regionLoopIconTransform;

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

    [Header("Region Progress Bar")]
    [SerializeField] private Image regionProgressFill;
    [SerializeField] private TextMeshProUGUI regionProgressText;
    [SerializeField] private TextMeshProUGUI regionNameText;
    [SerializeField] private string[] regionNames; //"Desert", "Forest", "Snow"...
    [SerializeField] private CanvasGroup centerRegionCG;
    [SerializeField] private Image regionIconImage;
    [SerializeField] private List<Sprite> regionSprites;

    [Header("Shop Panel")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private CanvasGroup shopCG;
    [SerializeField] private Transform shopContent;

    [Header("Cloud Transition")]
    [SerializeField] private GameObject cloudTransitionPanel;
    [SerializeField] private RectTransform leftCloud;
    [SerializeField] private RectTransform rightCloud;

    [Header("Tutorial")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private CanvasGroup tutorialCG;
    [SerializeField] private RectTransform tutorialHighlightCircle;
    [SerializeField] private float tutorialDelayBeforeAutoClose = 1.5f;
    [SerializeField] private Vector3 tutorialCircleOffset = new Vector3(0, 50, 0);
    [SerializeField] private Image fingerImage;
    [SerializeField] private float tutorialVerticalOffset = -5f;

    private Tween tutorialPulseTween;
    private bool tutorialActive = false;

    private Tween coinTween;
    private Tween loopTween;
    private int selectedLevelIndex;
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

        UpdateRegionProgressBar(VehicleManager.Instance.GetCurrentLevelIndex());
        UpdateSoundToggleVisual();
        UpdateVibrationToggleVisual();
        PlayCloudOpenTransition();
        if (!PlayerPrefs.HasKey("HasSeenIdleTutorial"))
        {
            ShowTutorial();
        }
    }

    #region === Panel On/Off ===

    public void HideAllPanels()
    {
        if(AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("UI_Click");
            HapticsManager.Instance.PlayMediumImpactVibration();
        }


        if (startLevelPanel.activeSelf)
            HidePanelWithAnimation(startLevelCG, playButtonTransform, startLevelPanel);

        if (coinBuyPanel.activeSelf)
            HidePanelWithAnimation(coinBuyCG, coinBuyHeader, coinBuyPanel);

        if (settingsPanel.activeSelf)
            HidePanelWithAnimation(settingsCG, settingsContent, settingsPanel);

        if (shopPanel.activeSelf)
            HidePanelWithAnimation(shopCG, shopContent, shopPanel);
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
        SetTutorialAsSeen();
        HideTutorialPanel();
        HideAllPanels();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("OpeningImportantPanel");

        selectedLevelIndex = VehicleManager.Instance.GetCurrentLevelIndex();

        selectedLevelText.text = "LEVEL " + (selectedLevelIndex + 1);

        startLevelPanel.SetActive(true);
        startLevelCG.alpha = 0f;

        StartLoopVehicleIconAnimation(selectedLevelIndex / 5);

        UIAnimator.FadeIn(startLevelCG);
        UIAnimator.MoveFromY(header, 200f, 0.4f, Ease.OutBack, 0f);
        UIAnimator.ScaleIn(levelImage, 0.4f, 0.1f);
        UIAnimator.MoveFromX(selectedLevelText.transform, -800f, 0.4f, Ease.OutExpo, 0.2f);
        UIAnimator.ScaleIn(playButtonTransform, 0.4f, 0.35f);
    }
    private void OnPlayButtonClicked()
    {
        //HideAllPanels();
        //int LevelIndex = PlayerPrefs.GetInt("Level", 0);
        //LevelIndex++;
        //PlayerPrefs.SetInt("Level", LevelIndex);
        ////GameManager.Instance.LevelStart();

        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // TODO: Optional restart animation
        PlayCloudCloseTransition();
    }
    public bool PanelActived()
    {
        return startLevelPanel.activeSelf;
    }
    public void StartLoopVehicleIconAnimation(int regionIndex)
    {
        if (regionIndex < regionSprites.Count)
            regionLoopIconImage.sprite = regionSprites[regionIndex];
        else
            regionLoopIconImage.sprite = null;

        regionLoopIconImage.DOFade(1f, 0.5f);

        if (loopTween != null && loopTween.IsActive())
            loopTween.Kill();

        float distance = 150f; 
        float duration = 5f;

        RectTransform rt = regionLoopIconTransform;

        rt.anchoredPosition = new Vector2(-distance, rt.anchoredPosition.y);
        rt.localRotation = Quaternion.Euler(0, 180f, 0); 

        loopTween = DOTween.Sequence()
            .AppendCallback(() => rt.localRotation = Quaternion.Euler(0, 180f, 0)) 
            .Append(
                rt.DOAnchorPosX(distance, duration).SetEase(Ease.InOutSine)
            )
            .AppendCallback(() => rt.localRotation = Quaternion.Euler(0, 0f, 0)) 
            .Append(
                rt.DOAnchorPosX(-distance, duration).SetEase(Ease.InOutSine)
            )
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

        UpdateSoundToggleVisual();
        UpdateVibrationToggleVisual();
    }

    public void ToggleSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("UI_Click");
            HapticsManager.Instance.PlaySelectionImpactVibration();
        }

        isSoundOn = !isSoundOn;
        if (isSoundOn)
        {
            AudioManager.Instance.MuteMusic(false);
            AudioManager.Instance.MuteMusic(false);
        }
        else
        {
            AudioManager.Instance.MuteMusic(true);
            AudioManager.Instance.MuteSFX(true);
        }
        PlayerPrefs.SetInt("Sound", isSoundOn ? 1 : 0);
        UpdateSoundToggleVisual();
    }

    public void ToggleVibration()
    {
        if (AudioManager.Instance != null)
        {
            HapticsManager.Instance.PlaySelectionImpactVibration();
            AudioManager.Instance.PlaySFX("UI_Click");
        }
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

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX("OpeningImportantPanel");

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

    #region === Region Progress Bar ===
    public void UpdateRegionProgressBar(int currentLevel)
    {
        int levelsPerRegion = 5;

        int regionIndex = currentLevel / levelsPerRegion;
        int progressInRegion = currentLevel % levelsPerRegion;

        float fillAmount = progressInRegion / (float)levelsPerRegion;
        regionProgressFill.fillAmount = fillAmount;
        regionProgressText.text = $"{progressInRegion} / {levelsPerRegion}";

        regionNameText.text = regionIndex < regionNames.Length ? regionNames[regionIndex] : "Unknown";

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

        rt.anchoredPosition = new Vector2(0f, -1250f);

        regionNameText.text = regionIndex < regionNames.Length ? regionNames[regionIndex] : "Unknown";
        regionIconImage.sprite = regionIndex < regionSprites.Count ? regionSprites[regionIndex] : null;

        float originalFontSize = regionNameText.fontSize;
        regionNameText.fontSize = originalFontSize * 2f;

        iconRT.anchoredPosition = new Vector2(-200f, iconRT.anchoredPosition.y);
        regionNameText.DOFade(0f, 0f);
        regionIconImage.DOFade(0f, 0f);

        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.75f);

        seq.AppendCallback(() =>
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX("NewRegionIdle");
                HapticsManager.Instance.PlaySuccessVibration();
            }
        });

        seq.Append(regionNameText.DOFade(1f, 2f));
        seq.Join(regionIconImage.DOFade(1f, 2f));

        seq.Join(iconRT.DOAnchorPosX(200f, 2f).SetEase(Ease.Linear));

        seq.AppendInterval(0.5f);

        seq.Append(regionNameText.DOFade(0f, 2f));
        seq.Join(regionIconImage.DOFade(0f, 2f));

        seq.OnComplete(() =>
        {
            regionNameText.fontSize = originalFontSize;
        });
    }

    private void ShowSideRegionName(int regionIndex)
    {
        regionNameText.text = regionIndex < regionNames.Length ? regionNames[regionIndex] : "Unknown";
        regionIconImage.sprite = regionIndex < regionSprites.Count ? regionSprites[regionIndex] : null;

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
    public void PlayCloudOpenTransition()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("CloudEffect");
            HapticsManager.Instance.PlaySoftImpactVibration();
        }

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

    public void PlayCloudCloseTransition()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("CloudEffect");
            HapticsManager.Instance.PlaySoftImpactVibration();
        }

        cloudTransitionPanel.SetActive(true);

        Vector2 leftStartPos = leftCloud.anchoredPosition;
        Vector2 rightStartPos = rightCloud.anchoredPosition;
        Vector2 closedPos = Vector2.zero;

        Sequence seq = DOTween.Sequence();
        seq.Append(leftCloud.DOAnchorPos(closedPos, 0.8f).SetEase(Ease.InOutQuad));
        seq.Join(rightCloud.DOAnchorPos(closedPos, 0.8f).SetEase(Ease.InOutQuad));
        seq.AppendCallback(() =>
        {
            SceneManager.LoadScene(1);
        });
    }
    #endregion

    #region === Tutorial ===
    public void ShowTutorial()
    {
        CameraDragController cam = Camera.main.GetComponent<CameraDragController>();
        if (cam != null)
        {
            cam.SetTutorialStartPosition(Camera.main.transform.position);
        }


        tutorialActive = true;

        tutorialPanel.SetActive(true);
        tutorialCG.alpha = 0f;
        tutorialCG.blocksRaycasts = true;
        tutorialCG.interactable = false;

        tutorialCG.DOFade(1f, 2.5f);

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(Camera.main, playButton.transform.position);
        RectTransform canvasRect = tutorialHighlightCircle.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 localPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos + tutorialCircleOffset,
            Camera.main,
            out localPos
        );

        float verticalUIOffset = tutorialVerticalOffset * 100f; 
        localPos.y += verticalUIOffset;

        tutorialHighlightCircle.anchoredPosition = localPos;

        tutorialHighlightCircle.localScale = Vector3.one;
        tutorialHighlightCircle.GetComponent<Image>().color = new Color(1, 1, 1, 0.6f);

        tutorialPulseTween = DOTween.Sequence()
            .Append(tutorialHighlightCircle.DOScale(1.2f, 0.6f).SetEase(Ease.OutQuad))
            .Join(tutorialHighlightCircle.GetComponent<Image>().DOFade(0.9f, 0.6f))
            .Append(tutorialHighlightCircle.DOScale(1.0f, 0.6f).SetEase(Ease.InQuad))
            .Join(tutorialHighlightCircle.GetComponent<Image>().DOFade(0.6f, 0.6f))
            .SetLoops(-1)
            .SetUpdate(true);

        fingerImage.gameObject.SetActive(true);
        DOTween.Sequence()
            .Append(fingerImage.transform.DOScale(0.9f, 0.55f)
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

        tutorialHighlightCircle.DOScale(0.6f, 0.3f).SetEase(Ease.InBack);

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
