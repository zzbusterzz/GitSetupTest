using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GridManager
{
    #region SERIALISED_FIELDS
    [SerializeField]
    private Vector2Int _gridSize;

    [SerializeField]
    private Vector2 _gridDimensionsHalf;

    [SerializeField]
    private Vector2 _padding;

#if UNITY_EDITOR
    [SerializeField]
    private bool _generateDebugTransformForGridPoint;
#endif
    #endregion

    #region PUBLIC_GETTERS
    public int TotalItems => _gridSize.x * _gridSize.y;
    public float Cardscale { get => _cardscale; set => _cardscale = value; }
    public Vector2 GridDimensionsHalf { get => _gridDimensionsHalf; }
    public Vector2Int GridSize { get => _gridSize; }
    #endregion

    #region PRIVATE_FIELDS
    private GameEvents _gameEvents;
    private List<Vector3> _gridPosition;
    private Vector3 _gridStart = default;
    private float _cardscale;
    private Camera _cachedMainCam;

#if UNITY_EDITOR
    private GameObject _helperGO;
#endif

    //Local constants
    private float _xPoint;
    private Vector2 _gridFullDim = default;
    private float _avilAreaWidth;
    private float _availAreaHeight = Screen.height;
    #endregion

    #region PUBLIC_FUNCTIONS
    public void Init(GameEvents gameEvents)
    {
        _gameEvents = gameEvents;
        _cachedMainCam = Camera.main;
        _gridPosition = new ();

        _xPoint = Screen.width * 0.01f * Constants.GridStartPercent;
        _gridFullDim = new Vector2(2f * _gridDimensionsHalf.x, 2 * _gridDimensionsHalf.y);
        _avilAreaWidth = Screen.width - _xPoint;

        GenerateGridStartPoint(out _cardscale);
        _gameEvents.OnGridSizeUpdated?.Invoke(_gridSize);
    }

    public Vector3 GetGridPosition()
    {
        int gridIndex = Random.Range(0, _gridPosition.Count);
        Vector3 pos = _gridPosition[gridIndex];
        _gridPosition.Remove(pos);
        return pos;
    }

    public void CreateGrid()
    {
        Vector2 scaledPdding = new Vector2(_padding.x * _cardscale,
                                            _padding.y * 0.75f * _cardscale);

        Vector3 gridStartVector =   _gridStart + 
                                    new Vector3(_gridDimensionsHalf.x * _cardscale + scaledPdding.x,
                                                -_gridDimensionsHalf.y * _cardscale - scaledPdding.y,
                                                0);

        float dimWidth = (_gridFullDim.x * _cardscale) + scaledPdding.x;
        float dimHeight = (_gridFullDim.y * _cardscale) + scaledPdding.y;

        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                _gridPosition.Add(new Vector3(
                    gridStartVector.x + (i * dimWidth),
                    gridStartVector.y - (j * dimHeight),
                    gridStartVector.z));
            }
        }
    }

    public void UpdateGridSize(int x, int y)
    {
        if(_gridSize.x != x || _gridSize.y != y)
        {
            _gridSize.x = x;
            _gridSize.y = y;

            GenerateGridStartPoint(out _cardscale);

            _gameEvents.OnGridSizeUpdated?.Invoke(_gridSize);
        }
    }
    #endregion

    #region PRIVATE_FUNCTIONS
    private void GenerateGridStartPoint(out float cardScale, float scale = 1)
    {
        cardScale = scale;

        GetWidthAndHeightInCurrentScale(scale, out float totalWidth, out float totalHeight);

        float widthAspect = totalWidth / _avilAreaWidth;
        float heightAspect = totalHeight / _availAreaHeight;

        //we use width aspect and height aspect to check if cards go outside or less than req size
        //if width aspect or height aspect is greater than 1 then rescale
        //if width aspect and height aspect is less than 85% then scale up
        if (widthAspect > 1
            || heightAspect > 1
            || (widthAspect < 0.85f && heightAspect < 0.85f))
        {
            cardScale = 1 / (widthAspect > heightAspect ? widthAspect : heightAspect);
            GetWidthAndHeightInCurrentScale(cardScale, out totalWidth, out totalHeight);
        }

        Debug.Log($"Final scale {cardScale}");

        //If the area width and area height is in the req 
        float remWidth = Mathf.Abs(totalWidth - _avilAreaWidth);
        float remHeight = Mathf.Abs(totalHeight - _availAreaHeight);

        Vector3 pos = new Vector3(  _xPoint + (remWidth / 2),
                                    _availAreaHeight - remHeight / 2,
                                    0);

        _gridStart = _cachedMainCam.ScreenToWorldPoint(pos);
        _gridStart.z = 0;

#if UNITY_EDITOR
        PlaceTempObjectAtGridStartPoint();
#endif

    }

    private void GetWidthAndHeightInCurrentScale(float scale, out float totalWidth, out float totalHeight)
    {
        float xDimension = _gridFullDim.x * scale + (_padding.x * scale);
        float yDimension = _gridFullDim.y * scale + (_padding.y * scale);

        Vector3 origin = _cachedMainCam.WorldToScreenPoint(Vector3.zero);
        Vector3 extent = _cachedMainCam.WorldToScreenPoint(new Vector3(xDimension, yDimension, 0));

        totalWidth = _gridSize.x * (Mathf.Abs(origin.x - extent.x));
        totalHeight = _gridSize.y * (Mathf.Abs(origin.y - extent.y));
    }

#if UNITY_EDITOR
    private void PlaceTempObjectAtGridStartPoint()
    {
        if (_generateDebugTransformForGridPoint)
        {
            if (_helperGO == null)
            {
                _helperGO = new GameObject();
            }
            _helperGO.transform.position = _gridStart;

        }
    }
#endif

#endregion
}