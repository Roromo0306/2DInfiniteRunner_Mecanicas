using UnityEngine;

public abstract class PowerUpEffectBase : ScriptableObject, IPowerUpEffect
{
    public abstract void Apply(PlayerModel player, GameModel game);
    public abstract void Remove(PlayerModel player, GameModel game);
}
