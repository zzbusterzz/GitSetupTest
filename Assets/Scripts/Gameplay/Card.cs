using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    #region PUBLIC_ACTION_FIELDS
    public static Action OnCardFlipBegan;
    public static Action<Card> CurrentCardOpened;
    #endregion

    #region SERIALISED_FIELDS
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    #endregion

    #region PRIVATE_FIELDS
    private int _curentCardIndex = -1;
    private bool _isHidden = false;
    private Coroutine _animCoroutine = null;
    private Transform _cachedTransform;
    private GameObject _cachedGO;
    private Vector3 _cachedOrigScale;
    #endregion

    #region GETTERS
    public int CurentCardIndex { get => _curentCardIndex; }
    public bool IsAnimating { get => _animCoroutine != null; }
    public Transform CachedTransform { get => _cachedTransform; }
    #endregion

    #region UNITY_FUNCTIONS
    private void Awake()
    {
        _cachedTransform = transform;
        _cachedGO = gameObject;
        _cachedOrigScale = transform.localScale;
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    public void Despawn()
    {
        if(_animCoroutine != null)
        {
            StopCoroutine(_animCoroutine);
            _animCoroutine = null;
        }
        _cachedGO.SetActive(false);
        _isHidden = false;
        _curentCardIndex = -1;
    }

    public void SetOffset(Sprite sprite, int index)
    {
        _curentCardIndex = index;
        _spriteRenderer.sprite = sprite;
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
            _animCoroutine = StartCoroutine(AnimCoroutine(_cachedTransform.rotation, Constants.QInvertedRot));
            _isHidden = true;
        }
    }

    public void HideCardInstant()
    {
        _isHidden = true;
        _cachedTransform.rotation = Constants.QInvertedRot;
    }

    public void SetCard(Vector3 pos, bool visibility, float scale = 1)
    {
        _cachedGO.SetActive(visibility);
        _cachedTransform.position = pos;
        _cachedTransform.rotation = Quaternion.identity;
        _cachedTransform.localScale = visibility ? new Vector3(_cachedOrigScale.x * scale, _cachedOrigScale.y * scale, _cachedOrigScale.z) :
            Vector3.one;
    }
    #endregion

    #region PRIVATE_FUNCTIONS
    private IEnumerator AnimCoroutine(Quaternion from, Quaternion to, Action<Card> onAnimComplete = null)
    {
        float curTime = 0;

        while (curTime < Constants.FlipTime)
        {
            _cachedTransform.rotation = Quaternion.Slerp(from, to, curTime / Constants.FlipTime);
            curTime += Time.deltaTime;
            yield return null;
        }

        _animCoroutine = null;
        onAnimComplete?.Invoke(this);
    }
    #endregion

}