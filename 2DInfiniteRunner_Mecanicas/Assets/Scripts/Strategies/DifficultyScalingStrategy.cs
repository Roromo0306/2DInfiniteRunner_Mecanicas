using UnityEngine;
public class DifficultyScalingStrategy : IObstacleSpawnStrategy
{
    public float GetNextSpawnInterval(float gameTime, float baseInterval)
    {
        float ramp = 1f - Mathf.Clamp01(gameTime / 60f) * 0.6f; // baja el intervalo con el tiempo
        return Mathf.Max(0.3f, baseInterval * ramp);
    }
    public (float width, float speed) GetObstacleParams(float gameTime)
    {
        float speed = 4f + gameTime * 0.05f;
        float width = Random.Range(0.5f, 1.5f);
        return (width, speed);
    }
}