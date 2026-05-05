using UnityEngine;
using UnityEngine.Timeline;

public class WheelUIBuilder : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private WheelThemeManager _themeManager;
    [SerializeField] private WheelProgressView _progressHandler;

    [Header("System Connections")]
    [SerializeField] private ZoneManager _zoneManager;
    [SerializeField] private ZoneConfigSO _zoneConfig;

    [Header("Slices")]
    [SerializeField] private WheelSliceUI[] _slices;

    private void OnEnable()
    {
        ZoneManager.OnZoneAdvanced += UpdateWheelUI;
        if (_zoneManager != null) UpdateWheelUI(_zoneManager.CurrentZone);
    }

    private void OnDisable() => ZoneManager.OnZoneAdvanced -= UpdateWheelUI;


    // Master method for rebuilding UI, triggered every time the player advances to a new zone
    private void UpdateWheelUI(int currentZone)
    {
        int safeInt = _zoneConfig.safeZoneInterval_value;
        int superInt = _zoneConfig.superZoneInterval_value;

        // Theme select and apply
        var theme = _themeManager.GetTheme(currentZone, safeInt, superInt);
        _themeManager.ApplyTheme(theme);

        // Progress and multiplier info
        _progressHandler.UpdateMultiplier(currentZone);
        _progressHandler.UpdateProgress(currentZone, safeInt, superInt,
            _themeManager.GetTheme(safeInt, safeInt, superInt).themeColor,
            _themeManager.GetTheme(superInt, safeInt, superInt).themeColor);

        // Config selection and slice building
        WheelConfigSO activeConfig = GetActiveConfig(currentZone, safeInt, superInt);
        BuildSlices(activeConfig, currentZone);
    }

    private WheelConfigSO GetActiveConfig(int zone, int safeInt, int superInt)
    {
        if (zone % superInt == 0) return _zoneConfig.zoneWheels_value[0];
        if (zone % safeInt == 0) return _zoneConfig.zoneWheels_value[2];
        return _zoneConfig.zoneWheels_value[1];
    }

    private void BuildSlices(WheelConfigSO config, int zone)
    {
        if (config == null || config.slices_value.Count != _slices.Length) return;
        for (int i = 0; i < _slices.Length; i++)
        {
            _slices[i].Setup(config.slices_value[i], zone);
        }
    }
}