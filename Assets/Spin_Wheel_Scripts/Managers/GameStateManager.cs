using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // Event for state changes
    public static event Action<GameState> OnStateChanged;

    public GameState CurrentState { get; private set; } = GameState.Idle;

    private void OnEnable()
    {
        GameSignals.OnRevived += HandleRevive;
        GameSignals.OnGameRestarted += RestartGame; 
    }

    private void OnDisable()
    {
        GameSignals.OnRevived -= HandleRevive;
        GameSignals.OnGameRestarted -= RestartGame;
    }

    public void SetState(GameState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);

        // Sets the current state of the game
    }

    // The game goes Idle for both Revive and Restart
    private void HandleRevive() => SetState(GameState.Idle); 

    private void RestartGame() 
    {
        CurrentState = GameState.Idle;
        OnStateChanged?.Invoke(CurrentState);
    }
}