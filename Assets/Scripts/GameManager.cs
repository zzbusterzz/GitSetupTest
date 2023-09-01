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
    #endregion

    #region PRIVATE_FIELDS
    private float _visibilityDuration = 4;
    private int _score = 0;
    private List<Vector3> _gridPosition;
    private List<Card> _activeCards;
    #endregion


    private void Start()
    {
        _gridPosition = new List<Vector3>();
        _activeCards = new List<Card>();
        NewGame();
    }

    public void NewGame()
    {
        CreateGrid();
        GenerateCards();
    }

    public void Restart()
    {

    }

    void CreateGrid()
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

    void GenerateCards()
    {
        int totalCards = _gridSize.x * _gridSize.y;

        if( totalCards % 2 == 0 ) 
        {
            int uniqueCardsToGen = totalCards / 2;

            Vector2Int _maxAllowedCards = new Vector2Int(13, 4);

            HashSet<int> numbers = new HashSet<int>();

            while(numbers.Count < uniqueCardsToGen) 
            {
                numbers.Add(Random.Range(0, 52));
            }

            foreach (int number in numbers)
            {
                Vector3 pos = GetGridPosition();
                Card c = GetCardInstance(pos);
                c.SetOffset(number % (_maxAllowedCards.x - 1), Mathf.FloorToInt(number / _maxAllowedCards.x));
                _activeCards.Add(c);

                pos = GetGridPosition();
                c = GetCardInstance(pos);
                c.SetOffset(number % (_maxAllowedCards.x - 1), Mathf.FloorToInt(number / _maxAllowedCards.x));
                _activeCards.Add(c);
            }
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
}
