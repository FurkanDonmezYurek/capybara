using UnityEngine;
using UnityEngine.UI;

public class GiftButton : MonoBehaviour
{
    [SerializeField] private Button giftButton;
    [SerializeField] private int minReward = 10;
    [SerializeField] private int maxReward = 100;

    private void Start()
    {
        giftButton.onClick.AddListener(GiveGift);
    }

    private void GiveGift()
    {
        int reward = Random.Range(minReward, maxReward + 1);

        // Örnek: Coin Manager'a ödül gönder
        CurrencyManager.Instance.AddCoin(reward);

        Debug.Log("Gift verildi! Coin: " + reward);

        // Butonu devre dışı bırak (birden fazla tıklamayı önlemek için)
        giftButton.interactable = false;

        // İstersen animasyon/effect/feedback ekleyebilirsin
    }
}
