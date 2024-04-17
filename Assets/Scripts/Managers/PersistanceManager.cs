using UnityEngine;

public class PersistanceManager
{
    public bool HasGame()
    {
        return PlayerPrefs.HasKey(Constants.Key);
    }

    public LevelStorage LoadGame()
    {
        if(HasGame())
        {
            return JsonUtility.FromJson<LevelStorage>(PlayerPrefs.GetString(Constants.Key));
        }
        return null;
    }

    public void SaveGame(
        int score,
        int pairStreak,
        int[] cardID,
        Vector3[] cardPositions,
        float gameTimer,
        float cardScale,
        bool isGameOngoing)
    {
        LevelStorage levelStorage = new LevelStorage();
        levelStorage.Score = score;
        levelStorage.PairStreak = pairStreak;
        levelStorage.CardID = cardID;
        levelStorage.CardPositions = cardPositions;
        levelStorage.CurrentTimer = gameTimer;
        levelStorage.IsGameOnGoing = isGameOngoing;
        levelStorage.CardScale = cardScale;

        PlayerPrefs.SetString(Constants.Key, JsonUtility.ToJson(levelStorage));
        PlayerPrefs.Save();
    }
}