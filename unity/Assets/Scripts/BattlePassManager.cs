using UnityEngine;

[System.Serializable]
public class BattlePassTrack
{
    public string name;
    public int level;
    public int currentXP;
    public int xpToNextLevel = 1000;
}

public class BattlePassManager : MonoBehaviour
{
    public static BattlePassManager Instance { get; private set; }

    [SerializeField] private bool premiumOwned = false;
    [SerializeField] private int seasonNumber = 1;
    [SerializeField] private BattlePassTrack freeTrack = new BattlePassTrack { name = "Free", level = 1 };
    [SerializeField] private BattlePassTrack premiumTrack = new BattlePassTrack { name = "Premium", level = 1 };

    // Season pass cost
    public const int PASS_PRICE_GEMS = 499;  // $4.99
    public const int FOUNDER_PRICE_GEMS = 999; // $9.99 (includes pass + legendary skin)

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
        LoadBattlePass();
    }

    public bool IsPremiumOwned() => premiumOwned;
    public int GetSeasonNumber() => seasonNumber;
    public BattlePassTrack GetFreeTrack() => freeTrack;
    public BattlePassTrack GetPremiumTrack() => premiumTrack;

    public void UnlockPremiumPass()
    {
        premiumOwned = true;
        SaveBattlePass();
    }

    public void AddXP(int amount)
    {
        // Add to free track always
        freeTrack.currentXP += amount;
        if (freeTrack.currentXP >= freeTrack.xpToNextLevel)
        {
            freeTrack.currentXP -= freeTrack.xpToNextLevel;
            freeTrack.level++;
            Debug.Log("Free Pass Level Up: " + freeTrack.level);
        }

        // Add to premium track if owned
        if (premiumOwned)
        {
            premiumTrack.currentXP += amount;
            if (premiumTrack.currentXP >= premiumTrack.xpToNextLevel)
            {
                premiumTrack.currentXP -= premiumTrack.xpToNextLevel;
                premiumTrack.level++;
                Debug.Log("Premium Pass Level Up: " + premiumTrack.level);
            }
        }

        SaveBattlePass();
    }

    void SaveBattlePass()
    {
        PlayerPrefs.SetInt("bp_premium", premiumOwned ? 1 : 0);
        PlayerPrefs.SetInt("bp_season", seasonNumber);
        PlayerPrefs.SetInt("bp_free_level", freeTrack.level);
        PlayerPrefs.SetInt("bp_free_xp", freeTrack.currentXP);
        PlayerPrefs.SetInt("bp_premium_level", premiumTrack.level);
        PlayerPrefs.SetInt("bp_premium_xp", premiumTrack.currentXP);
        PlayerPrefs.Save();
    }

    void LoadBattlePass()
    {
        premiumOwned = PlayerPrefs.GetInt("bp_premium", 0) == 1;
        seasonNumber = PlayerPrefs.GetInt("bp_season", 1);
        freeTrack.level = PlayerPrefs.GetInt("bp_free_level", 1);
        freeTrack.currentXP = PlayerPrefs.GetInt("bp_free_xp", 0);
        premiumTrack.level = PlayerPrefs.GetInt("bp_premium_level", 1);
        premiumTrack.currentXP = PlayerPrefs.GetInt("bp_premium_xp", 0);
    }
}
