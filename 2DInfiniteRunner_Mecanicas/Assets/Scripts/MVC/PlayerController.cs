using UnityEngine;
using RunnerGame.MVC.Model;
using RunnerGame.MVC.View;

namespace RunnerGame.MVC.Controller
{
    public class PlayerController : MonoBehaviour
    {
        private PlayerModel model;
        private PlayerView view;

        public void Initialize(PlayerModel model, PlayerView view)
        {
            this.model = model;
            this.view = view;
        }

        public void UpdateController(float deltaTime)
        {
            if (model == null || !model.IsAlive) return;

            float horizontalInput = Input.GetAxis("Horizontal");

            if (Input.GetButtonDown("Jump"))
            {
                model.Jump();
                view.PlayJump();
            }

            model.Move(horizontalInput);
            view.UpdatePosition(model.Velocity * deltaTime);
        }

        public void Die()
        {
            model.Die();
            view.PlayDeath();
        }
    }
}
