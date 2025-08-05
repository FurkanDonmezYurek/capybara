using UnityEngine;
using UnityEngine.UI;

public class GiftPanelControl : MonoBehaviour
{
    [SerializeField] private GameObject giftPanel;    // Gift Panel objesi
    [SerializeField] private Button giftButton;       // Gift butonu
    [SerializeField] private Button closeButton;      // Close butonu
    [SerializeField] private Text rewardText;         // Ödül metni

    private void Start()
    {
        // Buttonlara fonksiyon ekle
        giftButton.onClick.AddListener(OpenGiftPanel);  // Gift Button'a tıklandığında açılacak
        closeButton.onClick.AddListener(CloseGiftPanel);  // Close Button'a tıklandığında kapanacak
    }

    // Gift Panel'i açma fonksiyonu
    private void OpenGiftPanel()
    {
        // Ödülü rastgele belirle (örneğin: 10 - 100 coin)
        int rewardAmount = Random.Range(10, 100);
       

      

        giftPanel.SetActive(true);  // Gift Panel aktif hale getirilir
    }

    // Gift Panel'i kapatma fonksiyonu
    private void CloseGiftPanel()
    {
        giftPanel.SetActive(false);  // Gift Panel kapanır
    }
}
