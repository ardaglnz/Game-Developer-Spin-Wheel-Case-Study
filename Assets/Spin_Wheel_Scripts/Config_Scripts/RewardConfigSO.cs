// RewardConfigSO.cs
using UnityEngine;

[CreateAssetMenu(menuName = "WoF/RewardConfig")]
public class RewardConfigSO : ScriptableObject
{
    public string rewardId; // Uniiqe identifier for each reward
    public Sprite icon_value; // Reward's visual icon
    public int baseValue_value; // Amount of rewards
    public RewardType rewardType_value; // Categorization of the reward
}