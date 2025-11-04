using UnityEngine;

public class PlayerModel
{
    public float x = -3f;
    public float y = 0f;
    public float width = 0.8f;
    public float height = 1.6f;
    public bool isAlive = true;
    public bool hasPowerUp = false;
}

public class PlayerController
{
    PlayerModel model;
    float jumpVelocity = 7f;
    float verticalSpeed = 0f;
    float gravity = -20f;
    float groundY = 0f;
    public PlayerController(PlayerModel model) { this.model = model; }

    public void Update(float dt, ObstacleStoreSoA obstacles)
    {
        // simple input (hook via DI to input system, here pseudo)
        if (Input.GetButtonDown("Jump") && Mathf.Abs(model.y - groundY) < 0.01f)
        {
            verticalSpeed = jumpVelocity;
        }

        verticalSpeed += gravity * dt;
        model.y += verticalSpeed * dt;
        if (model.y < groundY) { model.y = groundY; verticalSpeed = 0f; }

        // Colisión AABB simplificado con SoA
        for (int i = 0; i < obstacles.count; i++)
        {
            float ox = obstacles.posX[i];
            float oy = obstacles.posY[i];
            float ow = obstacles.widths[i];
            // AABB check: player rect vs obstacle rect
            if (RectOverlap(model.x - model.width / 2, model.y, model.width, model.height,
                            ox - ow / 2, oy, ow, 1f))
            {
                model.isAlive = false;
                GameEvents.OnPlayerDied?.Invoke();
                break;
            }
        }
    }

    bool RectOverlap(float ax, float ay, float aw, float ah, float bx, float by, float bw, float bh)
    {
        return (ax < bx + bw) && (ax + aw > bx) && (ay < by + bh) && (ay + ah > by);
    }
}