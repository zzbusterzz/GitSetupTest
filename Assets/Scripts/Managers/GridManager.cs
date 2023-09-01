﻿using System.Collections.Generic;
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
    public float Cardscale { get => cardscale; }
    public Vector2 GridDimensionsHalf { get => _gridDimensionsHalf; }
    #endregion

    #region PRIVATE_FIELDS
    private List<Vector3> _gridPosition;
    private Vector3 _gridStart;
    private float cardscale;
    private Camera cachedMainCam;

    private float xPoint = Screen.width * (0.01f * Constants.GridStartPercent);
    #endregion

    #region PUBLIC_FUNCTIONS
    public void Init()
    {
        cachedMainCam = Camera.main;
        _gridPosition = new List<Vector3>();        
        GenerateGridStartPoint(out cardscale);
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
        Vector3 gridStartVector = _gridStart + new Vector3(_gridDimensionsHalf.x * cardscale, -_gridDimensionsHalf.y * cardscale, 0);
        float dimWidth = 2 * _gridDimensionsHalf.x * cardscale;
        float dimHeight = 2 * _gridDimensionsHalf.y * cardscale;

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
    #endregion

    #region PRIVATE_FUNCTIONS
    private void GenerateGridStartPoint(out float cardScale, float scale = 1)
    {
        cardScale = scale;

        float avilAreaWidth = Screen.width - xPoint;
        float availAreaHeight = Screen.height;

        float xDimension = 2f * _gridDimensionsHalf.x * scale;
        float yDimension = 2f * _gridDimensionsHalf.y * scale;

        Vector3 origin = cachedMainCam.WorldToScreenPoint(Vector3.zero);
        Vector3 extent = cachedMainCam.WorldToScreenPoint(new Vector3(xDimension, yDimension, 0));

        float totalWidth = _gridSize.x * (Mathf.Abs(origin.x - extent.x));
        float totalHeight = _gridSize.y * (Mathf.Abs(origin.y - extent.y));


        float widthAspect = totalWidth / avilAreaWidth;
        float heightAspect = totalHeight / availAreaHeight;

        if (widthAspect > 1 || heightAspect > 1)
        {
            if (widthAspect > heightAspect)
            {
                GenerateGridStartPoint(out cardScale, 1 + (1 - widthAspect));
            }
            else
            {
                GenerateGridStartPoint(out cardScale, 1 + (1 - heightAspect));
            }
        }
        else if (widthAspect < 0.85f && heightAspect < 0.85f)
        {
            if (widthAspect > heightAspect)
            {
                GenerateGridStartPoint(out cardScale, 1/widthAspect);
            }
            else
            {
                GenerateGridStartPoint(out cardScale, 1/heightAspect);
            }
        }
        else
        {
            //If the area width and area height is in the req 
            float xpadding = Mathf.Abs(totalWidth - avilAreaWidth);
            float ypadding = Mathf.Abs(totalHeight - availAreaHeight);

            Vector3 pos = new Vector3(xPoint + (xpadding / 2),
                                        availAreaHeight - ypadding / 2,
                                        0);

            _gridStart = cachedMainCam.ScreenToWorldPoint(pos);
            _gridStart.z = 0;
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