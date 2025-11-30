using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM && UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;

    [Header("Audio")]
    public AudioClip jumpClip;

    // Estados/modelos (si GameInstaller llama Initialize, se usan; si no, se sigue funcionando)
    private PlayerModel _model;
    private GameModel _game;

    // Internos
    private Rigidbody2D _rb;
    private bool _grounded = false;
    private int _jumpCount = 0;
    private bool _initialized = false;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

        // si no asignaste groundCheck en inspector, lo intentamos crear en runtime
        if (groundCheck == null)
        {
            var go = new GameObject("GroundCheck");
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(0f, -0.5f, 0f);
            groundCheck = go.transform;
        }

        // No asumimos que Initialize se llamó; intentamos obtener modelos si GameInstaller ya existe.
        TryAutoInitialize();
    }

    void TryAutoInitialize()
    {
        if (_initialized) return;
        if (GameInstaller.Instance != null)
        {
            _model = GameInstaller.Instance.PlayerModel;
            _game = GameInstaller.Instance.GameModel;
            _initialized = true;
        }
    }

    public void Initialize(PlayerModel model, GameModel game)
    {
        _model = model;
        _game = game;
        _initialized = true;
    }

    void Update()
    {
        // siempre intentar inicializar si todavía no se hizo (por orden de creación de objetos)
        TryAutoInitialize();

        // Evitar que la UI capture la tecla si hay algo seleccionado
        if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
        {
            // NO salimos aquí porque queremos aceptar clicks/taps incluso si hay UI, 
            // pero quitamos el foco para que teclas no queden "enganchadas"
            EventSystem.current.SetSelectedGameObject(null);
        }

        // Si tienes GameModel y quieres bloquear input hasta que GameModel.IsRunning==true, descomenta:
        // if (_game != null && !_game.IsRunning) return;

        // Comprobación de grounded mediante OverlapCircle (más fiable que confiar sólo en colisiones)
        _grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (_grounded) _jumpCount = 0;

        // Leer input robustamente (soporte antiguo + nuevo Input System si está presente)
        bool pressed = false;

        // 1) Input antiguo
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            pressed = true;

        // 2) Flecha arriba y W (opcional)
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            pressed = true;

        // 3) Touch (en mobile)
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began) { pressed = true; break; }
            }
        }

        // 4) Nuevo Input System (si está activo)
#if ENABLE_INPUT_SYSTEM && UNITY_INPUT_SYSTEM
        if (!pressed && Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.upArrowKey.wasPressedThisFrame ||
                Keyboard.current.wKey.wasPressedThisFrame)
            {
                pressed = true;
            }
        }

        // también toque con Input System (touchscreen)
        if (!pressed && UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
        {
            // comprobar si hay un toque que empezó este frame
            foreach (var t in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
            {
                if (t.phase == UnityEngine.InputSystem.TouchPhase.Began) { pressed = true; break; }
            }
        }
#endif

        if (pressed)
        {
            TryJump();
        }
    }

    void TryJump()
    {
        // Si hay modelo y quieres respetar vidas/estado, puedes comprobarlo aquí.
        // Por ejemplo: if (_model != null && _model.Lives <= 0) return;

        // doble salto si está permitido
        bool canDoubleJump = _model != null && _model.CanDoubleJump;

        if (_grounded)
        {
            DoJump();
            _jumpCount = 1;
        }
        else if (canDoubleJump && _jumpCount < 2)
        {
            DoJump();
            _jumpCount++;
        }
    }

    void DoJump()
    {
        // reset vertical velocidad y aplicar impulso
        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        // Sonido
        if (jumpClip != null)
        {
            var audio = TryResolveAudioService();
            if (audio != null) audio.PlayOneShot(jumpClip);
            else AudioSource.PlayClipAtPoint(jumpClip, transform.position);
        }
    }

    IAudioService TryResolveAudioService()
    {
        try
        {
            if (GameContainer.IsRegistered<IAudioService>())
                return GameContainer.Resolve<IAudioService>();
        }
        catch { }
        return null;
    }

    // Opcional: visualizar el groundCheck en el Editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
