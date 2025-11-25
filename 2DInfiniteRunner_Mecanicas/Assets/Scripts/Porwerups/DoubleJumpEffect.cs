using UnityEngine;

[CreateAssetMenu(menuName = "Runner/PowerUpEffects/DoubleJump")]
public class DoubleJumpEffect : PowerUpEffectBase
{
    public override void Apply(PlayerModel player, GameModel game)
    {
        player.CanDoubleJump = true;
    }

    public override void Remove(PlayerModel player, GameModel game)
    {
        player.CanDoubleJump = false;
    }
}
