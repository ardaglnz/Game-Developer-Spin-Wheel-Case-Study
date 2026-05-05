// ZoneConfigSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WoF/ZoneConfig")]
public class ZoneConfigSO : ScriptableObject
{
    public int totalZones; // Total number of levels
    public int safeZoneInterval_value = 5; // Silver Wheel
    public int superZoneInterval_value = 30; // Gold Wheel
    public List<WheelConfigSO> zoneWheels_value; // Wheel setups from Wheel SO's
}