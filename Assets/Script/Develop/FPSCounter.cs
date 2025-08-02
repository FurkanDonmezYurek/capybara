using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FPSCounter : MonoBehaviour
{
    private static FPSCounter instance;

    public float updateInterval = 0.5f;

    private float accumulated = 0.0f;
    private int frames = 0;
    private float timeLeft;

    private Text fpsText;
    private Canvas canvas;

    void Awake()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0; // VSync devre dışı, FPS kontrolü çalışır

        if (instance != null)
        {
            Destroy(gameObject); // Zaten varsa, bu kopyayı sil
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Sahne geçişlerinde silinmesin
        CreateUI();
    }

    void CreateUI()
    {
        canvas = new GameObject("FPSCanvas").AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        DontDestroyOnLoad(canvas.gameObject);

        GameObject textObj = new GameObject("FPSText");
        textObj.transform.SetParent(canvas.transform);

        fpsText = textObj.AddComponent<Text>();
        fpsText.font = Resources.Load<Font>("Font");
        fpsText.fontSize = 24;
        fpsText.alignment = TextAnchor.UpperRight;
        fpsText.color = Color.green;
        fpsText.rectTransform.anchorMin = new Vector2(1, 1);
        fpsText.rectTransform.anchorMax = new Vector2(1, 1);
        fpsText.rectTransform.pivot = new Vector2(1, 1);
        fpsText.rectTransform.anchoredPosition = new Vector2(-10, -10);
        fpsText.rectTransform.sizeDelta = new Vector2(200, 50);
    }

    void Start()
    {
        timeLeft = updateInterval;
    }

    void Update()
    {
        timeLeft -= Time.deltaTime;
        accumulated += Time.timeScale / Time.deltaTime;
        frames++;

        if (timeLeft <= 0.0)
        {
            float fps = accumulated / frames;
            fpsText.text = $"FPS: {fps:F1}";

            fpsText.color = fps >= 50 ? Color.green : (fps >= 30 ? Color.yellow : Color.red);

            timeLeft = updateInterval;
            accumulated = 0.0f;
            frames = 0;
        }
    }
}
