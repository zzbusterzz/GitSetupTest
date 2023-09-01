using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region SERIALISED_FIELDS
    [SerializeField]
    private GridManager _gridManager;

    [SerializeField]
    private Card _flippingCardPrefab;

    [SerializeField]
    private float _visibilityDuration = 4;
    [SerializeField]
    private float _gameTimer = 60;

    [SerializeField]
    private UnityEvent OnGameBegan;
    [SerializeField]
    private UnityEvent OnVictory;
    [SerializeField]
    private UnityEvent OnLose;

    [SerializeField]
    private UnityEvent OnCorrectGuess;
    [SerializeField]
    private UnityEvent OnWrongGuess;

    [SerializeField]
    private UnityEvent<int> OnScoreUpdated;
    [SerializeField]
    private UnityEvent<float> OnTimerUpdated;

    [SerializeField]
    private UnityEvent<bool> IsGameDataAvailable;

    #endregion

    #region PRIVATE_FIELDS
    private int _score = 0;
    private float _currentTimer = 0;
    private bool _isGameOnGoing = false;
    private int _pairStreak = 0;

    private List<Card> _activeCards;
    private List<Card> _inactiveCards;
    private int totalCards;

    private Card _previousClickedCard = null;
    private PersistanceManager _persistanceManager;
    #endregion

    #region UNITY_FUNCTIONS
    private void Awake()
    {
        _persistanceManager = new PersistanceManager();
        IsGameDataAvailable?.Invoke(_persistanceManager.HasGame());

        _activeCards = new List<Card>();
        _inactiveCards = new List<Card>();
    }

    private void Start()
    {
        Card.CurrentCardOpened += OnCardOpen;
        _gridManager.Init();
    }

    private void OnDestroy()
    {
        Card.CurrentCardOpened -= OnCardOpen;
    }

    private void Update()
    {
        if (_isGameOnGoing)
        {
            if(_currentTimer > 0)
            {
                _currentTimer -= Time.deltaTime;
                OnTimerUpdated?.Invoke(_currentTimer);
            }
            else
            {
                OnTimerUpdated?.Invoke(0);
                _isGameOnGoing = false;
                Loss();
            }
        }
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    /// <summary>
    /// Triggers new game
    /// </summary>
    public void NewGame()
    {
        totalCards = _gridManager.TotalItems;
        if (totalCards % 2 == 0)
        {
            ClearCurrentData();
            _gridManager.CreateGrid();
            GenerateCards();
            StartCoroutine(HideCardsAfterDelay());
            _currentTimer = _gameTimer;
            _pairStreak = 0;
            OnTimerUpdated?.Invoke(_currentTimer);
            OnGameBegan.Invoke();
        }
        else
        {
            Debug.LogError("Cannot play game as odd number of cards detected");
        }
    }

    /// <summary>
    /// Triggers Quit
    /// </summary>
    public void OnQuit()
    {
        Application.Quit();
    }
    #endregion

    #region PRIVATE_FUNCTIONS

    #region GAMEPLAY_FUNCTIONS
    /// <summary>
    /// Generates the card based on given grid size
    /// Randomly it will pick half the size of total req cards from deck of 52
    /// A random position will be chosen for two times from the generated grid vector positions
    /// Card will be placed from the position and id will be assigned
    /// </summary>
    private void GenerateCards()
    {
        int uniqueCardsToGen = totalCards / 2;

        HashSet<int> numbers = new HashSet<int>();

        while(numbers.Count < uniqueCardsToGen) 
        {
            numbers.Add(Random.Range(0, 52));
        }

        foreach (int number in numbers)
        {
            int xOffset = number % (Constants.MaxAllowedCards.x - 1);
            int yOffset = Mathf.FloorToInt(number / Constants.MaxAllowedCards.x);

            Vector3 pos = _gridManager.GetGridPosition();
            Card c = GetCardInstance(pos, _gridManager.Cardscale);
            c.SetOffset(xOffset, yOffset, number);
            _activeCards.Add(c);

            pos = _gridManager.GetGridPosition();
            c = GetCardInstance(pos, _gridManager.Cardscale);
            c.SetOffset(xOffset, yOffset, number);
            _activeCards.Add(c);
        }
    }

    /// <summary>
    /// Called when card animation of opening is complete
    /// Here check will be done if card is first or second
    /// if first it will wait for user to open next card
    /// if second it will compare to first and determine if its matching or not
    /// </summary>
    /// <param name="currCard"></param>
    private void OnCardOpen(Card currCard)
    {
        if (_previousClickedCard == null)
        {
            _previousClickedCard = currCard;
        }
        else
        {
            if (_previousClickedCard.CurentCardIndex == currCard.CurentCardIndex)
            {
                OnCorrectGuess?.Invoke();
                //Players score will keep on increasing as and when the streak continues
                //This will be the player bonus
                _score += 1 + _pairStreak;
                _pairStreak++;
                OnScoreUpdated?.Invoke(_score);

                ReturnAlongWithActiveCardList(currCard);
                ReturnAlongWithActiveCardList(_previousClickedCard);

                if (_activeCards.Count == 0)
                {
                    Victory();
                }
            }
            else
            {
                _pairStreak = 0;
                OnWrongGuess?.Invoke();
                _previousClickedCard.HideCard();
                currCard.HideCard();
            }
            _previousClickedCard = null;
        }
    }
    #endregion

    #region POOLING_FUNCTIONS
    private Card GetCardInstance(Vector3 pos, float scale)
    {
        Card c;
        if (_inactiveCards.Count > 0)
        {
            c = _inactiveCards[0];
            _inactiveCards.Remove(c);
            c.SetCard(pos, true, scale);
            return c;
        }

        c = GameObject.Instantiate(_flippingCardPrefab);
        c.SetCard(pos, true, scale);
        return c;
    }

    private void ReturnCard(Card card)
    {
        if (!_inactiveCards.Contains(card))
        {
            _inactiveCards.Add(card);
        }
        card.SetCard(Vector3.zero, false);
    }

    private void ReturnAlongWithActiveCardList(Card card)
    {
        ReturnCard(card);
        if (_activeCards.Contains(card))
        {
            _activeCards.Remove(card);
        }
    }
    #endregion

    #region MISC_FUNCTIONS
    /// <summary>
    /// This function wait for some time before hiding all generated cards and starts the timer
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideCardsAfterDelay()
    {
        float currentTime = 0;
        while(currentTime < _visibilityDuration)
        {
            yield return null;
            currentTime += Time.deltaTime;
        }

        ToggleCardReavealState(true);

        yield return new WaitForSeconds(Constants.FlipTime);

        _isGameOnGoing = true;
    }

    /// <summary>
    /// Toggles weather all cards are revealed or hidden
    /// </summary>
    /// <param name="hideCards"></param>
    private void ToggleCardReavealState(bool hideCards)
    {
        if (hideCards)
        {
            for (int i = 0; i < _activeCards.Count; i++)
            {
                _activeCards[i].HideCard();
            }
        }
        else
        {
            //We can use this option to reveal later on other cards as help to player
            //as sort of powerup to reveal present cards
            for (int i = 0; i < _activeCards.Count; i++)
            {
                _activeCards[i].RevealCard();
            }
        }
    }

    /// <summary>
    /// Triggers victory event
    /// </summary>
    private void Victory()
    {
        OnVictory.Invoke();
    }

    /// <summary>
    /// Triggers lose event
    /// </summary>
    private void Loss()
    {
        ClearActiveCards();
        OnLose.Invoke();
    }

    private void ClearActiveCards()
    {
        for (int i = 0; i < _activeCards.Count; i++)
        {
            _activeCards[i].StopAllCoroutines();
            ReturnCard(_activeCards[i]);
        }
        _activeCards.Clear();
    }

    private void ClearCurrentData()
    {
        if (_isGameOnGoing)
        {
            _isGameOnGoing = false;
            _currentTimer = 0;
            ClearActiveCards();
        }
    }

    #endregion

    #region PERSISTANCY
    public void SaveGame()
    {
        int[] cardId = new int[_activeCards.Count];
        Vector3[] cardPos = new Vector3[_activeCards.Count];

        for(int i = 0; i < _activeCards.Count; i++)
        {
            cardId[i] = _activeCards[i].CurentCardIndex;
            cardPos[i] = _activeCards[i].CachedTransform.position;
        }

        _persistanceManager.SaveGame(_score,
                                     _pairStreak,
                                     cardId,
                                     cardPos,
                                     _currentTimer,
                                     _gridManager.Cardscale,
                                     _isGameOnGoing);

        IsGameDataAvailable?.Invoke(true);
    }

    public void LoadGame()
    {
        ClearCurrentData();
        if (_persistanceManager.HasGame())
        {
            LevelStorage levelStorage = _persistanceManager.LoadGame();
            _gridManager.Cardscale = levelStorage.CardScale;

            for (int i = 0; i < levelStorage.CardID.Length; i++)
            {
                int xOffset = levelStorage.CardID[i] % (Constants.MaxAllowedCards.x - 1);
                int yOffset = Mathf.FloorToInt(levelStorage.CardID[i] / Constants.MaxAllowedCards.x);

                Card c = GetCardInstance(levelStorage.CardPositions[i], _gridManager.Cardscale);
                c.SetOffset(xOffset, yOffset, levelStorage.CardID[i]);
                _activeCards.Add(c);
                c.HideCardInstant();
            }

            _score = levelStorage.Score;
            _pairStreak = levelStorage.PairStreak;
            _currentTimer = levelStorage.CurrentTimer;
            _isGameOnGoing = levelStorage.IsGameOnGoing;
            OnScoreUpdated?.Invoke(_score);
            OnGameBegan.Invoke();
        }

    }
    #endregion
    #endregion
}