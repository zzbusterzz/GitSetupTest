using System.Collections.Generic;
using UnityEngine;

public class CardManager
{
    #region PRIVATE_FIELDS
    private List<Card> _activeCards;
    private List<Card> _inactiveCards;
    private Card _flippingCardPrefab;
    #endregion

    #region PUBLIC_GETTERS
    public int ActiveCardCount { get=>  _activeCards.Count;}
    #endregion

    #region PUBLIC_FUNCTIONS
    public void Init(Card cardPrefab)
    {
        _flippingCardPrefab = cardPrefab;
        _activeCards = new List<Card>();
        _inactiveCards = new List<Card>();
    }

    public void GetCardIdAndPos(out int[] ids, out Vector3[] poss)
    {
        int activeCardCount = _activeCards.Count;
        ids = new int[activeCardCount];
        poss = new Vector3[activeCardCount];

        for (int i = 0; i < activeCardCount; i++)
        {
            ids[i] = _activeCards[i].CurentCardIndex;
            poss[i] = _activeCards[i].CachedTransform.position;
        }
    }

    public void ClearActiveCards()
    {
        for (int i = 0; i < _activeCards.Count; i++)
        {
            _activeCards[i].Despawn();
            ReturnCard(_activeCards[i]);
        }
        _activeCards.Clear();
    }

    /// <summary>
    /// Toggles weather all cards are revealed or hidden
    /// </summary>
    /// <param name="hideCards"></param>
    public void ToggleCardReavealState(bool hideCards)
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
    #endregion

    #region POOLING_FUNCTIONS
    public Card GetCardInstance(Vector3 pos, float scale)
    {
        Card c;
        if (_inactiveCards.Count > 0)
        {
            c = _inactiveCards[0];
            _inactiveCards.Remove(c);
            c.SetCard(pos, true, scale);
            _activeCards.Add(c);
            return c;
        }

        c = GameObject.Instantiate(_flippingCardPrefab);
        c.SetCard(pos, true, scale);
        _activeCards.Add(c);
        return c;
    }

    public void ReturnCard(Card card)
    {
        if (!_inactiveCards.Contains(card))
        {
            _inactiveCards.Add(card);
        }
        card.SetCard(Vector3.zero, false);
    }

    public void ReturnAlongWithActiveCardList(Card card)
    {
        ReturnCard(card);
        if (_activeCards.Contains(card))
        {
            _activeCards.Remove(card);
        }
    }
    #endregion
}
