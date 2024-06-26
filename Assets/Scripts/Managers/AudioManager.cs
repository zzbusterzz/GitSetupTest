using UnityEngine;

public partial class AudioManager : MonoBehaviour
{
    #region SERIALISED_FIELD
    [SerializeField]
    private GameEvents _gameEvents;

    [SerializeField]
    private AudioSource _cardFlip;

    [SerializeField]
    private AudioSource _bgMusic;

    [SerializeField]
    private AudioData _audioData;
    #endregion

    #region UNITY_FUNCTIONS
    private void Awake()
    {
        _gameEvents.OnGameBegan += OnGameBegan;
        _gameEvents.OnVictory += OnVictory;
        _gameEvents.OnLose += OnLose;
        _gameEvents.OnCorrectGuess += PlayCardPairSuccess;
        _gameEvents.OnWrongGuess += PlayCardPairedFail;
        Card.OnCardFlipBegan += PlayCardFlip;
    }

    private void OnDestroy()
    {
        _gameEvents.OnGameBegan -= OnGameBegan;
        _gameEvents.OnVictory -= OnVictory;
        _gameEvents.OnLose -= OnLose;
        _gameEvents.OnCorrectGuess -= PlayCardPairSuccess;
        _gameEvents.OnWrongGuess -= PlayCardPairedFail;
        Card.OnCardFlipBegan -= PlayCardFlip;
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    public void PlayCardFlip()
    {
        _cardFlip.PlayOneShot(_audioData.CardFlip);
    }

    public void PlayCardPairSuccess()
    {
        _cardFlip.PlayOneShot(_audioData.CardPairSuccess);
    }

    public void PlayCardPairedFail()
    {
        _cardFlip.PlayOneShot(_audioData.CardPairFail);
    }

    public void OnVictory()
    {
        PlayMusic(MusicType.WIN);
    }

    public void OnLose()
    {
        PlayMusic(MusicType.LOSE);
    }

    public void OnGameBegan()
    {
        PlayMusic(MusicType.GAME);
    }
    #endregion

    #region PRIVATE_FUNCTIONS
    private void PlayMusic(MusicType musicType)
    {
        if (_bgMusic.isPlaying)
        {
            _bgMusic.Stop();
        }
        MusicData data = _audioData.MusicData[(int)musicType];
        _bgMusic.clip = data.Clip;
        _bgMusic.loop = data.IsLoop;
        _bgMusic.Play();
    }
    #endregion
}