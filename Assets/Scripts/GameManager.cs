using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SERIALISED_FIELDS
    [SerializeField]
    private Vector2Int _gridSize;

    [SerializeField]
    private Vector2 _gridDimensionsHalf;

    [SerializeField]
    private Card _flippingCardPrefab;

    [SerializeField]
    private Transform _gridStart;

    [SerializeField]
    private float _visibilityDuration = 4;
    #endregion

    #region PRIVATE_FIELDS
    private int _score = 0;
    private List<Vector3> _gridPosition;
    private List<Card> _activeCards;
    private List<Card> _inactiveCards;
    private int totalCards;

    private Card _previousClickedCard = null;
    #endregion

    #region UNITY_FUNCTIONS
    private void Start()
    {
        _gridPosition = new List<Vector3>();
        _activeCards = new List<Card>();
        _inactiveCards = new List<Card>();

        Card.CurrentCardOpened += OnCardOpen;

        NewGame();
    }

    private void OnDestroy()
    {
        Card.CurrentCardOpened -= OnCardOpen;
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    public void NewGame()
    {
        totalCards = _gridSize.x * _gridSize.y;
        if (totalCards % 2 == 0)
        {
            CreateGrid();
            GenerateCards();
            StartCoroutine(HideCardsAfterDelay());
        }
    }

    public void Restart()
    {

    }
    #endregion

    #region PRIVATE_FUNCTIONS
    private void CreateGrid()
    {
        Vector3 gridStartVector = _gridStart.position + new Vector3(_gridDimensionsHalf.x, -_gridDimensionsHalf.y, 0);
        float dimWidth = 2 * _gridDimensionsHalf.x;
        float dimHeight = 2 * _gridDimensionsHalf.y;

        for (int i = 0; i < _gridSize.x; i++)
        {
            for(int j = 0; j < _gridSize.y; j++)
            {
                _gridPosition.Add(new Vector3(
                    gridStartVector.x + (i * dimWidth),
                    gridStartVector.y - (j * dimHeight),
                    0));
            }
        }
    }

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
            Vector3 pos = GetGridPosition();
            Card c = GetCardInstance(pos);
            c.SetOffset(number % (Constants.MaxAllowedCards.x - 1), Mathf.FloorToInt(number / Constants.MaxAllowedCards.x));
            _activeCards.Add(c);

            pos = GetGridPosition();
            c = GetCardInstance(pos);
            c.SetOffset(number % (Constants.MaxAllowedCards.x - 1), Mathf.FloorToInt(number / Constants.MaxAllowedCards.x));
            _activeCards.Add(c);
        }
    }

    private Card GetCardInstance(Vector3 pos)
    {
        return GameObject.Instantiate(_flippingCardPrefab, pos, Quaternion.identity);
    }

    private Vector3 GetGridPosition()
    {
        int gridIndex = Random.Range(0, _gridPosition.Count);
        Vector3 pos = _gridPosition[gridIndex];
        _gridPosition.Remove(pos);
        return pos;
    }

    private IEnumerator HideCardsAfterDelay()
    {
        float currentTime = 0;
        while(currentTime < _visibilityDuration)
        {
            yield return null;
            currentTime++;
        }

        ToggleCardVisibility(true);
    }

    private void ToggleCardVisibility(bool hideCards)
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
            for (int i = 0; i < _activeCards.Count; i++)
            {
                _activeCards[i].RevealCard();
            }
        }
    }

    private void OnCardOpen(Card currCard)
    {
        if(_previousClickedCard == null)
        {
            _previousClickedCard = currCard;
        }
        else
        {
            if(_previousClickedCard.CurentCardIndex == currCard.CurentCardIndex)
            {
                _score++;
                //Play clear audio

                _previousClickedCard.gameObject.SetActive(false);
                currCard.gameObject.SetActive(false);

                _activeCards.Remove(currCard);
                _activeCards.Remove(_previousClickedCard);
                
                _inactiveCards.Add(currCard);
                _inactiveCards.Add(_previousClickedCard);
            }
            else
            {
                _previousClickedCard.HideCard();
                currCard.HideCard();
            }
            _previousClickedCard = null;
        }
    }

    void CheckForVictory()
    {

    }
    #endregion
}
