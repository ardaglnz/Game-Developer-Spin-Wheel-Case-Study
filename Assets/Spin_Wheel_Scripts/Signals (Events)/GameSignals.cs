using System.Collections.Generic;

public static class GameSignals //Events (communication) for the game cycle
{
    public static System.Action OnSpinRequested;
    public static System.Action<RewardData> OnRewardCollected;
    public static System.Action OnBombExploded;
    public static System.Action OnRevived;
    public static System.Action OnGameRestarted;
    public static System.Action<int> OnBalanceChanged;
}