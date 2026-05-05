using System;
using UnityEngine;

[Serializable]
public class RewardData
{
    //Explanations for RewardConfigSO

    public string RewardId;
    public RewardType Type;
    public int Amount;
    public Sprite Icon;

    // Constructor-Clone method
    public RewardData(RewardData other)
    {
        this.RewardId = other.RewardId;
        this.Type = other.Type;
        this.Amount = other.Amount;
        this.Icon = other.Icon;
    }

    // Parameterized constructor (For creating SliceData)
    public RewardData() { }

    public static RewardData FromConfig(RewardConfigSO config, int currentZone)
    {
        return new RewardData
        {
            RewardId = config.rewardId,
            Type = config.rewardType_value,
            Amount = config.baseValue_value * currentZone,
            Icon = config.icon_value
        };
    }
}