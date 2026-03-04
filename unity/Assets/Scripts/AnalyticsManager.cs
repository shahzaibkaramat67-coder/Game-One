using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance { get; private set; }

    private string userId;

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
        userId = Guid.NewGuid().ToString("N").Substring(0, 8);
        TrackEvent("app_launch", JsonUtility.ToJson(new AppLaunchProps { userId = userId }));
    }

    public void TrackEvent(string eventName, string propertiesJson = null)
    {
        StartCoroutine(TrackEventCoroutine(eventName, propertiesJson));
    }

    public void TrackPurchase(string productId, float price)
    {
        TrackEvent("purchase", JsonUtility.ToJson(new PurchaseProps { productId = productId, price = price }));
    }

    public void TrackMatchComplete(bool won, int durationSeconds)
    {
        TrackEvent("match_complete", JsonUtility.ToJson(new MatchCompleteProps { won = won, durationSeconds = durationSeconds }));
    }

    public void TrackBattlePassLevel(int level)
    {
        TrackEvent("battle_pass_level_up", JsonUtility.ToJson(new BattlePassLevelProps { level = level }));
    }

    System.Collections.IEnumerator TrackEventCoroutine(string eventName, string propertiesJson)
    {
        string url = Config.Instance.httpServerUrl + "/api/analytics/track";
        var body = new AnalyticsEventRequest
        {
            eventName = eventName,
            userId = userId,
            propertiesJson = propertiesJson
        };

        var json = JsonUtility.ToJson(body);
        var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Analytics error: " + req.error);
        }
    }
}

[Serializable]
public class AnalyticsEventRequest
{
    public string eventName;
    public string userId;
    public string propertiesJson;
}

[Serializable]
public class AppLaunchProps
{
    public string userId;
}

[Serializable]
public class PurchaseProps
{
    public string productId;
    public float price;
}

[Serializable]
public class MatchCompleteProps
{
    public bool won;
    public int durationSeconds;
}

[Serializable]
public class BattlePassLevelProps
{
    public int level;
}

