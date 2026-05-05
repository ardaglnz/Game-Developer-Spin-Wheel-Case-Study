using UnityEngine;

public class EconomyService : MonoBehaviour
{
    public static EconomyService Instance { get; private set; }

    private const string GOLD_KEY = "TotalGold"; // Saving player's gold as a PlayerPref
    private int _totalGold;

    public int GetBalance() => _totalGold;

    private void Awake()
    {
        if (Instance == null) Instance = this;

        // If there is no PlayerPref saved, start from 9999
        if (!PlayerPrefs.HasKey(GOLD_KEY))
        {
            _totalGold = 9999;
            PlayerPrefs.SetInt(GOLD_KEY, _totalGold);
            PlayerPrefs.Save();
        }
        else
        {
            _totalGold = PlayerPrefs.GetInt(GOLD_KEY);
        }
    }

    public void ProcessReward(RewardData reward) // Processing the rewards (loot)
    {
        if (reward.Type != RewardType.Currency) return;

        if (reward.RewardId.ToLower() == "gold")
        {
            _totalGold += reward.Amount;
            PlayerPrefs.SetInt(GOLD_KEY, _totalGold);
            PlayerPrefs.Save();
            GameSignals.OnBalanceChanged?.Invoke(_totalGold);
        }
    }

    // BombPopupView's gold spending attempt (for reviving)
    public bool TrySpendGold(int amount)
    {
        if (_totalGold < amount) return false;

        _totalGold -= amount;
        PlayerPrefs.SetInt(GOLD_KEY, _totalGold);
        PlayerPrefs.Save();
        GameSignals.OnBalanceChanged?.Invoke(_totalGold);
        return true;
    }
}