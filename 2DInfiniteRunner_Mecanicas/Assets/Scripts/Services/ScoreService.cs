using UnityEngine;

public class ScoreService : IService
{
    private GameModel _game;
    private PlayerModel _player;

    public ScoreService(GameModel game, PlayerModel player)
    {
        _game = game;
        _player = player;
    }

    public void Tick(float deltaTime)
    {
        // cada segundo añade puntos (por ejemplo 10 pts por segundo)
        var pointsPerSecond = 10f;
        var add = Mathf.FloorToInt(pointsPerSecond * deltaTime * _player.ScoreMultiplier);
        _game.Score += add;
    }
}
