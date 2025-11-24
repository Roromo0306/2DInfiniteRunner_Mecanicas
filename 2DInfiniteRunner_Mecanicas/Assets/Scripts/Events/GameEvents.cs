public struct PlayerJumpedEvent { }
public struct PlayerHurtEvent { public int remainingLives; }
public struct PlayerDiedEvent { public int finalScore; }
public struct PowerUpCollectedEvent { public string powerUpId; }
public struct LifeCollectedEvent { public int newLives; }
public struct GameStartedEvent { }
public struct GamePausedEvent { }
public struct GameResumedEvent { }
public struct GameOverEvent { public int finalScore; }
