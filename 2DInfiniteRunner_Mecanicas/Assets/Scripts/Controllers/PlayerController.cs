using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float jumpForce = 7f;
    public AudioClip jumpClip;
    private Rigidbody2D _rb;
    private PlayerModel _model;
    private GameModel _game;
    private bool _grounded = true;
    private int _jumpCount = 0;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(PlayerModel model, GameModel game)
    {
        _model = model;
        _game = game;
    }

    void Update()
    {
        if (!_game.IsRunning) return;

        // salto con espacio / touch / click
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            TryJump();
        }
    }

    void TryJump()
    {
        if (_grounded)
        {
            Jump();
            _jumpCount = 1;
        }
        else if (_model != null && _model.CanDoubleJump && _jumpCount < 2)
        {
            Jump();
            _jumpCount++;
        }
    }

    void Jump()
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0f);
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        var audio = GameContainer.Resolve<IAudioService>();
        audio.PlayOneShot(jumpClip);
        GameContainer.Resolve<IEventBus>().Publish(new PlayerJumpedEvent());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            _grounded = true;
            _jumpCount = 0;
        }
        else if (collision.collider.CompareTag("Obstacle"))
        {
            HandleHit();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Colliders fuera de pantalla marcados con tag "DeathZone"
        if (other.CompareTag("DeathZone"))
        {
            // muerte instantánea
            _model.LoseLife();
            GameContainer.Resolve<IEventBus>().Publish(new PlayerHurtEvent { remainingLives = _model.Lives });
            if (_model.Lives <= 0)
            {
                GameOver();
            }
            else
            {
                var audio = GameContainer.Resolve<IAudioService>();
                audio.PlayOneShot(GameContainer.Resolve<AudioLibrary>().hurt);
            }
        }
    }

    void HandleHit()
    {
        _model.LoseLife();
        GameContainer.Resolve<IEventBus>().Publish(new PlayerHurtEvent { remainingLives = _model.Lives });
        var audio = GameContainer.Resolve<IAudioService>();
        if (_model.Lives <= 0)
        {
            audio.PlayOneShot(GameContainer.Resolve<AudioLibrary>().death);
            GameOver();
        }
        else
        {
            audio.PlayOneShot(GameContainer.Resolve<AudioLibrary>().hurt);
        }
    }

    void GameOver()
    {
        _game.IsRunning = false;
        GameContainer.Resolve<IEventBus>().Publish(new GameOverEvent { finalScore = _game.Score });
    }
}
