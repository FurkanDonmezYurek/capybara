using UnityEngine;
using UnityEngine.UI;

public class MessagePanelControl : MonoBehaviour
{
    [SerializeField] private GameObject messagePanel;    // Message Panel objesi
    [SerializeField] private Button messageButton;       // Message Button
    [SerializeField] private Button closeButton;         // Close Button
    [SerializeField] private Text messageText;           // Message Text

    private void Start()
    {
        // Butonlara fonksiyon ekle
        messageButton.onClick.AddListener(OpenMessagePanel);  // Message Button'a tıklandığında açılacak
        closeButton.onClick.AddListener(CloseMessagePanel);   // Close Button'a tıklandığında kapanacak
    }

    // Message Panel'i açma fonksiyonu
    private void OpenMessagePanel()
    {
    
        messagePanel.SetActive(true);  // Message Panel aktif hale getirilir
    }

    // Message Panel'i kapatma fonksiyonu
    private void CloseMessagePanel()
    {
        messagePanel.SetActive(false);  // Message Panel kapanır
    }
}
