using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PurchaseProduct
{
    public string productId;
    public string title;
    public string description;
    public string price;
    public bool consumable;
}

public class BillingManager : MonoBehaviour
{
    public static BillingManager Instance { get; private set; }

    [SerializeField] private List<PurchaseProduct> products = new List<PurchaseProduct>
    {
        new PurchaseProduct { productId = "gems_100", title = "100 Gems", description = "$0.99", price = "0.99", consumable = false },
        new PurchaseProduct { productId = "gems_500", title = "500 Gems", description = "$4.99", price = "4.99", consumable = false },
        new PurchaseProduct { productId = "gems_1000", title = "1000 Gems", description = "$9.99", price = "9.99", consumable = false },
        new PurchaseProduct { productId = "battle_pass", title = "Premium Pass", description = "$4.99", price = "4.99", consumable = false },
        new PurchaseProduct { productId = "founder_pack", title = "Founder's Pack", description = "$9.99", price = "9.99", consumable = false },
    };

    public System.Action<string> OnPurchaseSuccess;
    public System.Action<string> OnPurchaseFailed;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeBilling();
    }

    void InitializeBilling()
    {
        Debug.Log("BillingManager: Initializing Google Play Billing...");
        // TODO: Integrate Google Play Billing Library
        // In production, use:
        // CodelessIAPManager or BillingManager from Google Play In-App Billing
    }

    public void PurchaseProduct(string productId)
    {
        var product = products.Find(p => p.productId == productId);
        if (product == null)
        {
            Debug.LogError("Product not found: " + productId);
            return;
        }

        Debug.Log("Attempting purchase: " + productId);
        // TODO: Call Google Play Billing API
        // For testing, simulate:
        SimulatePurchase(productId);
    }

    void SimulatePurchase(string productId)
    {
        // Sandbox/test mode: auto-grant gems
        int gemsToAdd = 0;
        switch (productId)
        {
            case "gems_100": gemsToAdd = 100; break;
            case "gems_500": gemsToAdd = 500; break;
            case "gems_1000": gemsToAdd = 1000; break;
            case "battle_pass":
                BattlePassManager.Instance.UnlockPremiumPass();
                break;
            case "founder_pack":
                BattlePassManager.Instance.UnlockPremiumPass();
                CosmeticManager.Instance.OwnCosmetic("skin_gold");
                gemsToAdd = 200;
                break;
        }

        if (gemsToAdd > 0)
        {
            CurrencyManager.Instance.AddGems(gemsToAdd);
        }

        Debug.Log("Purchase successful (simulated): " + productId);
        OnPurchaseSuccess?.Invoke(productId);
    }

    public void RequestProductInfo(string productId)
    {
        var product = products.Find(p => p.productId == productId);
        if (product != null)
        {
            Debug.Log($"{product.title}: {product.price}");
        }
    }
}
