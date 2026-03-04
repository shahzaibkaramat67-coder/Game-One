using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class WebSocketClient : MonoBehaviour
{
    public string serverUrl;
    public string userId;
    public DuelManager duelManager;

    private ClientWebSocket ws;
    private CancellationTokenSource cts;
    private readonly ConcurrentQueue<string> inbound = new ConcurrentQueue<string>();

    async void Start()
    {
        if (string.IsNullOrEmpty(userId)) userId = Guid.NewGuid().ToString("N").Substring(0, 8);
        if (string.IsNullOrEmpty(serverUrl)) serverUrl = Config.Instance.wsServerUrl;

        if (duelManager != null) duelManager.SetLocalUserId(userId);

#if UNITY_WEBGL
        Debug.LogError("WebSockets are not supported by this client implementation on WebGL.");
#else
        await ConnectToServer();
#endif
    }

    void Update()
    {
        while (inbound.TryDequeue(out var msg))
        {
            HandleMessage(msg);
        }
    }

    public async Task ConnectToServer()
    {
        try
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            ws?.Dispose();
            ws = new ClientWebSocket();

            var uri = new Uri($"{serverUrl}?userId={Uri.EscapeDataString(userId)}");
            await ws.ConnectAsync(uri, cts.Token);

            Debug.Log("WebSocket connected");
            _ = ReceiveLoop();

            if (duelManager != null) duelManager.SetLocalUserId(userId);
            EnqueueForMatch();
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket connect failed: " + e.Message);
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8 * 1024];
        var sb = new StringBuilder();

        try
        {
            while (ws != null && ws.State == WebSocketState.Open && !cts.IsCancellationRequested)
            {
                sb.Clear();

                WebSocketReceiveResult result;
                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cts.Token);
                    if (result.MessageType == WebSocketMessageType.Close) break;
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                if (result.MessageType == WebSocketMessageType.Close) break;

                inbound.Enqueue(sb.ToString());
            }
        }
        catch (OperationCanceledException)
        {
            // ignore
        }
        catch (Exception e)
        {
            Debug.LogWarning("WebSocket receive loop stopped: " + e.Message);
        }
    }

    private void HandleMessage(string msg)
    {
        var baseMsg = JsonUtility.FromJson<ServerMessage>(msg);
        if (baseMsg == null || string.IsNullOrEmpty(baseMsg.type)) return;

        if (baseMsg.type == "match_start")
        {
            duelManager?.SetLocalUserId(userId);
            duelManager?.StartMatch();
            return;
        }

        if (baseMsg.type == "match_end")
        {
            duelManager?.EndMatch(baseMsg.winnerId);
            return;
        }

        if (baseMsg.type == "hit")
        {
            var hit = JsonUtility.FromJson<HitMessage>(msg);
            var target = !string.IsNullOrEmpty(hit.targetId) ? hit.targetId : hit.target;
            duelManager?.OnRemoteHit(target, hit.damage);
        }
    }

    public void EnqueueForMatch()
    {
        _ = SendJson(new EnqueueMessage());
    }

    public void SendAction(string actionType)
    {
        _ = SendJson(new ActionEnvelope
        {
            type = "action",
            action = new PlayerAction
            {
                type = actionType,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            }
        });
    }

    private async Task SendJson<T>(T payload)
    {
        try
        {
            if (ws == null || ws.State != WebSocketState.Open) return;
            var json = JsonUtility.ToJson(payload);
            var bytes = Encoding.UTF8.GetBytes(json);
            await ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cts.Token);
        }
        catch (Exception e)
        {
            Debug.LogWarning("WebSocket send failed: " + e.Message);
        }
    }

    async void OnDestroy()
    {
        try
        {
            cts?.Cancel();
            if (ws != null && ws.State == WebSocketState.Open)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "closing", CancellationToken.None);
            }
        }
        catch
        {
            // ignore
        }
        finally
        {
            ws?.Dispose();
            ws = null;
        }
    }
}

[Serializable]
public class EnqueueMessage
{
    public string type = "enqueue";
}

[Serializable]
public class ActionEnvelope
{
    public string type;
    public PlayerAction action;
}

[Serializable]
public class PlayerAction
{
    public string type;
    public long timestamp;
}

[Serializable]
public class ServerMessage
{
    public string type;
    public string matchId;
    public string winnerId;
    public string selfId;
    public string opponentId;
}

[Serializable]
public class HitMessage
{
    public string type;
    public string target;
    public string targetId;
    public int damage;
    public int hp;
}
