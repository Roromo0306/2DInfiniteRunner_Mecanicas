using UnityEngine;
public class PlayerModel
{
    public int Lives { get; private set; }
    public int MaxLives { get; private set; }
    public bool HasShield { get; set; }
    public bool CanDoubleJump { get; set; }
    public float ScoreMultiplier { get; set; } = 1f;

    public PlayerModel(int maxLives)
    {
        MaxLives = maxLives;
        Lives = maxLives;
        HasShield = false;
        CanDoubleJump = false;
    }

    public void LoseLife()
    {
        if (HasShield)
        {
            HasShield = false;
            return;
        }
        Lives = Mathf.Max(0, Lives - 1);
    }

    public void AddLife()
    {
        Lives = Mathf.Min(MaxLives, Lives + 1);
    }

    public void ResetLives()
    {
        Lives = MaxLives;
        HasShield = false;
        CanDoubleJump = false;
        ScoreMultiplier = 1f;
    }
}
