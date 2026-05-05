using UnityEngine;

public class BombResultStrategy : MonoBehaviour, ISliceResultStrategy // Strategy for the Bomb outcome on the wheel
{
    [SerializeField] private GameStateManager _stateManager;

    public bool CanHandle(SliceData result) => result.IsBomb;

    public void Execute(SliceData result, IZoneEvaluator zone)
    {
        _stateManager.SetState(GameState.BombResult);
        GameSignals.OnBombExploded?.Invoke();
    }
}