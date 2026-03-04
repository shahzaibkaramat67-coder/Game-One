using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    [SerializeField] private PlayerController playerA;
    [SerializeField] private PlayerController playerB;
    [SerializeField] private DuelManager duelManager;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private WebSocketClient wsClient;

    void Awake()
    {
        if (ConfigLoader.Instance == null) gameObject.AddComponent<ConfigLoader>();
    }

    void Start()
    {
        if (playerA == null) playerA = FindPlayerByName("PlayerA");
        if (playerB == null) playerB = FindPlayerByName("PlayerB");

        if (playerA == null || playerB == null || playerA == playerB)
        {
            var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            if (players.Length >= 1 && playerA == null) playerA = players[0];
            if (players.Length >= 2 && (playerB == null || playerB == playerA)) playerB = players[1];
        }

        if (duelManager == null) duelManager = FindObjectOfType<DuelManager>();
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>();
        if (wsClient == null) wsClient = FindObjectOfType<WebSocketClient>();

        if (duelManager != null)
        {
            if (playerA != null) duelManager.playerA = playerA;
            if (playerB != null) duelManager.playerB = playerB;
            if (uiManager != null) duelManager.uiManager = uiManager;
        }

        if (uiManager != null)
        {
            if (playerA != null) uiManager.playerA = playerA;
            if (playerB != null) uiManager.playerB = playerB;
            if (duelManager != null) uiManager.duelManager = duelManager;
            if (wsClient != null) uiManager.wsClient = wsClient;
        }

        if (wsClient != null && duelManager != null) wsClient.duelManager = duelManager;

        Debug.Log($"Game initialized | Environment: {ConfigLoader.Instance.GetCurrentEnvironment()}");
    }

    private static PlayerController FindPlayerByName(string name)
    {
        var go = GameObject.Find(name);
        return go != null ? go.GetComponent<PlayerController>() : null;
    }
}

