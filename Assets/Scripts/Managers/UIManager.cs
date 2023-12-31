using System.Numerics;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region SERIALISED_FIELDS
    [SerializeField]
    private GameObject _menuPanel;
    [SerializeField]
    private GameObject _gamePanel;
    [SerializeField]
    private GameObject _settingsPanel;

    [SerializeField]
    private GameObject _newGame;
    [SerializeField]
    private GameObject _quit;
    [SerializeField]
    private GameObject _resume;
    [SerializeField]
    private GameObject _loadGame;

    [SerializeField]
    private TMP_InputField _gridX;
    [SerializeField]
    private TMP_InputField _gridY;


    [SerializeField]
    private TextMeshProUGUI _score;
    [SerializeField]
    private TextMeshProUGUI _timer;
    #endregion

    private bool _isGameToLoadAvailable = false;
    private Vector2Int _curentGridSize; 
    #region UNITY_FUNCTIONS
    private void Start()
    {
        _gamePanel.SetActive(false);
        _settingsPanel.SetActive(false);
        _menuPanel.SetActive(true);

        _resume.SetActive(false);
        _loadGame.SetActive(_isGameToLoadAvailable);
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    public void SetLoadgameButtonStatus(bool status)
    {
        _isGameToLoadAvailable = status;
    }

    public void UpdateGridSize(Vector2Int vector2Int)
    {
        _curentGridSize = vector2Int;
    }

    public void OnGameStartOrLoad()
    {
        _gamePanel.SetActive(true);
        _menuPanel.SetActive(false);
    }

    public void OnGameLost()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
        _loadGame.SetActive(_isGameToLoadAvailable);
    }

    public void OnGameWon()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
        _loadGame.SetActive(_isGameToLoadAvailable);
    }

    public void OnMainMenuClick()
    {
        _resume.SetActive(true);
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
        _loadGame.SetActive(_isGameToLoadAvailable);
    }

    public void OnResumeClick()
    {
        _resume.SetActive(false);
        _gamePanel.SetActive(true);
        _menuPanel.SetActive(false);
    }

    #region SETTINGS_FUNCTIONS
    public void OnSettings()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(false);
        _settingsPanel.SetActive(true);
        _gridX.text = _curentGridSize.x + "";
        _gridY.text = _curentGridSize.y + "";
    }

    public void OnSettingsBack()
    {
        _menuPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }
    #endregion

    #region GAMEPLAY_UI
    public void OnScoreUpdated(int score)
    {
        _score.text = score + "";
    }

    public void OnTimerUpdated(float time)
    {
        _timer.text = Mathf.CeilToInt(time) + "";
    }
    #endregion

    #endregion
}