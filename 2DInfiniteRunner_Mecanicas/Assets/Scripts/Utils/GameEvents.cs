using System;

public static class GameEvents
{
    public static Action<int> OnScoreChanged;
    public static Action<float> OnTimeUpdated;
    public static Action OnPlayerDied;
    public static Action<PowerUpType> OnPowerUpPicked;
    public static Action<PowerUpType> OnPowerUpEnded;
    
}
