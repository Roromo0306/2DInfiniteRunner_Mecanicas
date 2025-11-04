using UnityEngine;
using System;

public class GameController
{
    readonly GameConfigSO config;
    readonly ObstacleStoreSoA obstacleStore;
    readonly MovementSystem movementSystem;
    readonly IObstacleSpawnStrategy spawnStrategy;
    float spawnTimer = 0f;
    float gameTime = 0f;
    int score = 0;
    bool isRunning = true;

    public GameController(GameConfigSO config, IObstacleSpawnStrategy spawnStrategy, float worldLeftX)
    {
        this.config = config;
        this.spawnStrategy = spawnStrategy;
        obstacleStore = new ObstacleStoreSoA(128);
        movementSystem = new MovementSystem(obstacleStore, worldLeftX);
    }

    public void Update(float dt)
    {
        if (!isRunning) return;
        gameTime += dt;
        spawnTimer -= dt;
        if (spawnTimer <= 0f)
        {
            float interval = spawnStrategy.GetNextSpawnInterval(gameTime, config.initialObstacleSpawnInterval);
            spawnTimer = interval;
            var (width, speed) = spawnStrategy.GetObstacleParams(gameTime);
            // spawn at right side (x), random y within ground height (ejemplo)
            obstacleStore.Add(10f, -1.0f, width, speed * config.gameSpeed);
        }

        movementSystem.Update(dt);

        // scoring: sumar por tiempo
        int newScore = (int)(gameTime * config.pointsPerSecond);
        if (newScore != score)
        {
            score = newScore;
            GameEvents.OnScoreChanged?.Invoke(score);
        }
    }

    public void OnPlayerDied()
    {
        isRunning = false;
        GameEvents.OnPlayerDied?.Invoke();
    }

    // Exponer store para que view/pool pueda sincronizar visualmente
    public ObstacleStoreSoA GetObstacleStore() => obstacleStore;
}