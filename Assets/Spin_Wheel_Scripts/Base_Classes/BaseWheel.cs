using UnityEngine;

public abstract class BaseWheel : MonoBehaviour, ISpinnable // Abstract class for the wheels
{
    [SerializeField] protected WheelConfigSO _config;
    [SerializeField] protected SpinService _spinService;

    protected bool _isSpinning;
    private int _pendingResultIndex;
    protected abstract WheelType Type { get; }

    protected virtual void OnValidate()
    {
        if (_spinService == null)
        {
            _spinService = FindObjectOfType<SpinService>();
        }
    }

    public virtual void Spin() // Initiates the spin process, triggered by SpinService
    {
        if (_isSpinning) return;
        _isSpinning = true;

        SliceData result = PickResult();
        _pendingResultIndex = _config.slices_value.IndexOf(result);

        if (_pendingResultIndex == -1)
        {
            Debug.LogError("[WHEEL] The choosen reward cannot be found in the reward list!");
            _pendingResultIndex = 0; // Fallback index
        }

        WheelSpinEvent.OnSpinRequested?.Invoke(Type, _pendingResultIndex, OnAnimationComplete);
    }

    public virtual void OnSpinResult(SliceData result)
    {
        WheelSpinEvent.OnSpinResultReady?.Invoke(result);
    }
    protected void OnAnimationComplete() // Triggered by the when the visual spinning sequence has ended
    {
        _isSpinning = false;
        FinishSpin(); 
    }

    protected void FinishSpin()
    {
        SliceData finalResult = _config.slices_value[_pendingResultIndex];
        WheelSpinEvent.OnSpinResultReady?.Invoke(finalResult);
    }


    protected virtual SliceData PickResult() // Precalculates the winning slice based on a weighted random system
    {
        float totalWeight = 0f;
        foreach (var s in _config.slices_value) totalWeight += s.Weight;

        float roll = Random.value * totalWeight;
        float cumulative = 0f;

        foreach (var slice in _config.slices_value)
        {
            cumulative += slice.Weight;
            if (roll <= cumulative) return slice;
        }

        return _config.slices_value[^1];
    }
}