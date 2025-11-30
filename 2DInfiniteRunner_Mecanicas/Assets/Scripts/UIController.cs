using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject creditsPanel;
    public GameObject highScorePanel;
    public GameObject loadingPanel;
    public GameObject countdownPanel;
    public GameObject hudPanel;
    public GameObject gameOverPanel;

    [Header("HUD")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI livesText;

    [Header("Menu")]
    public TMP_InputField nameInput;
    public TextMeshProUGUI bestScoresText;

    [Header("GameOver")]
    public TextMeshProUGUI finalScoreText;

    public GameConfig config;

    private PlayerModel _player;
    private GameModel _game;
    private IEventBus _bus;
    private ISaveService _save;

    void Start()
    {
        _bus = GameContainer.Resolve<IEventBus>();
        _save = GameContainer.Resolve<ISaveService>();
        _player = GameInstaller.Instance.PlayerModel;
        _game = GameInstaller.Instance.GameModel;

        _bus.Subscribe<PlayerHurtEvent>(OnPlayerHurt);
        _bus.Subscribe<GameOverEvent>(OnGameOver);
        _bus.Subscribe<LifeCollectedEvent>(OnLifeCollected);

        ShowMainMenu();
    }

    void OnDestroy()
    {
        _bus.Unsubscribe<PlayerHurtEvent>(OnPlayerHurt);
        _bus.Unsubscribe<GameOverEvent>(OnGameOver);
        _bus.Unsubscribe<LifeCollectedEvent>(OnLifeCollected);
    }

    void Update()
    {
        if (_game.IsRunning)
        {
            timerText.text = FormatTime(_game.TimeElapsed);
            scoreText.text = _game.Score.ToString();
            livesText.text = $"{_player.Lives}/{_player.MaxLives}";
        }
    }

    string FormatTime(float t)
    {
        int minutes = (int)(t / 60);
        int seconds = (int)(t % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    // BOTONES del menu
    public void OnPlayPressed()
    {
        // guardar nombre antes de empezar
        var playerName = nameInput.text;
        if (string.IsNullOrEmpty(playerName)) playerName = "Player";
        PlayerPrefs.SetString("RUNNER_PLAYERNAME", playerName);
        StartCoroutine(StartGameFlow());
    }

    IEnumerator StartGameFlow()
    {
        mainMenuPanel.SetActive(false);
        loadingPanel.SetActive(true);
        // bootstrapping visual: tiempo breve
        yield return new WaitForSeconds(1f);
        loadingPanel.SetActive(false);
        // countdown
        countdownPanel.SetActive(true);
        float c = config.countdownSeconds;
        var audio = GameContainer.Resolve<IAudioService>();
        while (c > 0)
        {
            countdownPanel.GetComponentInChildren<TextMeshProUGUI>().text = Mathf.CeilToInt(c).ToString();
            yield return new WaitForSeconds(1f);
            c -= 1f;
        }
        countdownPanel.GetComponentInChildren<TextMeshProUGUI>().text = "GO!";
        yield return new WaitForSeconds(0.5f);
        countdownPanel.SetActive(false);
        SceneManager.LoadScene("Game");
        // start game
        hudPanel.SetActive(true);
        GameInstaller.Instance.GameModel.IsRunning = true;
        _bus.Publish(new GameStartedEvent());
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        creditsPanel.SetActive(false);
        highScorePanel.SetActive(false);
        loadingPanel.SetActive(false);
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        countdownPanel.SetActive(false );

        // load scores
        var list = _save.LoadHighScores(10);
        bestScoresText.text = "";
        int i = 1;
        foreach (var entry in list)
        {
            bestScoresText.text += $"{i}. {entry.name} - {entry.score}\n";
            i++;
        }
    }

    public void OnCreditsPressed()
    {
        creditsPanel.SetActive(true);
    }

    public void OnCloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    public void OnHighScoresPressed()
    {
        highScorePanel.SetActive(true);
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    private void OnPlayerHurt(PlayerHurtEvent evt)
    {
        // anim/feedback si quieres
        livesText.text = $"{evt.remainingLives}/{_player.MaxLives}";
    }

    private void OnLifeCollected(LifeCollectedEvent evt)
    {
        // sumar vida en modelo
        for (int i = 0; i < evt.newLives; i++) _player.AddLife();
        livesText.text = $"{_player.Lives}/{_player.MaxLives}";
        GameContainer.Resolve<IAudioService>().PlayOneShot(GameContainer.Resolve<AudioLibrary>().pickupLife);
    }

    private void OnGameOver(GameOverEvent evt)
    {
        _game.IsRunning = false;
        hudPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        finalScoreText.text = evt.finalScore.ToString();

        // save highscore
        var name = PlayerPrefs.GetString("RUNNER_PLAYERNAME", "Player");
        _save.SaveHighScore(name, evt.finalScore);
    }

    // boton reiniciar desde GameOver
    public void OnRestartPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void OnMainMenuPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"); // si tienes escena dedicada
    }
}
