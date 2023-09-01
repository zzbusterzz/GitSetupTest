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
    private GameObject _newGame;
    [SerializeField]
    private GameObject _quit;
    [SerializeField]
    private GameObject _resume;
    [SerializeField]
    private GameObject _loadGame;

    [SerializeField]
    private TextMeshProUGUI _score;
    [SerializeField]
    private TextMeshProUGUI _timer;
    #endregion

    private bool _isGameToLoadAvailable = false;

    #region UNITY_FUNCTIONS
    private void Start()
    {
        _gamePanel.SetActive(false);
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

    public void OnScoreUpdated(int score)
    {
        _score.text = score + "";
    }

    public void OnTimerUpdated(float time)
    {
        _timer.text = Mathf.CeilToInt(time) + "";
    }
    #endregion
}