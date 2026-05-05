using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryService : MonoBehaviour
{
    public static InventoryService Instance { get; private set; } // Singleton

    private readonly Dictionary<string, RewardData> _items = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void Add(RewardData data) // Collecting rewards
    {
        if (_items.ContainsKey(data.RewardId))
            _items[data.RewardId].Amount += data.Amount;
        else
            _items.Add(data.RewardId, new RewardData(data));

        GameSignals.OnRewardCollected?.Invoke(data);
    }

    public void Clear() => _items.Clear();
    public List<RewardData> GetAll() => _items.Values.ToList();
}