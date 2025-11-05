using UnityEngine;

namespace RunnerGame.MVC.View
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ObstacleView : MonoBehaviour
    {
        // Index actual en el ObstacleStoreSoA. -1 si no está asignado.
        public int StoreIndex { get; private set; } = -1;

        // Si quieres animaciones o efectos al activarse/desactivarse
        [Header("Opcionales")]
        [SerializeField] private Animator animator;
        [SerializeField] private ParticleSystem hitEffect;

        private SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Asigna este view al índice i del store (para tracking).
        /// </summary>
        public void AssignIndex(int i)
        {
            StoreIndex = i;
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Libera (desasigna) el view.
        /// </summary>
        public void Release()
        {
            StoreIndex = -1;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Actualiza la posición y escala/anchura visual según datos (llamado desde el pool).
        /// </summary>
        public void SyncToData(Vector2 position, float width)
        {
            transform.position = new Vector3(position.x, position.y, transform.position.z);
            // Ajusta escala X en función del ancho (asume que sprite mide 1 unidad por defecto)
            Vector3 s = transform.localScale;
            s.x = width;
            transform.localScale = s;
        }

        public void PlayHit()
        {
            if (animator) animator.SetTrigger("Hit");
            if (hitEffect) hitEffect.Play();
        }
    }
}

