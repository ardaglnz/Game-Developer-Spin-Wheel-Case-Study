using UnityEngine;
using System.Linq;

public class TransactionService : MonoBehaviour
{
    [SerializeField] private InventoryService _inventory;
    [SerializeField] private EconomyService _economy;


    // Adds the loot safely to the inventory when the player gives up
    public void CommitLoot()
    {
        var items = _inventory.GetAll();
        foreach (var item in items)
        {
            if (item.Type == RewardType.Currency)
                _economy.ProcessReward(item);
        }
        _inventory.Clear();
        GameSignals.OnGameRestarted?.Invoke();
    }

    public void AbandonLoot()
    {
        LogTransaction(false);
        _inventory.Clear();
        GameSignals.OnGameRestarted?.Invoke();
    }

    private void LogTransaction(bool isSuccess)
    {
        string status = isSuccess ? "SUCCESSFUL CLAIM" : "LOOT LOST";
        Debug.Log($"<color=white>--- {status} ---</color>");
        foreach (var item in _inventory.GetAll())
        {
            Debug.Log($"• {item.RewardId}: x{item.Amount}");
        }
    }
}