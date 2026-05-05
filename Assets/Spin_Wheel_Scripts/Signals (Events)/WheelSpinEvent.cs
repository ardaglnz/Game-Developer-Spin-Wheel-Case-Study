using System;

public static class WheelSpinEvent 
{
    // Wheel type, target slice index, and completion callback
    public static Action<WheelType, int, Action> OnSpinRequested;
    public static Action<SliceData> OnSpinResultReady;
}