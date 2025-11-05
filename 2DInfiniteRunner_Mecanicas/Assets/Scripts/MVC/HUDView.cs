using UnityEngine;
using UnityEngine.UI;
using TMPro;
using RunnerGame.Core;
using RunnerGame.MVC.Model;

namespace RunnerGame.MVC.View
{
    /// <summary>
    /// View del HUD (UI principal del juego).
    /// Muestra puntuación, tiempo y barra de power-up.
    /// Se actualiza por Observer/Eventos.
    /// </summary>
    public class HUDView : MonoBehaviour, IGameEventListener
    {
        [Header("Referencias UI")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI timeText;
        [SerializeField] private Slider powerUpBar;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;

        private float currentScore;
        private float currentTime;
        private bool isGameOver = false;

        private void Awake()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (powerUpBar != null)
                powerUpBar.gameObject.SetActive(false);
        }

        /// <summary>
        /// Se llama cada frame desde GameController.UpdateUI() o desde el Observer.
        /// </summary>
        public void UpdateScore(float newScore)
        {
            currentScore = newScore;
            if (scoreText)
                scoreText.text = $"Score: {Mathf.FloorToInt(currentScore)}";
        }

        public void UpdateTime(float newTime)
        {
            currentTime = newTime;
            if (timeText)
                timeText.text = FormatTimeMMSS(newTime);
        }

        public void UpdatePowerUp(float ratio)
        {
            if (powerUpBar == null) return;
            powerUpBar.gameObject.SetActive(ratio > 0);
            powerUpBar.value = ratio;
        }

        public void ShowGameOver(float finalScore)
        {
            isGameOver = true;
            if (gameOverPanel)
                gameOverPanel.SetActive(true);
            if (finalScoreText)
                finalScoreText.text = $"Final Score: {Mathf.FloorToInt(finalScore)}";
        }

        public void HideGameOver()
        {
            isGameOver = false;
            if (gameOverPanel)
                gameOverPanel.SetActive(false);
        }

        // Implementación del Observer
        public void OnEventRaised(GameEventType type, object data)
        {
            switch (type)
            {
                case GameEventType.ScoreUpdated:
                    UpdateScore((float)data);
                    break;
                case GameEventType.PowerUpProgress:
                    UpdatePowerUp((float)data);
                    break;
                case GameEventType.GameOver:
                    ShowGameOver((float)data);
                    break;
                case GameEventType.TimeUpdated:
                    UpdateTime((float)data);
                    break;
            }
        }

        private void OnEnable()
        {
            GameEvents.OnScoreChanged += OnScoreChanged;
            GameEvents.OnTimeUpdated += OnTimeUpdated;
        }

        private void OnDisable()
        {
            GameEvents.OnScoreChanged -= OnScoreChanged;
            GameEvents.OnTimeUpdated -= OnTimeUpdated;
        }

        private void OnScoreChanged(int newScore)
        {
            if (scoreText != null) scoreText.text = $"Score: {newScore}";
            Debug.Log($"[HUD] Score updated -> {newScore}");
        }
        private string FormatTimeMMSS(float elapsed)
        {
            int minutes = Mathf.FloorToInt(elapsed / 60f);
            int seconds = Mathf.FloorToInt(elapsed % 60f);
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        // ÚNICO método OnTimeUpdated (formatea mm:ss)
       private void OnTimeUpdated(float elapsed)
        {
            Debug.Log($"[HUDView] OnTimeUpdated called with elapsed={elapsed}");
            if (timeText == null) return;
            timeText.text = FormatTimeMMSS(elapsed); // o lo que tengas
        }
        

        // Método de prueba: ejecuta desde el Inspector (clic en los tres puntos del componente -> TestShowTime)
        [ContextMenu("TestShowTime")]
        public void TestShowTime()
        {
            if (timeText == null)
            {
                Debug.LogWarning("[HUDView] TestShowTime: timeText is null — asigna el Text en el inspector.");
                return;
            }
            timeText.gameObject.SetActive(true);
            timeText.text = "TEST " + FormatTimeMMSS(Time.time);
            Debug.Log("[HUDView] TestShowTime ejecutado. Revisa el texto en pantalla.");
        }
    }
}
