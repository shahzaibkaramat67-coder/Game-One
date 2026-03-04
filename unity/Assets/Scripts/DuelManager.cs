using UnityEngine;

public class DuelManager : MonoBehaviour
{
    public PlayerController playerA;
    public PlayerController playerB;
    public UIManager uiManager;
    public float matchDuration = 45f;

    private float timer;
    private bool running;
    private string localUserId;

    void Start()
    {
        timer = matchDuration;
        running = false;
    }

    void Update()
    {
        if (!running) return;

        timer -= Time.deltaTime;
        if (timer <= 0f) EndMatch(null);
    }

    public void SetLocalUserId(string userId)
    {
        localUserId = userId;
    }

    public void StartMatch()
    {
        timer = matchDuration;
        running = true;

        if (playerA != null) playerA.currentHP = playerA.maxHP;
        if (playerB != null) playerB.currentHP = playerB.maxHP;

        uiManager?.ShowResult("");
    }

    public void EndMatch(string winnerId)
    {
        running = false;

        string resultMsg;
        if (!string.IsNullOrEmpty(winnerId) && !string.IsNullOrEmpty(localUserId))
        {
            resultMsg = winnerId == localUserId ? "Player 1 Wins!" : "Player 2 Wins!";
        }
        else
        {
            int winner = 0;
            if (playerA != null && playerB != null)
            {
                if (playerA.currentHP > playerB.currentHP) winner = 1;
                else if (playerB.currentHP > playerA.currentHP) winner = 2;
            }

            resultMsg = winner == 1 ? "Player 1 Wins!" : (winner == 2 ? "Player 2 Wins!" : "Draw!");
        }

        Debug.Log("Match ended");
        uiManager?.ShowResult(resultMsg);

        // Rewards (local prototype)
        const int xpReward = 100;
        const int coinReward = 25;
        BattlePassManager.Instance?.AddXP(xpReward);
        CurrencyManager.Instance?.AddCoins(coinReward);
    }

    public float GetRemainingTime()
    {
        return running ? timer : 0f;
    }

    public void OnRemoteHit(string targetUserId, int damage)
    {
        if (playerA == null || playerB == null) return;

        // Convention: PlayerA represents "this device", PlayerB represents the opponent.
        if (!string.IsNullOrEmpty(localUserId))
        {
            if (targetUserId == localUserId) playerA.ApplyDamage(damage);
            else playerB.ApplyDamage(damage);
            return;
        }

        // Fallback for older server payloads
        if (targetUserId == "A") playerA.ApplyDamage(damage);
        else if (targetUserId == "B") playerB.ApplyDamage(damage);
    }
}

