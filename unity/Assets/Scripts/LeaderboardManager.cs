using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class LeaderboardEntry
{
    public string userId;
    public string displayName;
    public int rating;
    public int wins;
    public int losses;
    public int rank;
}

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private List<LeaderboardEntry> topPlayers = new List<LeaderboardEntry>();
    private LeaderboardEntry playerRank;

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

    public void FetchTopPlayers(int limit, Action<List<LeaderboardEntry>> callback)
    {
        StartCoroutine(FetchTopPlayersCoroutine(limit, callback));
    }

    public void FetchPlayerRank(string userId, Action<LeaderboardEntry> callback)
    {
        StartCoroutine(FetchPlayerRankCoroutine(userId, callback));
    }

    public void UpdateRating(string userId, bool won, Action<int> callback)
    {
        StartCoroutine(UpdateRatingCoroutine(userId, won, callback));
    }

    System.Collections.IEnumerator FetchTopPlayersCoroutine(int limit, Action<List<LeaderboardEntry>> callback)
    {
        string url = Config.Instance.httpServerUrl + "/api/leaderboard/top/" + limit;
        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Leaderboard fetch error: " + req.error);
                callback(new List<LeaderboardEntry>());
                yield break;
            }

            try
            {
                var data = JsonUtility.FromJson<LeaderboardWrapper>("{\"entries\":" + req.downloadHandler.text + "}");
                topPlayers = data?.entries != null ? new List<LeaderboardEntry>(data.entries) : new List<LeaderboardEntry>();
                callback(topPlayers);
            }
            catch (Exception e)
            {
                Debug.LogError("Leaderboard parse error: " + e);
                callback(new List<LeaderboardEntry>());
            }
        }
    }

    System.Collections.IEnumerator FetchPlayerRankCoroutine(string userId, Action<LeaderboardEntry> callback)
    {
        string url = Config.Instance.httpServerUrl + "/api/leaderboard/rank/" + userId;
        using (var req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Rank fetch error: " + req.error);
                callback(null);
                yield break;
            }

            try
            {
                playerRank = JsonUtility.FromJson<LeaderboardEntry>(req.downloadHandler.text);
                callback(playerRank);
            }
            catch (Exception e)
            {
                Debug.LogError("Rank parse error: " + e);
                callback(null);
            }
        }
    }

    System.Collections.IEnumerator UpdateRatingCoroutine(string userId, bool won, Action<int> callback)
    {
        string url = Config.Instance.httpServerUrl + "/api/leaderboard/update-rating";
        var payload = new UpdateRatingRequest { userId = userId, opponent = "ai", won = won };
        var json = JsonUtility.ToJson(payload);

        var req = new UnityWebRequest(url, "POST");
        req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json));
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();
        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Rating update error: " + req.error);
            callback(0);
            yield break;
        }

        try
        {
            var response = JsonUtility.FromJson<RatingResponse>(req.downloadHandler.text);
            callback(response != null ? response.ratingChange : 0);
        }
        catch (Exception e)
        {
            Debug.LogError("Rating parse error: " + e);
            callback(0);
        }
    }

    public List<LeaderboardEntry> GetTopPlayers() => topPlayers;
    public LeaderboardEntry GetPlayerRank() => playerRank;
}

[Serializable]
public class LeaderboardWrapper
{
    public LeaderboardEntry[] entries;
}

[Serializable]
public class RatingResponse
{
    public bool success;
    public int ratingChange;
}

[Serializable]
public class UpdateRatingRequest
{
    public string userId;
    public string opponent;
    public bool won;
}

