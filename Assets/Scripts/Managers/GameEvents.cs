using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName ="GameEvents", menuName = "Game/Events")]
public class GameEvents : ScriptableObject
{
    public Action OnGameBegan;
    public Action OnVictory;
    public Action OnLose;

    public Action OnCorrectGuess;
    public Action OnWrongGuess;

    public Action<bool> ToggleGameState;
    public Action<int> OnScoreUpdated;
    public Action<float> OnTimerUpdated;
    public Action<bool> IsGameDataAvailable;

    public Action<Vector2Int> OnGridSizeUpdated;
}
