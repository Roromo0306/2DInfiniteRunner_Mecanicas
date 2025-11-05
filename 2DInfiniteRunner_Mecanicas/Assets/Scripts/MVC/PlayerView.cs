using UnityEngine;

namespace RunnerGame.MVC.View
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerView : MonoBehaviour
    {
        [Header("Anim & Audio")]
        [SerializeField] private Animator animator;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip deathClip;
        [SerializeField] private AudioClip powerUpClip;

        private void Awake()
        {
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        public void PlayJump()
        {
            Debug.Log("[PlayerView] PlayJump() - jumpClip null? " + (jumpClip == null));
            if (animator) animator.SetTrigger("Jump");
            if (audioSource != null && jumpClip != null)
            {
                audioSource.PlayOneShot(jumpClip);
            }
        }

        public void PlayDeath()
        {
            Debug.Log("[PlayerView] PlayDeath()");
            if (animator) animator.SetTrigger("Death");
            if (audioSource != null && deathClip != null) audioSource.PlayOneShot(deathClip);
        }

        public void PlayPowerUp()
        {
            Debug.Log("[PlayerView] PlayPowerUp()");
            if (audioSource != null && powerUpClip != null) audioSource.PlayOneShot(powerUpClip);
        }
    }
}
