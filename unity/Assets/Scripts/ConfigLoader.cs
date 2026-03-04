using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnvironmentConfig
{
    public string wsServerUrl;
    public string httpServerUrl;
    public string environment;
    public int matchDuration;
    public int playerMaxHP;
    public int damagePerHit;
    public int battlePassPriceGems;
    public int founderPackPriceGems;
    public int coinsPerMatchWin;
    public int xpPerMatch;
}

[System.Serializable]
public class EnvironmentConfigWrapper
{
    public EnvironmentConfig development;
    public EnvironmentConfig staging;
    public EnvironmentConfig production;

    public EnvironmentConfig GetConfig(string env)
    {
        switch (env.ToLower())
        {
            case "staging": return staging;
            case "production": return production;
            default: return development;
        }
    }
}

public class ConfigLoader : MonoBehaviour
{
    public static ConfigLoader Instance { get; private set; }

    private EnvironmentConfig currentConfig;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadEnvironment();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadEnvironment()
    {
        // Determine environment from PlayerPrefs. If not set, default to:
        // - development in Editor (localhost)
        // - production in builds (deployed backend)
        string envName;
        if (PlayerPrefs.HasKey("environment"))
        {
            envName = PlayerPrefs.GetString("environment", "development");
        }
        else
        {
            envName = Application.isEditor ? "development" : "production";
        }
        
        // Load env.json
        TextAsset envJson = Resources.Load<TextAsset>("env");
        if (envJson == null)
        {
            Debug.LogError("env.json not found in Resources folder!");
            return;
        }

        try
        {
            EnvironmentConfigWrapper wrapper = JsonUtility.FromJson<EnvironmentConfigWrapper>(envJson.text);
            currentConfig = wrapper.GetConfig(envName);
            ApplyConfig(currentConfig);
            Debug.Log($"✓ Environment loaded: {envName}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse env.json: " + e.message);
        }
    }

    void ApplyConfig(EnvironmentConfig config)
    {
        if (Config.Instance != null)
        {
            Config.Instance.wsServerUrl = config.wsServerUrl;
            Config.Instance.httpServerUrl = config.httpServerUrl;
            Config.Instance.serverEnvironment = config.environment;
            Config.Instance.matchDuration = config.matchDuration;
            Config.Instance.playerMaxHP = config.playerMaxHP;
            Config.Instance.damagePerHit = config.damagePerHit;
            Config.Instance.battlePassPriceGems = config.battlePassPriceGems;
            Config.Instance.founderPackPriceGems = config.founderPackPriceGems;
            Config.Instance.coinsPerMatchWin = config.coinsPerMatchWin;
            Config.Instance.xpPerMatch = config.xpPerMatch;
        }
    }

    public void SetEnvironment(string envName)
    {
        PlayerPrefs.SetString("environment", envName);
        PlayerPrefs.Save();
        LoadEnvironment();
        Debug.Log($"✓ Environment switched to: {envName}. Restart app to apply.");
    }

    public string GetCurrentEnvironment()
    {
        return currentConfig?.environment ?? "development";
    }
}
