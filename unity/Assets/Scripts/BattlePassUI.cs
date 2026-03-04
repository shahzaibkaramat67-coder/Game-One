using UnityEngine;
using UnityEngine.UI;

public class BattlePassUI : MonoBehaviour
{
    [SerializeField] private Text freePassLevelText;
    [SerializeField] private Text premiumPassLevelText;
    [SerializeField] private Text freePassXPText;
    [SerializeField] private Text premiumPassXPText;
    [SerializeField] private Button buyPassButton;
    [SerializeField] private Button founderPackButton;
    [SerializeField] private Text statusText;

    private BattlePassManager bpManager;
    private CurrencyManager currencyManager;

    void Start()
    {
        bpManager = BattlePassManager.Instance;
        currencyManager = CurrencyManager.Instance;

        buyPassButton.onClick.AddListener(OnBuyPassClicked);
        founderPackButton.onClick.AddListener(OnFounderPackClicked);

        RefreshUI();
    }

    void Update()
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        var freeTrack = bpManager.GetFreeTrack();
        var premiumTrack = bpManager.GetPremiumTrack();

        freePassLevelText.text = "Level: " + freeTrack.level;
        freePassXPText.text = freeTrack.currentXP + " / " + freeTrack.xpToNextLevel;

        if (bpManager.IsPremiumOwned())
        {
            premiumPassLevelText.text = "Level: " + premiumTrack.level;
            premiumPassXPText.text = premiumTrack.currentXP + " / " + premiumTrack.xpToNextLevel;
            buyPassButton.interactable = false;
            founderPackButton.interactable = false;
            statusText.text = "Premium Pass Unlocked ✓";
        }
        else
        {
            premiumPassLevelText.text = "LOCKED";
            premiumPassXPText.text = "Unlock to earn rewards!";
            buyPassButton.interactable = true;
            founderPackButton.interactable = true;
            statusText.text = "Free tier only";
        }
    }

    void OnBuyPassClicked()
    {
        int price = BattlePassManager.PASS_PRICE_GEMS;
        if (currencyManager.HasGems(price))
        {
            currencyManager.RemoveGems(price);
            bpManager.UnlockPremiumPass();
            Debug.Log("Premium Pass purchased!");
        }
        else
        {
            Debug.Log("Not enough gems!");
        }
    }

    void OnFounderPackClicked()
    {
        int price = BattlePassManager.FOUNDER_PRICE_GEMS;
        if (currencyManager.HasGems(price))
        {
            currencyManager.RemoveGems(price);
            bpManager.UnlockPremiumPass();
            CosmeticManager.Instance.OwnCosmetic("skin_gold"); // Legendary skin
            currencyManager.AddGems(200); // Refund some gems
            Debug.Log("Founder's Pack purchased!");
        }
        else
        {
            Debug.Log("Not enough gems for Founder's Pack!");
        }
    }
}
