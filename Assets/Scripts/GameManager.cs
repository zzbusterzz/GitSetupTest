using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region SERIALISED_FIELDS
    [SerializeField]
    private Vector2Int _gridSize;
    #endregion

    #region PRIVATE_FIELDS
    private float _visibilityDuration = 4;
    private int _score = 0;
    #endregion

    public void NewGame()
    {

    }

    public void Restart()
    {

    }

    void CreateGrid()
    {

    }
}
