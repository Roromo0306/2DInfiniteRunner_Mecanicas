using UnityEngine;

public enum PowerUpType { Shield, DoublePoints, SlowTime }

[CreateAssetMenu(menuName = "Config/PowerUp")]
public class PowerUpSO : ScriptableObject
{
    public PowerUpType type;
    public float duration = 5f;
    public Sprite icon;
    public AudioClip pickupSfx;
}
