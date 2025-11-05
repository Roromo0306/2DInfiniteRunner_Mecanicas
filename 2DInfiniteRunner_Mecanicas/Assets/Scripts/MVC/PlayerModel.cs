using UnityEngine;

namespace RunnerGame.MVC.Model
{
    [System.Serializable]
    public class PlayerModel
    {
        // Datos del jugador (el "estado")
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float Speed { get; private set; } = 5f;
        public float jumpForce { get; private set; } = 8f;

        // Estado del jugador
        public bool IsAlive { get; private set; } = true;
        public bool HasPowerUp { get; private set; } = false;

        // Power-Up actual
        public PowerUpSO ActivePowerUp { get; private set; }

        public PlayerModel()
        {
            Position = Vector2.zero;
            Velocity = Vector2.zero;
        }

        public void Move(float horizontalInput)
        {
            Velocity = new Vector2(horizontalInput * Speed, Velocity.y);
        }

        public void Jump()
        {
            Velocity = new Vector2(Velocity.x, jumpForce);
        }

        public void Die()
        {
            IsAlive = false;
        }

        public void ActivatePowerUp(PowerUpSO powerUp)
        {
            ActivePowerUp = powerUp;
            HasPowerUp = true;
        }

        public void DeactivatePowerUp()
        {
            ActivePowerUp = null;
            HasPowerUp = false;
        }
    }
}

