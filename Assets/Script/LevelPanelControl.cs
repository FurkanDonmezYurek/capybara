using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelPanelControl : MonoBehaviour
{
    [SerializeField] private GameObject levelPanel;    // Level Panel
    [SerializeField] private Button startButton;       // Başla Button
    [SerializeField] private Button closeButton;       // Kapat Button
    [SerializeField] private TextMeshProUGUI levelText; // Level Text (Bölüm numarası)

    private int currentLevel = 1;  // Başlangıçta Level 1

    private void Start()
    {
        // Butonlara fonksiyon ekle
        startButton.onClick.AddListener(StartLevel);  // Başla butonuna tıklandığında başlatacak
        closeButton.onClick.AddListener(CloseLevelPanel);   // Kapat butonuna tıklandığında kapanacak
    }

    // Başla butonuna basıldığında level başlasın
    private void StartLevel()
    {
        // Burada oyunun başladığı fonksiyonu çağırabilirsin (level başlatma gibi)
        Debug.Log("Level " + currentLevel + " Başladı!");

        SceneManager.LoadScene("Level1");  // "Level1" sahnesine geçiş yap


        // Örneğin, yeni seviyeye geçmek için:
        // SceneManager.LoadScene("Level" + currentLevel); 

        levelPanel.SetActive(false);  // Level Paneli kapat
    }

    // Kapat butonuna basıldığında level paneli kapanacak
    private void CloseLevelPanel()
    {
        levelPanel.SetActive(false);  // Level Panelini kapat
    }

    // Level seçimini yaparken (Level 1, 2, 3 vb.)
    public void OpenLevelPanel(int level)
    {
        currentLevel = level;  // Seçilen level’i güncelle
        levelText.text = "Bölüm " + level.ToString();  // Text ile bölüm numarasını göster
        levelPanel.SetActive(true);  // Level panelini aç
    }
}
