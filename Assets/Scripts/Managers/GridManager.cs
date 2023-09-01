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

    #if UNITY_EDITOR
    [SerializeField]
    private bool _generateDebugTransformForGridPoint;
    #endif
    #endregion

    #region PUBLIC_GETTERS
    public int TotalItems => _gridSize.x* _gridSize.y;
    #endregion

    #region PRIVATE_FIELDS
    private List<Vector3> _gridPosition;
    private Vector3 _gridStart;
    #endregion

    #region PUBLIC_FUNCTIONS
    public void Init()
    {
        _gridPosition = new List<Vector3>();
        GenerateGridStartPoint();
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
        Vector3 gridStartVector = _gridStart + new Vector3(_gridDimensionsHalf.x, -_gridDimensionsHalf.y, 0);
        float dimWidth = 2 * _gridDimensionsHalf.x;
        float dimHeight = 2 * _gridDimensionsHalf.y;

        for (int i = 0; i < _gridSize.x; i++)
        {
            for (int j = 0; j < _gridSize.y; j++)
            {
                _gridPosition.Add(new Vector3(
                    gridStartVector.x + (i * dimWidth),
                    gridStartVector.y - (j * dimHeight),
                    0));
            }
        }
    }
    #endregion

    #region PRIVATE_FUNCTIONS
    private void GenerateGridStartPoint()
    {
        Camera mainCam = Camera.main;
        float xPoint = Screen.width * (0.01f * Constants.GridStartPercent);
        _gridStart = mainCam.ScreenToWorldPoint(new Vector3(xPoint, 0, 0));

        float avilAreaWidth = Screen.width - xPoint;
        float availAreaHeight = Screen.height;

        Vector3 origin = mainCam.WorldToScreenPoint(Vector3.zero);
        Vector3 extent = mainCam.WorldToScreenPoint(new Vector3(2f * _gridDimensionsHalf.x, 2f * _gridDimensionsHalf.y, 0));

        float totalWidth = _gridSize.x * (Mathf.Abs(origin.x - extent.x));
        float totalHeight = _gridSize.y * (Mathf.Abs(origin.y - extent.y));


        //If the area width and area height is in the req 
        if (totalWidth < avilAreaWidth && totalHeight < availAreaHeight)
        {
            float xpadding = Mathf.Abs(totalWidth - avilAreaWidth);
            float ypadding = Mathf.Abs(totalHeight - availAreaHeight);

            Vector3 pos = new Vector3(xPoint + (xpadding / 2),
                                        availAreaHeight - ypadding / 2,
                                        0);

            _gridStart = mainCam.ScreenToWorldPoint(pos);

#if UNITY_EDITOR
            if (_generateDebugTransformForGridPoint)
            {
                GameObject go = new GameObject();
                go.transform.position = _gridStart;

            }
#endif
        }
    }
    #endregion
}