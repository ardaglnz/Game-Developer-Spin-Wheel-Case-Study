using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpinService : MonoBehaviour
{
    [SerializeField] private WheelFactory _wheelFactory;
    [SerializeField] private ZoneManager _zoneManager;
    [SerializeField] private GameStateManager _stateManager;
    [SerializeField] private List<MonoBehaviour> _strategyObjects;

    private List<ISliceResultStrategy> _strategies;

    private void Awake()
    {
        _strategies = _strategyObjects
            .OfType<ISliceResultStrategy>()
            .ToList();

        if (_strategies.Count == 0)
            Debug.LogError("[SPIN SERVICE] No strategy has found!");
    }

    private void OnEnable()
    {
        GameSignals.OnSpinRequested += ExecuteSpin;
        GameSignals.OnRevived += ResetServiceStatus;
        WheelSpinEvent.OnSpinResultReady += HandleSpinResult; 
    }

    private void OnDisable()
    {
        GameSignals.OnSpinRequested -= ExecuteSpin;
        GameSignals.OnRevived -= ResetServiceStatus;
        WheelSpinEvent.OnSpinResultReady -= HandleSpinResult; 
    }

    private void ResetServiceStatus() => 
        _stateManager.SetState(GameState.Idle);

    private void ExecuteSpin() // Wheel spin order
    {
        if (_stateManager.CurrentState != GameState.Idle) return;
        _stateManager.SetState(GameState.Spinning);
        _wheelFactory.GetWheel(_zoneManager.CurrentZone).Spin();
    }

    public void HandleSpinResult(SliceData result) // Handles the outcome of the wheel spin order
    {
        if (result == null) return;

        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(result));

        if (strategy == null)
        {
            Debug.LogError($"[SPIN SERVICE] '{result.SliceType}' strategy couldn't found for the slice type!");
            _stateManager.SetState(GameState.Idle);
            return;
        }

        strategy.Execute(result, _zoneManager);

        // Bomb state is setted on the strategy, for the other conditions return to the idle
        if (_stateManager.CurrentState != GameState.BombResult)
            _stateManager.SetState(GameState.Idle);
    }
}