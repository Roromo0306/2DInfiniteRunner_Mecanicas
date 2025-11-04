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

        var playerModel = new PlayerModel();
        playerController = new PlayerController(playerModel);
        // Pasar referencias a views (por ejemplo) o registrar servicios disponibles
        // Por ejemplo, HUDView puede obtener GameController via un setter o evento.
    }

    void Update()
    {
        float dt = Time.deltaTime;
        gameController.Update(dt);
        playerController.Update(dt, gameController.GetObstacleStore());
    }
}
