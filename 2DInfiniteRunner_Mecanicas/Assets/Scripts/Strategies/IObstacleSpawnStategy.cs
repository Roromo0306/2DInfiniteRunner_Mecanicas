public interface IObstacleSpawnStrategy
{
    // devuelve intervalo hasta siguiente spawn
    float GetNextSpawnInterval(float gameTime, float baseInterval);
    // devuelve parámetros del obstáculo (por simplicidad solo width y speed)
    (float width, float speed) GetObstacleParams(float gameTime);
}