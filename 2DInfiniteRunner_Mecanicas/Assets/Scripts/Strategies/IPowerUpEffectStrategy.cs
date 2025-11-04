public interface IPowerUpEffectStrategy
{
    void Apply(PlayerModel player, float duration);
    void Remove(PlayerModel player);
}