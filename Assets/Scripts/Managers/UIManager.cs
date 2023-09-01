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
    private TextMeshProUGUI _score;
    [SerializeField]
    private TextMeshProUGUI _timer;
    #endregion

    #region UNITY_FUNCTIONS
    private void Start()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);

        _resume.SetActive(false);
    }
    #endregion

    #region PUBLIC_FUNCTIONS
    public void OnGameStart()
    {
        _gamePanel.SetActive(true);
        _menuPanel.SetActive(false);
    }

    public void OnGameLost()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
    }

    public void OnGameWon()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
    }

    public void OnMainMenuClick()
    {
        _gamePanel.SetActive(false);
        _menuPanel.SetActive(true);
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