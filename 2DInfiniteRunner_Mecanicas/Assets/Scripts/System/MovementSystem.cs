using UnityEngine;

public class MovementSystem
{
    private ObstacleStoreSoA obstacles;
    private float worldLeftX;

    public MovementSystem(ObstacleStoreSoA store, float worldLeftX)
    {
        this.obstacles = store;
        this.worldLeftX = worldLeftX;
    }

    public void Update(float deltaTime)
    {
        for (int i = obstacles.count - 1; i >= 0; i--)
        {
            obstacles.posX[i] -= obstacles.speeds[i] * deltaTime;
            if (obstacles.posX[i] + obstacles.widths[i] < worldLeftX)
            {
                obstacles.RemoveAt(i);
            }
        }
    }
}

