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

    // Multiplicador de score (p. ej. power-up x2)
    float scoreMultiplier = 1f;

    public GameController(GameConfigSO config, IObstacleSpawnStrategy spawnStrategy, float worldLeftX)
    {
        this.config = config;
        this.spawnStrategy = spawnStrategy;
        obstacleStore = new ObstacleStoreSoA(128);
        movementSystem = new MovementSystem(obstacleStore, worldLeftX);

        // Evitar spawn instantáneo justo al arrancar
        spawnTimer = config.initialObstacleSpawnInterval;
        gameTime = 0f;
        score = 0;
        isRunning = true;
    }

    // Llamado cada frame desde el Bootstrapper con Time.deltaTime
    public void Update(float dt)
    {
        if (!isRunning) return;

        // Tiempo global del juego
        gameTime += dt;

        // Spawning
        spawnTimer -= dt;
        if (spawnTimer <= 0f)
        {
            float interval = spawnStrategy.GetNextSpawnInterval(gameTime, config.initialObstacleSpawnInterval);
            spawnTimer = Mathf.Max(0.01f, interval); // evitar 0 por seguridad
            var (width, speed) = spawnStrategy.GetObstacleParams(gameTime);

            // spawn at right side (x), random y within ground height (ejemplo)
            // Ajusta la x,y de spawn según tu juego; aquí se usa X fijo y Y -1 como ejemplo
            obstacleStore.Add(10f, -1.0f, width, speed * config.gameSpeed);
        }

        // Movimiento/actualización de obstáculos (SoA)
        movementSystem.Update(dt);

        // Scoring: sumar por tiempo y multiplicador (asegura entero por tiempo acumulado)
        int newScore = Mathf.FloorToInt(gameTime * config.pointsPerSecond * scoreMultiplier);
        if (newScore != score)
        {
            score = newScore;
            GameEvents.OnScoreChanged?.Invoke(score);
        }

        // Notificar tiempo (float) para el HUD si quieres mostrar mm:ss u otro formato
        GameEvents.OnTimeUpdated?.Invoke(gameTime);
    }

    // Llamar cuando el jugador muere
    public void OnPlayerDied()
    {
        isRunning = false;
        GameEvents.OnPlayerDied?.Invoke();
        // opcional: si tienes un evento GameOver con la puntuación:
        // GameEvents.OnGameOver?.Invoke(score);
    }

    // Exponer store para que view/pool pueda sincronizar visualmente
    public ObstacleStoreSoA GetObstacleStore() => obstacleStore;

    // Control run/reset desde Bootstrapper o UI
    public void ResetGame()
    {
        // vaciar store
        //obstacleStore.Clear(); // asume que tienes Clear(); si no, implementa RemoveAll
       // movementSystem.Reset(); // asume que MovementSystem soporta reset; si no, ajusta

        spawnTimer = config.initialObstacleSpawnInterval;
        gameTime = 0f;
        score = 0;
        scoreMultiplier = 1f;
        isRunning = true;

        GameEvents.OnScoreChanged?.Invoke(score);
        GameEvents.OnTimeUpdated?.Invoke(gameTime);
    }

    public void StartGame()
    {
        isRunning = true;
    }

    public void StopGame()
    {
        isRunning = false;
    }

    public int GetScore() => score;
    public float GetElapsedTime() => gameTime;

    // Método para power-ups que modifican la puntuación
    public void SetScoreMultiplier(float mult)
    {
        scoreMultiplier = Mathf.Max(0f, mult);
    }
}
