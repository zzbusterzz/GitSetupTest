using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerClickHandler
{
    private const string _matTexStr = "_CardFront";
    private static Vector2Int _maxAllowedCards = new Vector2Int(13, 4);
    private const float _flipTime = 1;

    [SerializeField]
    private Vector2 _materialTexOffset;

    [SerializeField]
    private Renderer _meshRenderer;

    private MaterialPropertyBlock matBlock;
    private int _curentCardIndex = -1;
    private bool isHidden = false;
    private Coroutine animCoroutine = null;
    private Transform _transform;

    //TODO: move to scriptable objects
    private Vector3 _invertedRot = new Vector3(0, 180, 0);
    private Vector3 _revealRot = new Vector3(0, 0, 0);
    private Quaternion _qInvertedRot;
    private Quaternion _qRevealRot;


    private void Awake()
    {
        matBlock = new MaterialPropertyBlock();
        _transform = transform;
        _qInvertedRot = Quaternion.Euler(_invertedRot);
        _qRevealRot = Quaternion.Euler(_revealRot);
    }

    public void SetOffset(int xOffset, int yOffset)
    {
        _curentCardIndex = (_maxAllowedCards.x * xOffset) + (_maxAllowedCards.y * yOffset);
        //matBlock.SetVector(_matTexStr, new Vector2(
        //    _materialTexOffset.x * xOffset,
        //    _materialTexOffset.y * yOffset));
        //_meshRenderer.SetPropertyBlock(matBlock); //Seems to be some issue in batching zzz

        _meshRenderer.sharedMaterial.SetTextureOffset(_matTexStr, new Vector2(
            _materialTexOffset.x * xOffset,
            _materialTexOffset.y * yOffset));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Clicked {_curentCardIndex}");

        if(isHidden && animCoroutine == null)
        {
            animCoroutine = StartCoroutine(AnimCoroutine(_qInvertedRot, _qRevealRot));
            isHidden = false;
        }
    }

    private IEnumerator AnimCoroutine(Quaternion from, Quaternion to)
    {
        float curTime = 0;

        while(curTime < _flipTime)
        {
            _transform.rotation = Quaternion.Slerp(from, to, curTime / _flipTime);
            curTime += Time.deltaTime;
            yield return null;
        }

        animCoroutine = null;
    }

    public void RevealCard()
    {
        if (isHidden)
        {
            animCoroutine = StartCoroutine(AnimCoroutine(_qInvertedRot, _qRevealRot));
            isHidden = false;
        }
    }

    public void HideCard()
    {
        if (!isHidden)
        {
            animCoroutine = StartCoroutine(AnimCoroutine(_qRevealRot, _qInvertedRot));
            isHidden = true;
        }
    }
}
