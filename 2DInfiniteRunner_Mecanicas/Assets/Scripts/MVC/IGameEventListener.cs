namespace RunnerGame.Core
{
    public enum GameEventType
    {
        ScoreUpdated,
        PowerUpProgress,
        GameOver,
        TimeUpdated
    }

    public interface IGameEventListener
    {
        void OnEventRaised(GameEventType type, object data);
    }
}
