using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LiveOffer
{
    public string id;
    public string title;
    public string description;
    public int originalPrice;
    public int discountedPrice;
    public int discountPercent;
    public float remainingSeconds;
    public bool isActive;
}

public class LiveOpsManager : MonoBehaviour
{
    public static LiveOpsManager Instance { get; private set; }

    [SerializeField] private List<LiveOffer> activeOffers = new List<LiveOffer>();

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
        GenerateDailyOffers();
    }

    void Update()
    {
        // Tick down offer timers
        foreach (var offer in activeOffers)
        {
            offer.remainingSeconds -= Time.deltaTime;
            if (offer.remainingSeconds <= 0)
            {
                offer.isActive = false;
            }
        }
    }

    void GenerateDailyOffers()
    {
        // Clear old offers
        activeOffers.Clear();

        // Generate random cosmetic flash sales (24h limit)
        string[] cosmetics = { "skin_red", "skin_blue", "emote_victory", "nameplate_elite" };
        int[] prices = { 150, 150, 100, 200 };
        int[] discounts = { 30, 30, 25, 40 }; // percent off

        for (int i = 0; i < 3; i++)
        {
            int idx = Random.Range(0, cosmetics.Length);
            int original = prices[idx];
            int discount = discounts[idx];
            int discounted = original - (original * discount / 100);

            activeOffers.Add(new LiveOffer
            {
                id = "flash_" + i,
                title = cosmetics[idx],
                description = discount + "% OFF - 24h only!",
                originalPrice = original,
                discountedPrice = discounted,
                discountPercent = discount,
                remainingSeconds = 86400f, // 24 hours
                isActive = true
            });
        }

        Debug.Log("Generated " + activeOffers.Count + " daily flash offers");
    }

    public List<LiveOffer> GetActiveOffers()
    {
        return activeOffers.FindAll(o => o.isActive);
    }

    public void LogEvent(string eventName, string userId)
    {
        // TODO: Send to Analytics
        Debug.Log($"[LIVE-OPS EVENT] {eventName} | User: {userId}");
    }
}
