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
                timeText.text = $"Time: {newTime:0.0}s";
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
    }
}
