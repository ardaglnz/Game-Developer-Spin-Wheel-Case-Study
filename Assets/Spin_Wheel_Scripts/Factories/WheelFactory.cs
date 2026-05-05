using UnityEngine;

// Factory method that decides which wheel will spin on which zone
public class WheelFactory : MonoBehaviour
{
    [Header("Wheel Implementations")]
    [SerializeField] private BaseWheel _normalWheel;
    [SerializeField] private BaseWheel _silverWheel;
    [SerializeField] private BaseWheel _goldenWheel;

    [Header("Config")]
    [SerializeField] private ZoneConfigSO _zoneConfig;

    // Returns BaseWheel reference according to the zone index
    public BaseWheel GetWheel(int currentZone)
    {
        // Super Zone control
        if (currentZone % _zoneConfig.superZoneInterval_value == 0)
        {
            Debug.Log($"<color=yellow>[FACTORY]</color> Zone {currentZone}: Golden Wheel seçildi.");
            return _goldenWheel;
        }

        // Safe Zone control
        if (currentZone % _zoneConfig.safeZoneInterval_value == 0)
        {
            Debug.Log($"<color=silver>[FACTORY]</color> Zone {currentZone}: Silver Wheel seçildi.");
            return _silverWheel;
        }

        // All others are normal
        return _normalWheel;
    }
}