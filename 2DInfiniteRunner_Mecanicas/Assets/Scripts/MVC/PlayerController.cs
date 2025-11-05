using UnityEngine;
using RunnerGame.MVC.View;

namespace RunnerGame.MVC.Controller
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Settings")]
        public float jumpForce = 9f;
        public LayerMask groundMask;
        public Transform groundCheck;
        public float groundCheckRadius = 0.12f;

        private Rigidbody2D rb;
        private PlayerView view;

        private bool canDoubleJump = false;
        private bool didDoubleJump = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            view = GetComponent<PlayerView>();
            Debug.Log($"[PlayerController] Awake. rb? {(rb != null)}, view? {(view != null)}");
        }

        private void Start()
        {
            Debug.Log($"[PlayerController] Start pos={transform.position}, rb.gravityScale={rb.gravityScale}, rb.bodyType={rb.bodyType}, rb.isKinematic={rb.isKinematic}");
            if (rb != null) rb.freezeRotation = true;
            if (groundCheck == null) Debug.LogWarning("[PlayerController] groundCheck no asignado. Crea un child y asignalo en inspector.");
        }

        private void Update()
        {
            // Input debug
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("[PlayerController] Input Jump detected. IsGrounded? " + IsGrounded());
                TryJump();
            }
        }

        private void TryJump()
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                didDoubleJump = false;
                view?.PlayJump();
                Debug.Log("[PlayerController] Jump executed. rb.velocity=" + rb.velocity);
            }
            else if (canDoubleJump && !didDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                didDoubleJump = true;
                view?.PlayJump();
                Debug.Log("[PlayerController] DoubleJump executed. rb.velocity=" + rb.velocity);
            }
            else
            {
                Debug.Log("[PlayerController] Jump attempted but not grounded and no double jump available.");
            }
        }

        private bool IsGrounded()
        {
            if (groundCheck == null)
            {
                // fallback: pequeña raycast hacia abajo
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.11f, groundMask);
                bool grounded = hit.collider != null;
                Debug.DrawRay(transform.position, Vector3.down * 0.11f, grounded ? Color.green : Color.red, 0.5f);
                return grounded;
            }

            bool result = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
            Debug.DrawRay(groundCheck.position, Vector3.down * 0.02f, result ? Color.green : Color.red, 0.5f);
            return result;
        }

        public void SetDoubleJump(bool enabled)
        {
            canDoubleJump = enabled;
            if (!enabled) didDoubleJump = false;
        }

        public void Kill()
        {
            rb.velocity = Vector2.zero;
            view?.PlayDeath();
            GameEvents.OnPlayerDied?.Invoke();
        }
    }
}
