using UnityEngine;

public class RewardResultStrategy : MonoBehaviour, ISliceResultStrategy // Strategy for the Rewaard outcome on the wheel
{
    [SerializeField] private InventoryService _inventory;
    [SerializeField] private ZoneManager _zoneManager;

    public bool CanHandle(SliceData result) => !result.IsBomb;

    public void Execute(SliceData result, IZoneEvaluator zone)
    {
        // Save the zone before the Advance call
        int zoneAtSpinTime = zone.CurrentZone;
        _zoneManager.Advance();

        if (result.RewardConfig != null)
        {
            RewardData reward = result.Resolve(zoneAtSpinTime);
            _inventory.Add(reward);
        }
    }
}