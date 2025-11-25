public interface IPowerUpEffect
{
    void Apply(PlayerModel player, GameModel game);
    void Remove(PlayerModel player, GameModel game);
}
