using UnityEngine;
using UnityEngine.UI;

public class CosmeticItemUI : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Text priceText;
    [SerializeField] private Button buyButton;
    [SerializeField] private Text statusText;

    private Cosmetic cosmetic;
    public System.Action<Cosmetic> OnPurchase;

    void Start()
    {
        buyButton.onClick.AddListener(OnBuyClicked);
    }

    public void SetCosmetic(Cosmetic c)
    {
        cosmetic = c;
        nameText.text = c.name;
        priceText.text = c.price.ToString();
        statusText.text = c.isPremium ? "💎" : "🪙";

        if (c.owned)
        {
            buyButton.interactable = false;
            buyButton.GetComponentInChildren<Text>().text = "OWNED";
        }
        else
        {
            buyButton.interactable = true;
            buyButton.GetComponentInChildren<Text>().text = "BUY";
        }
    }

    void OnBuyClicked()
    {
        OnPurchase?.Invoke(cosmetic);
    }
}
