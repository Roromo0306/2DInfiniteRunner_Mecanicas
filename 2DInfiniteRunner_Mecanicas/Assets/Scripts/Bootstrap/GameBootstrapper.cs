// Assets/_Project/Scripts/Bootstrap/GameBootstrapper.cs
using UnityEngine;
using RunnerGame.MVC.View;
using RunnerGame.Systems;
using RunnerGame.MVC.Controller;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Config / Assets")]
    public GameConfigSO config;
    public PowerUpSO[] powerUps;

    [Header("Scene refs (preferible asignar en Inspector)")]
    public PlayerView playerViewInScene;        // arrastra Player (o su componente PlayerView)
    public ObstacleView obstaclePrefab;         // asigna Prefab Obstacle
    public PoolManager obstaclePoolManager;     // opcional: si ya lo creaste, arrástralo

    [Header("Audio")]
    public AudioSource audioSource;             // opcional (busca si no asignado)

    [Header("World")]
    public float worldLeftX = -12f;

    // runtime
    IAudioService audioService;
    GameController gameController;
    PlayerController playerController;

    void Awake()
    {
        // 1) Validaciones básicas
        if (config == null)
        {
            Debug.LogError("[Bootstrapper] GameConfigSO (config) no asignado en el Inspector. Asigna el asset y reinicia.");
            enabled = false;
            return;
        }

        // 2) AudioService: usa audioSource del inspector o busca uno en escena
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>() ?? FindObjectOfType<AudioSource>();
        }
        audioService = new AudioService(audioSource);

        // 3) Crear GameController (clase POCO)
        var spawnStrategy = new DifficultyScalingStrategy();
        gameController = new GameController(config, spawnStrategy, worldLeftX);

        // 4) Asegurar PlayerView existe (si no está asignado, intentar buscar)
        if (playerViewInScene == null)
        {
            playerViewInScene = FindObjectOfType<PlayerView>();
            if (playerViewInScene == null)
            {
                Debug.LogError("[Bootstrapper] No se encontró PlayerView en la escena y no fue asignado en Inspector. Crea el Player (con PlayerView) o arrástralo al campo playerViewInScene.");
                enabled = false;
                return;
            }
        }

        // 5) Asegurar PlayerController (componente) en el mismo GameObject que PlayerView
        playerController = playerViewInScene.GetComponent<PlayerController>();
        if (playerController == null)
        {
            // Añadimos el componente si no existía
            playerController = playerViewInScene.gameObject.AddComponent<PlayerController>();
        }

        // 6) PoolManager: si no hay referencia en inspector, buscar/crear
        if (obstaclePoolManager == null)
        {
            obstaclePoolManager = FindObjectOfType<PoolManager>();
            if (obstaclePoolManager == null)
            {
                var poolGO = new GameObject("ObstaclePool");
                obstaclePoolManager = poolGO.AddComponent<PoolManager>();
            }
        }

        // 7) Asignar prefab al PoolManager (necesario). Si no hay prefab, error claro.
        if (obstaclePrefab == null)
        {
            Debug.LogError("[Bootstrapper] obstaclePrefab no asignado. Arrastra Prefab 'Obstacle' al campo obstaclePrefab en el Bootstrapper.");
            enabled = false;
            return;
        }
        // intentamos asignar la referencia pública/serializada del pool
        // (PoolManager debe exponer un campo público o serializado llamado 'obstaclePrefab' o método SetObstaclePrefab)
        // Intentamos asignarlo por reflexión si el campo es privado serializado (fallback).
        bool assigned = false;
        var poolType = obstaclePoolManager.GetType();
        var field = poolType.GetField("obstaclePrefab", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        if (field != null && field.FieldType == typeof(ObstacleView))
        {
            field.SetValue(obstaclePoolManager, obstaclePrefab);
            assigned = true;
        }
        else
        {
            // intentar propiedad pública
            var prop = poolType.GetProperty("ObstaclePrefab", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            if (prop != null && prop.PropertyType == typeof(ObstacleView) && prop.CanWrite)
            {
                prop.SetValue(obstaclePoolManager, obstaclePrefab);
                assigned = true;
            }
        }

        if (!assigned)
        {
            Debug.LogWarning("[Bootstrapper] No se pudo asignar obstaclePrefab al PoolManager por reflexión. Asegúrate de que PoolManager tiene un campo serializado 'obstaclePrefab' o una propiedad pública 'ObstaclePrefab'. De todas formas, si PoolManager fue creado por ti en la escena, asigna manualmente el prefab en su Inspector.");
        }

        // 8) Inicializar posible UI/HUD si lo tienes vía eventos (no obligatorio aquí)

        // 9) Inicialización completa
        Debug.Log("[Bootstrapper] Inicialización completada correctamente.");
    }

    void Update()
    {
        float dt = Time.deltaTime;

        // 1) Lógica principal (timer, score, spawns, etc.)
        if (gameController != null) gameController.Update(dt);

        // 2) Actualizar player controller (controlamos orden desde aquí)
        if (playerController != null) playerController.UpdateController(dt);

        // 3) Sincronizar vistas de obstáculos con datos
        if (obstaclePoolManager != null && gameController != null)
        {
            // GameController debe exponer GetObstacleStore()
            var storeMethod = typeof(GameController).GetMethod("GetObstacleStore");
            if (storeMethod != null)
            {
                var store = storeMethod.Invoke(gameController, null);
                if (store != null)
                {
                    // llamamos a SyncWithStore si existe
                    var syncMethod = obstaclePoolManager.GetType().GetMethod("SyncWithStore");
                    if (syncMethod != null)
                    {
                        syncMethod.Invoke(obstaclePoolManager, new object[] { store });
                    }
                }
            }
        }
    }
}
