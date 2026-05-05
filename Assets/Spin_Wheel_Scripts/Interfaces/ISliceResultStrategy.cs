public interface ISliceResultStrategy //Strategy Interface, implementable by future conditions
{
    bool CanHandle(SliceData result);
    void Execute(SliceData result, IZoneEvaluator zone);
}