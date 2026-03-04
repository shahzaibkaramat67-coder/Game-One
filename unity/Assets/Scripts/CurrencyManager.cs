using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private int coins = 0;      // Soft currency (earned in-game)
    [SerializeField] private int gems = 0;       // Premium currency (bought with real money)

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
        LoadCurrency();
    }

    public int GetCoins() => coins;
    public int GetGems() => gems;

    public bool HasCoins(int amount) => coins >= amount;
    public bool HasGems(int amount) => gems >= amount;

    public void AddCoins(int amount)
    {
        coins = Mathf.Max(0, coins + amount);
        SaveCurrency();
    }

    public void RemoveCoins(int amount)
    {
        if (HasCoins(amount))
        {
            coins -= amount;
            SaveCurrency();
        }
    }

    public void AddGems(int amount)
    {
        gems = Mathf.Max(0, gems + amount);
        SaveCurrency();
    }

    public void RemoveGems(int amount)
    {
        if (HasGems(amount))
        {
            gems -= amount;
            SaveCurrency();
        }
    }

    void SaveCurrency()
    {
        PlayerPrefs.SetInt("coins", coins);
        PlayerPrefs.SetInt("gems", gems);
        PlayerPrefs.Save();
    }

    void LoadCurrency()
    {
        coins = PlayerPrefs.GetInt("coins", 0);
        gems = PlayerPrefs.GetInt("gems", 0);
    }
}
