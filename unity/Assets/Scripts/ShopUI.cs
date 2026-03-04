using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [SerializeField] private Transform shopPanel;
    [SerializeField] private Button closeShopButton;
    [SerializeField] private Text currencyText;
    [SerializeField] private GridLayoutGroup cosmeticGrid;
    [SerializeField] private GameObject cosmeticItemPrefab;

    private CosmeticManager cosmeticManager;
    private CurrencyManager currencyManager;

    void Start()
    {
        cosmeticManager = CosmeticManager.Instance;
        currencyManager = CurrencyManager.Instance;

        closeShopButton.onClick.AddListener(() => shopPanel.gameObject.SetActive(false));
        PopulateShop();
    }

    void Update()
    {
        currencyText.text = $"Coins: {currencyManager.GetCoins()} | Gems: {currencyManager.GetGems()}";
    }

    void PopulateShop()
    {
        // Clear existing
        foreach (Transform child in cosmeticGrid.transform)
        {
            Destroy(child.gameObject);
        }

        // Add cosmetic items
        foreach (var cosmetic in cosmeticManager.GetAllCosmetics())
        {
            var item = Instantiate(cosmeticItemPrefab, cosmeticGrid.transform);
            var itemUI = item.GetComponent<CosmeticItemUI>();
            if (itemUI != null)
            {
                itemUI.SetCosmetic(cosmetic);
                itemUI.OnPurchase += OnCosmeticPurchased;
            }
        }
    }

    void OnCosmeticPurchased(Cosmetic cosmetic)
    {
        if (cosmetic.owned)
        {
            Debug.Log("Already owned: " + cosmetic.name);
            return;
        }

        bool canAfford = cosmetic.isPremium
            ? currencyManager.HasGems(cosmetic.price)
            : currencyManager.HasCoins(cosmetic.price);

        if (!canAfford)
        {
            Debug.Log("Cannot afford: " + cosmetic.name);
            return;
        }

        // Deduct currency
        if (cosmetic.isPremium)
            currencyManager.RemoveGems(cosmetic.price);
        else
            currencyManager.RemoveCoins(cosmetic.price);

        // Mark as owned
        cosmeticManager.OwnCosmetic(cosmetic.id);
        PopulateShop(); // Refresh display
    }

    public void OpenShop()
    {
        shopPanel.gameObject.SetActive(true);
    }
}
