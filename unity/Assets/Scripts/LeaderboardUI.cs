using UnityEngine;
using UnityEngine.UI;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private Transform leaderboardPanel;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform entriesContainer;
    [SerializeField] private GameObject entryPrefab;
    [SerializeField] private Text playerRankText;

    private LeaderboardManager lbManager;
    private string playerId;

    void Start()
    {
        lbManager = LeaderboardManager.Instance;
        playerId = System.Guid.NewGuid().ToString().Substring(0, 8);
        closeButton.onClick.AddListener(() => leaderboardPanel.gameObject.SetActive(false));
    }

    public void OpenLeaderboard()
    {
        leaderboardPanel.gameObject.SetActive(true);
        RefreshLeaderboard();
    }

    void RefreshLeaderboard()
    {
        lbManager.FetchTopPlayers(20, (entries) =>
        {
            // Clear existing
            foreach (Transform child in entriesContainer)
            {
                Destroy(child.gameObject);
            }

            // Add entries
            for (int i = 0; i < entries.Count; i++)
            {
                var entry = entries[i];
                var item = Instantiate(entryPrefab, entriesContainer);
                var text = item.GetComponent<Text>();
                text.text = $"#{i + 1} {entry.displayName} - {entry.rating} Rating ({entry.wins}W {entry.losses}L)";
            }
        });

        // Show player rank
        lbManager.FetchPlayerRank(playerId, (rank) =>
        {
            if (rank != null)
            {
                playerRankText.text = $"Your Rank: #{rank.rank} | Rating: {rank.rating}";
            }
        });
    }
}
