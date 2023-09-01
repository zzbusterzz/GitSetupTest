using UnityEngine;

[System.Serializable]
public class LevelStorage
{
    public int Score;
    public int PairStreak;
    public int[] CardID;
    public Vector3[] CardPositions;
    public float CurrentTimer;
    public float CardScale;
    public bool IsGameOnGoing;
}
