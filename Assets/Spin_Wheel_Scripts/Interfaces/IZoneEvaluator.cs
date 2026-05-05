public interface IZoneEvaluator // Interface for evaluating current and future zones
{
    int CurrentZone { get; }
    bool IsSafeZone();   // zone % 5 == 0
    bool IsSuperZone();  // zone % 30 == 0
}