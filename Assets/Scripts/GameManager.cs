using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /*---------------UI---------------*/
    //pre game screen
    [SerializeField]
    TMP_InputField _nameField;
    [SerializeField]
    Button _playButton;
    [SerializeField]
    Button _leadersButton;
    [SerializeField]
    Button _closeLeadersButton;
    [SerializeField]
    Button _rulesButton;
    [SerializeField]
    Button _closeRulesButton;
    [SerializeField]
    Button _exitButton;
    [SerializeField]
    Image _leadersBlock;
    [SerializeField]
    TextMeshProUGUI _leadersText;
    [SerializeField]
    Image _rulesBlock;
    [SerializeField]
    GameObject _preGameScreen;

    //game active screen
    [SerializeField]
    TextMeshProUGUI _scoreText;
    [SerializeField]
    TextMeshProUGUI _timeText;
    [SerializeField]
    TextMeshProUGUI _powerupText;
    [SerializeField]
    GameObject _UIScreen;

    //after game screen
    [SerializeField]
    Button _restartButton;
    [SerializeField]
    GameObject _afterGameScreen;
    /*---------------END_UI_BLOCK---------------*/

    [SerializeField]
    GameObject[] _foodPrefabs;
    [SerializeField]
    GameObject[] _powerupPrefabs;
    [SerializeField]
    GameObject _box;
    [SerializeField]
    AudioClip _gameOverSound;

    int _score = 0;
    int _time = 10;
    int _scoreModifier = 1;
    int _foodSpawnRate = 2;
    int _powerupSpawnRate = 13;

    bool _missIgnore = false;
    public static bool isGameActive;

    void Start()
    {
        DataManager.Instance.LoadData();
        LoadLeaders();

        _playButton.onClick.AddListener(StartGame);
        _leadersButton.onClick.AddListener(ShowLeaders);
        _closeLeadersButton.onClick.AddListener(CloseLeaders);
        _rulesButton.onClick.AddListener(ShowRules);
        _closeRulesButton.onClick.AddListener(CloseRules);
        _exitButton.onClick.AddListener(Exit);
        _restartButton.onClick.AddListener(RestartGame);
    }

    void StartGame()
    {
        if (_nameField.text == string.Empty)
        {
            _nameField.placeholder.color = Color.red;
        }
        else
        {
            DataManager.Instance.CurrentName = _nameField.text;
            _preGameScreen.SetActive(false);
            _UIScreen.SetActive(true);
            _box.SetActive(true);

            isGameActive = true;
            UpdateScore(0);
            StartCoroutine(SpawnTarget(_foodPrefabs, _foodSpawnRate));
            StartCoroutine(SpawnTarget(_powerupPrefabs, _powerupSpawnRate, _powerupSpawnRate));
            StartCoroutine(Countdown());
        }
    }

    void LoadLeaders()
    {
        for (int i = 0; i < DataManager.Instance.Leaders.Length; i++)
        {
            _leadersText.text += $"{i + 1}. " + DataManager.Instance.Leaders[i].Name + "\t" + DataManager.Instance.Leaders[i].Points + "\n";
        }
    }

    void ShowLeaders()
    {
        _nameField.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(false);
        _leadersButton.gameObject.SetActive(false);
        _rulesButton.gameObject.SetActive(false);
        _exitButton.gameObject.SetActive(false);

        _leadersBlock.gameObject.SetActive(true);
    }

    void CloseLeaders()
    {
        _nameField.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(true);
        _leadersButton.gameObject.SetActive(true);
        _rulesButton.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);

        _leadersBlock.gameObject.SetActive(false);
    }

    void ShowRules()
    {
        _nameField.gameObject.SetActive(false);
        _playButton.gameObject.SetActive(false);
        _leadersButton.gameObject.SetActive(false);
        _rulesButton.gameObject.SetActive(false);
        _exitButton.gameObject.SetActive(false);

        _rulesBlock.gameObject.SetActive(true);
    }

    void CloseRules()
    {
        _nameField.gameObject.SetActive(true);
        _playButton.gameObject.SetActive(true);
        _leadersButton.gameObject.SetActive(true);
        _rulesButton.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);

        _rulesBlock.gameObject.SetActive(false);
    }

    void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Exit();
#endif
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GameOver()
    {
        isGameActive = false;

        AudioSource audioSource = Camera.main.GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.PlayOneShot(_gameOverSound);

        _afterGameScreen.SetActive(true);

        DataManager.Instance.CurrentPoints = _score;

        DataManager.Instance.SaveData();
    }

    public void UpdateScore(int scoreAmount)
    {
        if (_missIgnore && scoreAmount < 0) scoreAmount = 0;

        _score += scoreAmount > 0 ? scoreAmount * _scoreModifier : scoreAmount;
        if (_score < 0) _score = 0;
        _scoreText.SetText("Score: " + _score);
    }

    public void UpdateTime(int timeAmount)
    {
        _time += timeAmount;
        if (_time > 60) _time = 60;
        _timeText.SetText("Time: " + _time);

        if (_time == 0) GameOver();
    }

    public IEnumerator SetScoreModifier(int scoreModifier, int duration)
    {
        _powerupText.gameObject.SetActive(true);
        _scoreModifier = scoreModifier;
        while (duration > 0 && isGameActive)
        {
            _powerupText.SetText("Score X2: " + duration);
            yield return new WaitForSeconds(1);
            duration--;
        }
        _scoreModifier = 1;
        _powerupText.gameObject.SetActive(false);
    }

    public IEnumerator SetMissIgnore(int duration)
    {
        _powerupText.gameObject.SetActive(true);
        _missIgnore = true;
        while (duration > 0 && isGameActive)
        {
            _powerupText.SetText("Score saver: " + duration);
            yield return new WaitForSeconds(1);
            duration--;
        }
        _missIgnore = false;
        _powerupText.gameObject.SetActive(false);
    }

    public IEnumerator SetTimeAdder(int scoreAmount,  int duration)
    {
        _powerupText.gameObject.SetActive(true);
        UpdateTime(scoreAmount);
        while (duration > 0 && isGameActive)
        {
            _powerupText.SetText("Added " + scoreAmount + " seconds!");
            yield return new WaitForSeconds(1);
            duration--;
        }
        _powerupText.gameObject.SetActive(false);
    }

    IEnumerator SpawnTarget(GameObject[] targets, int spawnRate, int spawnDelay = 0)
    {
        yield return new WaitForSeconds(spawnDelay);

        while (isGameActive)
        {
            int index = Random.Range(0, targets.Length);
            Instantiate(targets[index]);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    IEnumerator Countdown()
    {
        while (isGameActive)
        {
            UpdateTime(-1);
            yield return new WaitForSeconds(1);
        }
    }
}
