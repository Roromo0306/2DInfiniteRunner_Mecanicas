using UnityEngine;

[CreateAssetMenu(menuName = "Config/GameConfig")]
public class GameConfigSO : ScriptableObject
{
    public float initialObstacleSpawnInterval = 1.2f;
    public float minSpawnInterval = 0.4f;
    public float difficultyRampPerMinute = 0.1f;
    public float gameSpeed = 5f;
    public int pointsPerSecond = 1;
}

