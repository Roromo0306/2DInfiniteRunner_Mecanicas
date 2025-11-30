using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    public static GameInstaller Instance { get; private set; }

    // referencias de escena que asignarás en inspector
    public AudioSource sfxSource;
    public AudioSource musicSource;
    public AudioLibrary audioLibrary;
    public GameConfig config;
    public PlayerController playerPrefab;
    public GameObject playerObjectInScene;
    public ObstacleSpawner obstacleSpawner;
    public PowerupSpawner powerupSpawner;
    public LifeSpawner lifeSpawner;
    public AudioSource musicSourceInstance;

    // Models expuestos
    public PlayerModel PlayerModel { get; private set; }
    public GameModel GameModel { get; private set; }

    private IEventBus _bus;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        Instance = this;

        // crear e inyectar servicios
        _bus = new EventBus();
        GameContainer.Register<IEventBus>(_bus);

        var audioSrv = new AudioService(sfxSource, musicSource);
        GameContainer.Register<IAudioService>(audioSrv);

        var saveSrv = new PlayerPrefsSaveService();
        GameContainer.Register<ISaveService>(saveSrv);

        GameContainer.Register<AudioLibrary>(audioLibrary);

        // crear modelos
        PlayerModel = new PlayerModel(config.maxLives);
        GameModel = new GameModel();

        // si tienes player pre-instanciado en escena, inicializar
        if (playerObjectInScene != null)
        {
            var pc = playerObjectInScene.GetComponent<PlayerController>();
            if (pc != null) pc.Initialize(PlayerModel, GameModel);
        }

        // iniciar musica
        audioSrv.PlayMusic(audioLibrary.music, 1f);
    }

    void Update()
    {
        // acelerar difficulty: subir velocidad progresivamente con tiempo
        if (GameModel.IsRunning)
        {
            GameModel.TimeElapsed += Time.deltaTime;
            var minutes = GameModel.TimeElapsed / 60f;
            var targetSpeed = Mathf.Min(config.maxSpeed, config.baseSpeed + minutes * config.accelerationPerMinute);
            // Propaga velocidad a spawners / movers: opción sencilla: ajustar obstacleSpawner's spawn rate or set a global multiplier
            // Aquí mandamos un mensaje si alguien lo necesita
            GameContainer.Resolve<IEventBus>().Publish(targetSpeed);
            // ajustar música pitch con la velocidad relativa
            var pitch = 1f + (targetSpeed - config.baseSpeed) / (config.maxSpeed - config.baseSpeed) * 0.5f;
            GameContainer.Resolve<IAudioService>().SetMusicPitch(pitch);
        }
    }
}
