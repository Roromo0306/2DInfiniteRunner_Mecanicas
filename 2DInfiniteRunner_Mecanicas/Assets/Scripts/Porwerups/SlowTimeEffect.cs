using UnityEngine;

[CreateAssetMenu(menuName = "Runner/PowerUpEffects/SlowTime")]
public class SlowTimeEffect : PowerUpEffectBase
{
    public float timeScale = 0.5f;

    public override void Apply(PlayerModel player, GameModel game)
    {
        Time.timeScale = timeScale;
    }

    public override void Remove(PlayerModel player, GameModel game)
    {
        Time.timeScale = 1f;
    }
}
