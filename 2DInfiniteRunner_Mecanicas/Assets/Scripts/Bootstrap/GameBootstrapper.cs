using RunnerGame.MVC.Controller;
using RunnerGame.MVC.Model;
using RunnerGame.MVC.View;
using RunnerGame.Systems;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    public GameConfigSO config;
    public PowerUpSO[] powerUps;
    public AudioSource audioSource;
    public float worldLeftX = -12f;
    public ObstacleView obstaclePrefab;
    IAudioService audioService;
    GameController gameController;
    PlayerController playerController;
    PoolManager obstaclePoolManager;
    PlayerView playerViewInScene;

    void Awake()
    {
        // Mensaje inicial
        Debug.Log("[Bootstrapper] Awake: inicializando...");

        // 1) Config obligatorio
        if (config == null)
        {
            Debug.LogError("[Bootstrapper] GameConfigSO (config) no asignado en el Inspector. Crea y arrastra un asset en Bootstrapper.config.");
            return;
        }

        // 2) AudioService: si no asignaste AudioSource, intenta obtener uno del mismo GameObject
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                // intenta buscar cualquier AudioSource en la escena
                audioSource = FindObjectOfType<AudioSource>();
                if (audioSource != null)
                    Debug.LogWarning("[Bootstrapper] audioSource no asignado. Se encontró y usará el primer AudioSource de la escena.");
                else
                    Debug.LogWarning("[Bootstrapper] audioSource no encontrado en escena. Si no asignas uno no habrá SFX.");
            }
            else
            {
                Debug.Log("[Bootstrapper] audioSource tomado del mismo GameObject del Bootstrapper.");
            }
        }

        audioService = new AudioService(audioSource);

        // 3) Spawn strategy
        var spawnStrategy = new DifficultyScalingStrategy();
        gameController = new GameController(config, spawnStrategy, worldLeftX);

        // 4) PlayerController / PlayerView: si no arrastraste la vista, intenta Find
        if (playerViewInScene == null)
        {
            playerViewInScene = FindObjectOfType<RunnerGame.MVC.View.PlayerView>();
            if (playerViewInScene != null)
                Debug.Log("[Bootstrapper] playerViewInScene no asignado en inspector — se encontró PlayerView en la escena.");
            else
                Debug.LogWarning("[Bootstrapper] playerViewInScene no encontrado. Asegúrate de tener un GameObject Player con PlayerView.");
        }

        // 5) Crear/obtener PlayerController: asumimos PlayerController es MonoBehaviour en el GameObject Player
        if (playerViewInScene != null)
        {
            playerController = playerViewInScene.GetComponent<RunnerGame.MVC.Controller.PlayerController>();
            if (playerController == null)
            {
                Debug.LogWarning("[Bootstrapper] PlayerView existe pero no tiene PlayerController. Intentando AddComponent<PlayerController>()...");
                playerController = playerViewInScene.gameObject.AddComponent<RunnerGame.MVC.Controller.PlayerController>();
            }
        }

        // 6) PoolManager (ObstaclePool): intenta usar referencia inspector, luego Find, luego crear
        if (obstaclePoolManager == null)
        {
            obstaclePoolManager = FindObjectOfType<RunnerGame.Systems.PoolManager>();
            if (obstaclePoolManager != null)
            {
                Debug.Log("[Bootstrapper] obstaclePoolManager asignado automáticamente con FindObjectOfType.");
            }
            else
            {
                Debug.LogWarning("[Bootstrapper] obstaclePoolManager no encontrado: se creará un GameObject ObstaclePool con PoolManager.");
                var poolGO = new GameObject("ObstaclePool");
                obstaclePoolManager = poolGO.AddComponent<RunnerGame.Systems.PoolManager>();

                // Intentar asignar prefab automático desde Resources (si existe)
                if (obstaclePrefab == null)
                {
                    var loaded = Resources.Load<RunnerGame.MVC.View.ObstacleView>("Prefabs/Obstacle");
                    if (loaded != null)
                    {
                        obstaclePoolManager.SetObstaclePrefab(loaded);// método helper (ver nota más abajo)
                        Debug.Log("[Bootstrapper] Obstacle prefab cargado desde Resources/Prefabs/Obstacle (colócalo ahí si quieres autoload).");
                    }
                    else
                    {
                        Debug.LogWarning("[Bootstrapper] No se encontró prefab en Resources/Prefabs/Obstacle. Asigna el Obstacle prefab en el PoolManager inspector.");
                    }
                }
            }
        }

        // 7) Inicialización final del playerModel/controller si todo ok
        var playerModel = new RunnerGame.MVC.Model.PlayerModel();
        if (playerController != null)
        {
            // Si tu PlayerController tiene Initialize, llámalo; si no, intenta SetModel
            var initMethod = playerController.GetType().GetMethod("Initialize");
            if (initMethod != null)
            {
                initMethod.Invoke(playerController, new object[] { playerModel, playerViewInScene });
                Debug.Log("[Bootstrapper] PlayerController.Initialize(...) invocado.");
            }
            else
            {
                Debug.Log("[Bootstrapper] PlayerController no tiene Initialize(). Se espera que gestione su propio model internamente.");
            }
        }
        else
        {
            Debug.LogWarning("[Bootstrapper] PlayerController sigue siendo null: crea un Player GameObject con PlayerController o asigna manualmente en el Inspector.");
        }

        Debug.Log("[Bootstrapper] Awake: inicialización completada con estado:");
        Debug.Log($"  config: {(config != null)}, audioSource: {(audioSource != null)}, playerView: {(playerViewInScene != null)}, poolManager: {(obstaclePoolManager != null)}");
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (gameController != null) gameController.Update(dt);

      

        if (obstaclePoolManager != null && gameController != null)
        {
            obstaclePoolManager.SyncWithStore(gameController.GetObstacleStore());
        }
    }
}
