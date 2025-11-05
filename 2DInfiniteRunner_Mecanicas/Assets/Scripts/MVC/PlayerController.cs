// Assets/_Project/Scripts/MVC/Controller/PlayerController.cs
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
        }

        private void Start()
        {
            if (rb != null) rb.freezeRotation = true;
        }

        private void Update()
        {
            UpdateController(Time.deltaTime);
        }

        public void UpdateController(float deltaTime)
        {
            if (Input.GetButtonDown("Jump") || Input.GetKeyDown(KeyCode.Space))
            {
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
            }
            else if (canDoubleJump && !didDoubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                didDoubleJump = true;
                view?.PlayJump();
            }
        }

        private bool IsGrounded()
        {
            if (groundCheck == null)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.11f, groundMask);
                return hit.collider != null;
            }
            return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
        }

        public void SetDoubleJump(bool enabled)
        {
            canDoubleJump = enabled;
            if (!enabled) didDoubleJump = false;
        }

        public void Kill()
        {
            if (rb != null) rb.velocity = Vector2.zero;
            view?.PlayDeath();
            GameEvents.OnPlayerDied?.Invoke();
        }
    }
}
