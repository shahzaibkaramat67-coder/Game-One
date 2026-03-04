using UnityEngine;

[CreateAssetMenu(menuName = "DuelRush/Config", fileName = "Config")]
public class Config : ScriptableObject
{
    public static Config Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<Config>("Config");
                if (_instance == null)
                {
                    // Allow the project to run even if the asset wasn't created yet.
                    // (An editor menu item can generate the asset for persistence.)
                    _instance = CreateInstance<Config>();
                    _instance.hideFlags = HideFlags.DontSave;
                    Debug.LogWarning("Config asset not found at Resources/Config.asset. Using runtime defaults.");
                }
            }
            return _instance;
        }
    }
    private static Config _instance;

    [Header("Server Settings")]
    public string wsServerUrl = "ws://localhost:3000";
    public string httpServerUrl = "http://localhost:3000";
    public string serverEnvironment = "development"; // development, staging, production

    [Header("Game Settings")]
    public int matchDuration = 45;
    public int playerMaxHP = 100;
    public int damagePerHit = 12;

    [Header("Monetization")]
    public int battlePassPriceGems = 499;      // $4.99
    public int founderPackPriceGems = 999;     // $9.99
    public int coinsPerMatchWin = 25;
    public int xpPerMatch = 100;

    public void UpdateEnvironment(string environment)
    {
        serverEnvironment = environment;
        switch (environment)
        {
            case "development":
                wsServerUrl = "ws://localhost:3000";
                httpServerUrl = "http://localhost:3000";
                break;
            case "staging":
                wsServerUrl = "wss://staging.duelrush.com";
                httpServerUrl = "https://staging.duelrush.com";
                break;
            case "production":
                wsServerUrl = "wss://duelrush.com";
                httpServerUrl = "https://duelrush.com";
                break;
        }
    }
}
