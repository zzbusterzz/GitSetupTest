using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Game/AudioData", order = 1)]
public class AudioData : ScriptableObject
{
    //Music data currently corresponds to each music type attribute
    //So 0 index we will store Win
    //1 index we will store Lose
    //2 index we will store Game

    public MusicData[] MusicData;

    public AudioClip CardFlip;
    public AudioClip CardPairSuccess;
    public AudioClip CardPairFail;

}