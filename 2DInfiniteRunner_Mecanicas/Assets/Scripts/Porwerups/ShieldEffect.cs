using UnityEngine;

[CreateAssetMenu(menuName = "Runner/PowerUpEffects/Shield")]
public class ShieldEffect : PowerUpEffectBase
{
    public override void Apply(PlayerModel player, GameModel game)
    {
        player.HasShield = true;
    }

    public override void Remove(PlayerModel player, GameModel game)
    {
        player.HasShield = false;
    }
}
