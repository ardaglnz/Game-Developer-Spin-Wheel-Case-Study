// SliceData.cs
using System;

[Serializable]
public class SliceData
{
    public SliceType SliceType;         // Reward or bomb
    public RewardConfigSO RewardConfig; // Reward's config
    public float Weight;                // For weighted random selection on BaseWheel

    // Resolving the result type of the wheel, applying strategies
    public RewardData Resolve(int currentZone)
    {
        if (SliceType == SliceType.Bomb)
            return null;

        return RewardData.FromConfig(RewardConfig, currentZone);
    }

    public bool IsBomb => SliceType == SliceType.Bomb;
}