using RunnerGame.MVC.Controller;
using RunnerGame.MVC.Model;
using RunnerGame.MVC.View;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public GameConfigSO config;
    public PowerUpSO[] powerUps;
    public AudioSource audioSource;
    public float worldLeftX = -12f;
    IAudioService audioService;
    GameController gameController;
    PlayerController playerController;

    void Awake()
    {
        audioService = new AudioService(audioSource);
        var spawnStrategy = new DifficultyScalingStrategy();
        gameController = new GameController(config, spawnStrategy, worldLeftX);

        // Busca el PlayerController en la escena
        playerController = FindObjectOfType<PlayerController>();
        PlayerView playerView = FindObjectOfType<PlayerView>();

        var playerModel = new PlayerModel();

        // Inicializamos correctamente MVC
        playerController.Initialize(playerModel, playerView);
    }

    void Update()
    {
        float dt = Time.deltaTime;
        gameController.Update(dt);
        playerController.UpdateController(dt);
    }
}
