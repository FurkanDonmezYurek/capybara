using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro kullanıyorsanız

public class LevelLockManager : MonoBehaviour
{
    [Header("Level Button Elements")]
    [SerializeField] private Button[] levelButtons; // Tüm level butonları
    [SerializeField] private GameObject[] lockIcons; // Kilit ikonları
    [SerializeField] private TextMeshProUGUI[] levelTexts; // Level Text’leri

    private int currentLevel = 1; // Başlangıç seviyesi (örneğin Level 1)

    private void Start()
    {
        // Level'leri kontrol et
        UpdateLevelLock();
    }

    // Seviye kilitlerini güncelle
    public void UpdateLevelLock()
    {
        for (int i = 0; i < levelButtons.Length; i++)
        {
            // Eğer oyuncu bu seviyeyi açabilirse, butonu etkinleştir
            if (i < currentLevel) // Örneğin, Level 1'i açabilmek için 1. seviyeyi bitirmiş olmalı
            {
                levelButtons[i].interactable = true;
                lockIcons[i].SetActive(false); // Kilidi kaldır
                levelTexts[i].text = "Level " + (i + 1);
            }
            else
            {
                levelButtons[i].interactable = false;
                lockIcons[i].SetActive(true); // Kilidi göster
                levelTexts[i].text = "Locked"; // Kilitli yazısı göster
            }
        }
    }

    // Seviye bitirildiğinde kilidi aç (oyuncu 1. seviyeyi bitirdiğinde 2. seviye açılabilir)
    public void UnlockNextLevel()
    {
        if (currentLevel < levelButtons.Length) // Son seviyeye gelmediyse
        {
            currentLevel++;
            UpdateLevelLock(); // Kilit durumunu güncelle
        }
    }

    // Bu metodu bir Level Butonuna bağlayın: Level seçildiğinde
    public void OnLevelButtonClick(int levelNumber)
    {
        if (levelNumber <= currentLevel) // Seviye kilidi açıksa
        {
            Debug.Log("Level " + levelNumber + " başlatılıyor.");
            // Sahne geçişi veya seviyenin başlatılması burada yapılabilir.
            // SceneManager.LoadScene("Level" + levelNumber);
        }
        else
        {
            Debug.Log("Seviye kilitli, önce önceki seviyeyi bitir!");
        }
    }
}
