using UnityEngine;

[System.Serializable]
public class Cosmetic
{
    public string id;
    public string name;
    public int price;
    public bool isPremium;  // true = gems, false = coins
    public bool owned;

    public Cosmetic(string id, string name, int price, bool isPremium, bool owned = false)
    {
        this.id = id;
        this.name = name;
        this.price = price;
        this.isPremium = isPremium;
        this.owned = owned;
    }
}

public class CosmeticManager : MonoBehaviour
{
    public static CosmeticManager Instance { get; private set; }

    [SerializeField] private Cosmetic[] availableCosmetics = new Cosmetic[]
    {
        new Cosmetic("skin_red", "Red Warrior", 150, false),
        new Cosmetic("skin_blue", "Blue Ninja", 150, false),
        new Cosmetic("skin_gold", "Gold Legend", 500, true),
        new Cosmetic("emote_victory", "Victory Dance", 100, false),
        new Cosmetic("emote_taunt", "Taunt Emote", 250, true),
        new Cosmetic("frame_diamond", "Diamond Frame", 400, true),
        new Cosmetic("nameplate_elite", "Elite Badge", 200, false),
        new Cosmetic("nameplate_legendary", "Legendary Badge", 600, true),
    };

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
        LoadOwnedCosmetics();
    }

    public Cosmetic[] GetAllCosmetics() => availableCosmetics;

    public bool OwnCosmetic(string cosmeticId)
    {
        foreach (var cosmetic in availableCosmetics)
        {
            if (cosmetic.id == cosmeticId)
            {
                cosmetic.owned = true;
                SaveOwnedCosmetics();
                return true;
            }
        }
        return false;
    }

    public bool IsOwned(string cosmeticId)
    {
        foreach (var cosmetic in availableCosmetics)
        {
            if (cosmetic.id == cosmeticId) return cosmetic.owned;
        }
        return false;
    }

    void SaveOwnedCosmetics()
    {
        foreach (var cosmetic in availableCosmetics)
        {
            PlayerPrefs.SetInt("cosmetic_" + cosmetic.id, cosmetic.owned ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    void LoadOwnedCosmetics()
    {
        foreach (var cosmetic in availableCosmetics)
        {
            cosmetic.owned = PlayerPrefs.GetInt("cosmetic_" + cosmetic.id, 0) == 1;
        }
    }
}
