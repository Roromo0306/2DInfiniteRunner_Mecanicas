using UnityEngine;

[CreateAssetMenu(menuName = "Runner/GameConfig")]
public class GameConfig : ScriptableObject
{
    public float baseSpeed = 5f;
    public float maxSpeed = 20f;
    public float accelerationPerMinute = 2f; // cuanto sube por minuto
    public float spawnX = 10f;
    public float destroyX = -15f;
    public float obstacleMinSpawn = 1f;
    public float obstacleMaxSpawn = 2.5f;
    public float lifeSpawnInterval = 15f;
    public float powerupSpawnInterval = 10f;
    public int maxLives = 3;
    public float countdownSeconds = 3f;
    public float powerupDefaultDuration = 5f;
}
