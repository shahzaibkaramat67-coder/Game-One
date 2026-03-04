using UnityEngine;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private Transform storePanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button gems100Button;
    [SerializeField] private Button gems500Button;
    [SerializeField] private Button gems1000Button;

    private BillingManager billingManager;

    void Start()
    {
        billingManager = BillingManager.Instance;

        closeButton.onClick.AddListener(() => storePanel.gameObject.SetActive(false));
        gems100Button.onClick.AddListener(() => billingManager.PurchaseProduct("gems_100"));
        gems500Button.onClick.AddListener(() => billingManager.PurchaseProduct("gems_500"));
        gems1000Button.onClick.AddListener(() => billingManager.PurchaseProduct("gems_1000"));
    }

    public void OpenStore()
    {
        storePanel.gameObject.SetActive(true);
    }
}
