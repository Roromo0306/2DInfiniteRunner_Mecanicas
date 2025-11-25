using UnityEngine;

[CreateAssetMenu(menuName = "Runner/PowerUpEffects/DoublePoints")]
public class DoublePointsEffect : PowerUpEffectBase
{
    public float multiplier = 2f;

    public override void Apply(PlayerModel player, GameModel game)
    {
        player.ScoreMultiplier = multiplier;
    }

    public override void Remove(PlayerModel player, GameModel game)
    {
        player.ScoreMultiplier = 1f;
    }
}
