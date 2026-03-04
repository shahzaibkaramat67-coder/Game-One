using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text playerAHPText;
    public Text playerBHPText;
    public Text timerText;
    public Text resultText;
    public Button attackButton;
    public Button dodgeButton;
    public Button rematchButton;

    public PlayerController playerA;
    public PlayerController playerB;
    public DuelManager duelManager;
    public WebSocketClient wsClient;

    void Start()
    {
        if (attackButton != null) attackButton.onClick.AddListener(OnAttackPressed);
        if (dodgeButton != null) dodgeButton.onClick.AddListener(OnDodgePressed);
        if (rematchButton != null) rematchButton.onClick.AddListener(OnRematchPressed);
    }

    void Update()
    {
        if (playerAHPText != null && playerBHPText != null)
        {
            playerAHPText.text = "P1 HP: " + playerA.currentHP;
            playerBHPText.text = "P2 HP: " + playerB.currentHP;
        }

        if (timerText != null && duelManager != null)
        {
            timerText.text = "Time: " + Mathf.Max(0, duelManager.GetRemainingTime()).ToString("F1") + "s";
        }
    }

    void OnAttackPressed()
    {
        if (playerA != null) playerA.PrimaryAttack();
        if (wsClient != null) wsClient.SendAction("attack");
    }

    void OnDodgePressed()
    {
        if (playerA != null) playerA.Dodge();
        if (wsClient != null) wsClient.SendAction("dodge");
    }

    void OnRematchPressed()
    {
        if (wsClient != null) wsClient.EnqueueForMatch();
    }

    public void ShowResult(string text)
    {
        if (resultText != null) resultText.text = text;
    }
}
