using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening; // DOTween için gerekli namespace

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject loadingPanel;   // Loading Paneli
    [SerializeField] private TextMeshProUGUI loadingText; // Yükleniyor yazısı
    [SerializeField] private Image progressBar;         // Progress bar (isteğe bağlı)
    [SerializeField] private Image spinnerImage;         // Dönen spinner (isteğe bağlı)

    // Bu fonksiyon sahneyi yüklerken loading ekranını gösterir
    public void LoadLevel(int levelIndex)
    {
        // Loading panelini aktif etmeden önce, panelin aktif olduğundan emin olalım
        loadingPanel.SetActive(true);

        // Eğer spinnerImage null ise, animasyon başlatma
        if (spinnerImage != null)
        {
            // Dönen spinner animasyonunu başlat
            spinnerImage.transform.DORotate(new Vector3(0, 0, 360), 1f, RotateMode.LocalAxisAdd)
                .SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);  // Spinner sürekli dönecek
        }

        // Loading text'in yavaşça görünmesini sağla
        if (loadingText != null)
        {
            loadingText.DOFade(1f, 0.5f).SetEase(Ease.Linear);  // Yavaşça belirginleşmesini sağla
        }

        // Asenkron olarak sahneyi yükle
        StartCoroutine(LoadSceneAsync(levelIndex)); 
    }

    private IEnumerator LoadSceneAsync(int levelIndex)
    {
        // Yükleme işlemi başlıyor
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelIndex);

        // Yükleme işlemi tamamlanana kadar bekle
        while (!asyncLoad.isDone)
        {
            // Progress bar'ı güncelle
            if (progressBar != null)
            {
                progressBar.fillAmount = asyncLoad.progress;

                // Progress bar'ın dolmasını sağlamak için
                float targetProgress = asyncLoad.progress;
                progressBar.DOFillAmount(targetProgress, 0.5f).SetEase(Ease.Linear);  // Yavaşça dolmasını sağla
            }

            // Yükleme yüzdesini göster
            if (loadingText != null)
                loadingText.text = "Yükleniyor... " + (asyncLoad.progress * 100f).ToString("F0") + "%";

            yield return null;  // Bir sonraki frame'e geç
        }

        // Sahne yüklendikten sonra spinner animasyonunu durdur
        if (spinnerImage != null)
        {
            spinnerImage.transform.DOKill();  // Dönen animasyonu sonlandır
        }

        // Loading ekranını kapat
        loadingPanel.SetActive(false);  
    }
}
