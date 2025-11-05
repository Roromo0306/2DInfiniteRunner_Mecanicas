using UnityEngine;

namespace RunnerGame.MVC.View
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class PlayerView : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;

        [Header("Clips de sonido")]
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip deathClip;
        [SerializeField] private AudioClip powerUpClip;

        private void Awake()
        {
            // Autoasignación si no se ha hecho en el Inspector
            if (animator == null) animator = GetComponent<Animator>();
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        public void PlayJump()
        {
            if (animator) animator.SetTrigger("Jump");
            if (audioSource && jumpClip) audioSource.PlayOneShot(jumpClip);
        }

        public void PlayDeath()
        {
            if (animator) animator.SetTrigger("Death");
            if (audioSource && deathClip) audioSource.PlayOneShot(deathClip);
        }

        public void PlayPowerUp()
        {
            if (audioSource && powerUpClip) audioSource.PlayOneShot(powerUpClip);
        }

        public void UpdatePosition(Vector2 newPosition)
        {
            transform.position = newPosition;
        }
    }
}
