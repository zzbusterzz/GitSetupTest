using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public static Action OnCardFlipBegan;
    public static Action<Card> CurrentCardOpened;

    [SerializeField]
    private Vector2 _materialTexOffset;

    [SerializeField]
    private Renderer _meshRenderer;

    //private MaterialPropertyBlock matBlock;
    private int _curentCardIndex = -1;
    private bool _isHidden = false;
    private Coroutine _animCoroutine = null;
    private Transform _transform;

    public int CurentCardIndex { get => _curentCardIndex; }
    public bool IsAnimating { get => _animCoroutine != null; }

    private void Awake()
    {
        //matBlock = new MaterialPropertyBlock();
        _transform = transform;
    }

    public void SetOffset(int xOffset, int yOffset)
    {
        _curentCardIndex = (Constants.MaxAllowedCards.x * xOffset) + (Constants.MaxAllowedCards.y * yOffset);
        //matBlock.SetVector(_matTexStr, new Vector2(
        //    _materialTexOffset.x * xOffset,
        //    _materialTexOffset.y * yOffset));
        //_meshRenderer.SetPropertyBlock(matBlock); //Seems to be some issue in batching zzz

        _meshRenderer.material.SetTextureOffset(Constants.MatTexStr, new Vector2(
            _materialTexOffset.x * xOffset,
            _materialTexOffset.y * yOffset));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isHidden && _animCoroutine == null)
        {
            OnCardFlipBegan?.Invoke();
            _animCoroutine = StartCoroutine(AnimCoroutine(Constants.QInvertedRot, Constants.QRevealRot, Card.CurrentCardOpened));
            _isHidden = false;
        }
    }

    private IEnumerator AnimCoroutine(Quaternion from, Quaternion to, Action<Card> onAnimComplete = null)
    {
        float curTime = 0;

        while(curTime < Constants.FlipTime)
        {
            _transform.rotation = Quaternion.Slerp(from, to, curTime / Constants.FlipTime);
            curTime += Time.deltaTime;
            yield return null;
        }

        _animCoroutine = null;
        onAnimComplete?.Invoke(this);
    }

    public void RevealCard()
    {
        if (_isHidden)
        {
            _animCoroutine = StartCoroutine(AnimCoroutine(Constants.QInvertedRot, Constants.QRevealRot));
            _isHidden = false;
        }
    }

    public void HideCard()
    {
        if (!_isHidden)
        {
            OnCardFlipBegan?.Invoke();
            _animCoroutine = StartCoroutine(AnimCoroutine(_transform.rotation, Constants.QInvertedRot));
            _isHidden = true;
        }
    }
}
