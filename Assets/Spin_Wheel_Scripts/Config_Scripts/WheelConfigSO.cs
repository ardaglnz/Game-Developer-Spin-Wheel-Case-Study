// WheelConfigSO.cs
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WoF/WheelConfig")]
public class WheelConfigSO : ScriptableObject
{
    public WheelType wheelType; // Bronze, Gold, Silver
    public List<SliceData> slices_value; // Each slice's value changeable from Reward SO's, each slice is a Reward
}