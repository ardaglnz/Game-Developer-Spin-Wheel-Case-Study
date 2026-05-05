using System;
using UnityEngine;

public class ZoneManager : MonoBehaviour, IZoneEvaluator
{
    public static event Action<int> OnZoneAdvanced;

    [SerializeField] private ZoneConfigSO _config;
    public int CurrentZone { get; private set; } = 1;

    private void OnEnable() => GameSignals.OnGameRestarted += ResetToFirstZone;
    private void OnDisable() => GameSignals.OnGameRestarted -= ResetToFirstZone;

    public bool IsSafeZone() => CurrentZone % _config.safeZoneInterval_value == 0;
    public bool IsSuperZone() => CurrentZone % _config.superZoneInterval_value == 0;

    public void Advance()
    {

        if (_config == null)
        {
            return;
        }

        Debug.Log($"<color=orange>[ZONE]</color> Current zone: {CurrentZone}, Total zones: {_config.totalZones}");

        if (CurrentZone >= _config.totalZones)
        {
            Debug.LogWarning("[ZONE] Reached num. of maximum zones.");
            return;
        }

        CurrentZone++;

        Debug.Log("<color=cyan>[ZONE]</color> Event throwing...");

        try
        {
            OnZoneAdvanced?.Invoke(CurrentZone);
            Debug.Log("<color=green>[ZONE]</color> Event throwed.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"<color=red>[HATA]</color> UI Error while throwing an error: {e.Message}");
        }
    }

    public void ResetToFirstZone()
    {
        CurrentZone = 1;
        OnZoneAdvanced?.Invoke(CurrentZone);
    }
}