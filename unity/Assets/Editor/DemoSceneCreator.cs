using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class DemoSceneCreator
{
    [MenuItem("DuelRush/Create Demo Scene")]
    public static void CreateDemoScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        // Players
        var playerA = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playerA.name = "PlayerA";
        playerA.transform.position = new Vector3(-5f, 0f, 0f);
        var pcA = playerA.AddComponent<PlayerController>();
        pcA.maxHP = 100;

        var playerB = GameObject.CreatePrimitive(PrimitiveType.Cube);
        playerB.name = "PlayerB";
        playerB.transform.position = new Vector3(5f, 0f, 0f);
        var pcB = playerB.AddComponent<PlayerController>();
        pcB.maxHP = 100;

        // Managers (singletons + wiring)
        var gm = new GameObject("GameManager");
        gm.AddComponent<ConfigLoader>();
        gm.AddComponent<CurrencyManager>();
        gm.AddComponent<CosmeticManager>();
        gm.AddComponent<BattlePassManager>();
        gm.AddComponent<LiveOpsManager>();
        gm.AddComponent<AnalyticsManager>();
        gm.AddComponent<LeaderboardManager>();
        gm.AddComponent<BillingManager>();

        var duelManager = gm.AddComponent<DuelManager>();
        duelManager.playerA = pcA;
        duelManager.playerB = pcB;

        var wsClient = gm.AddComponent<WebSocketClient>();
        wsClient.duelManager = duelManager;

        gm.AddComponent<GameSetup>();

        // UI
        EnsureEventSystem();
        var canvas = CreateCanvas();
        var uiManager = canvas.AddComponent<UIManager>();
        duelManager.uiManager = uiManager;

        uiManager.playerA = pcA;
        uiManager.playerB = pcB;
        uiManager.duelManager = duelManager;
        uiManager.wsClient = wsClient;

        uiManager.playerAHPText = CreateText(canvas.transform, "P1HPText", new Vector2(10, -10), TextAnchor.UpperLeft);
        uiManager.playerBHPText = CreateText(canvas.transform, "P2HPText", new Vector2(-10, -10), TextAnchor.UpperRight);
        uiManager.timerText = CreateText(canvas.transform, "TimerText", new Vector2(0, -10), TextAnchor.UpperCenter);
        uiManager.resultText = CreateText(canvas.transform, "ResultText", new Vector2(0, 100), TextAnchor.MiddleCenter, 36);

        uiManager.attackButton = CreateButton(canvas.transform, "AttackBtn", "ATTACK", new Vector2(-140, 80));
        uiManager.dodgeButton = CreateButton(canvas.transform, "DodgeBtn", "DODGE", new Vector2(-140, 20));
        uiManager.rematchButton = CreateButton(canvas.transform, "RematchBtn", "REMATCH", new Vector2(0, -120));

        // Save scene and add to build settings
        const string scenesDir = "Assets/Scenes";
        if (!Directory.Exists(scenesDir)) Directory.CreateDirectory(scenesDir);

        const string scenePath = scenesDir + "/DuelScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);

        AddSceneToBuild(scenePath);

        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(scenePath));
        Debug.Log("Created demo scene: " + scenePath);
    }

    private static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    private static Canvas CreateCanvas()
    {
        var go = new GameObject("Canvas");
        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        go.AddComponent<CanvasScaler>();
        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static Text CreateText(Transform parent, string name, Vector2 anchoredPos, TextAnchor anchor, int fontSize = 24)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(500, 60);
        rect.anchorMin = rect.anchorMax = AnchorToVector(anchor);
        rect.pivot = AnchorToVector(anchor);
        rect.anchoredPosition = anchoredPos;

        var txt = go.AddComponent<Text>();
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.fontSize = fontSize;
        txt.alignment = anchor;
        txt.color = Color.white;
        txt.text = "";
        return txt;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 anchoredPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rect = go.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(220, 60);
        rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchoredPosition = anchoredPos;

        var img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.9f);

        var btn = go.AddComponent<Button>();

        var text = CreateText(go.transform, "Text", Vector2.zero, TextAnchor.MiddleCenter, 22);
        text.text = label;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);
        text.rectTransform.anchoredPosition = Vector2.zero;
        text.rectTransform.sizeDelta = Vector2.zero;

        return btn;
    }

    private static Vector2 AnchorToVector(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return new Vector2(0f, 1f);
            case TextAnchor.UpperCenter: return new Vector2(0.5f, 1f);
            case TextAnchor.UpperRight: return new Vector2(1f, 1f);
            case TextAnchor.MiddleLeft: return new Vector2(0f, 0.5f);
            case TextAnchor.MiddleCenter: return new Vector2(0.5f, 0.5f);
            case TextAnchor.MiddleRight: return new Vector2(1f, 0.5f);
            case TextAnchor.LowerLeft: return new Vector2(0f, 0f);
            case TextAnchor.LowerCenter: return new Vector2(0.5f, 0f);
            case TextAnchor.LowerRight: return new Vector2(1f, 0f);
            default: return new Vector2(0.5f, 0.5f);
        }
    }

    private static void AddSceneToBuild(string scenePath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        var existing = scenes.FindIndex(s => s.path == scenePath);
        if (existing >= 0)
        {
            scenes[existing].enabled = true;
        }
        else
        {
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}

