using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

#if TMP_PRESENT
using TMPro;
#endif


// Usa el namespace que estés usando en tu proyecto
namespace RunnerGame.MVC.View
{
    [RequireComponent(typeof(Canvas))]
    public class GameOverView : MonoBehaviour
    {
        [Header("UI")]

        [SerializeField] private TextMeshProUGUI finalScoreTextTMP;

       
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;

        [Header("Opcional: Fade")]
        [SerializeField] private Animator fadeAnimator; // animator con trigger "StartFade"
        [Tooltip("Segundos que dura la animación de fade (si usas).")]
        [SerializeField] private float fadeDuration = 0.6f;

        [Header("Comportamiento")]
        [SerializeField] private bool pauseOnGameOver = true;

        private int lastScore = 0;
        private bool isShown = false;

        private void Awake()
        {
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(OnRestartButtonPressed);
            }

            // Asegurarse de que los textos existan (al menos uno)
            if (finalScoreTextTMP == null
#if TMP_PRESENT
                && finalScoreTextTMP == null
#endif
            )
            {
                Debug.LogWarning("[GameOverView] No hay referencia a ningún Text/TMP Text para mostrar la puntuación.");
            }

            // Subscribe a eventos globales (GameEvents fue propuesto en tu arquitectura)
            GameEvents.OnScoreChanged += OnScoreChanged;
            GameEvents.OnPlayerDied += OnPlayerDied;
        }

        private void OnDestroy()
        {
            GameEvents.OnScoreChanged -= OnScoreChanged;
            GameEvents.OnPlayerDied -= OnPlayerDied;

            if (restartButton != null)
                restartButton.onClick.RemoveListener(OnRestartButtonPressed);
        }

        private void OnScoreChanged(int newScore)
        {
            lastScore = newScore;
        }

        private void OnPlayerDied()
        {
            // Muestra el panel una vez por muerte
            if (isShown) return;
            isShown = true;

            if (pauseOnGameOver) Time.timeScale = 0f;

            if (gameOverPanel != null) gameOverPanel.SetActive(true);
            UpdateFinalScoreText(lastScore);
        }

        private void UpdateFinalScoreText(int score)
        {
#if TMP_PRESENT
            if (finalScoreTextTMP != null)
            {
                finalScoreTextTMP.text = $"Puntuación final: {score}";
                return;
            }
#endif
            if (finalScoreTextTMP != null)
            {
                finalScoreTextTMP.text = $"Puntuación final: {score}";
            }
        }

        // Método público para el botón Restart (lo puedes asignar también desde el Inspector)
        public void OnRestartButtonPressed()
        {
            // Si Time.timeScale estaba pausado, restaura para la carga
            if (Time.timeScale == 0f) Time.timeScale = 1f;

            if (fadeAnimator != null)
            {
                StartCoroutine(PlayFadeAndReload());
            }
            else
            {
                ReloadSceneImmediate();
            }
        }

        private IEnumerator PlayFadeAndReload()
        {
            // Disparar animación de fade (asegúrate de tener un trigger llamado "StartFade")
            if (fadeAnimator != null)
            {
                fadeAnimator.SetTrigger("StartFade");
            }

            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.unscaledDeltaTime; // usar unscaled ya que Time.timeScale puede ser 0
                yield return null;
            }

            ReloadSceneImmediate();
        }

        private void ReloadSceneImmediate()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        // Método público para ocultar el panel (si quieres reiniciar sin recargar escena)
        public void HideGameOver()
        {
            isShown = false;
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (Time.timeScale == 0f) Time.timeScale = 1f;
        }
    }
}
