using UnityEngine;

public enum PowerUpType { Shield, SlowTime, DoublePoints, DoubleJump }

[CreateAssetMenu(menuName = "Runner/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    [Tooltip("Id único")]
    public string id;

    public PowerUpType type;

    [Tooltip("Prefab que se instancia en escena (con collider y PowerupPickup)")]
    public GameObject prefab;

    [Tooltip("Duración del powerup en segundos (si 0 usa el default del GameConfig)")]
    public float duration;

    [Tooltip("Sonido al recoger")]
    public AudioClip pickupSound;

    [Tooltip("Efecto (Strategy) asociado a este PowerUp")]
    public PowerUpEffectBase effect;
}
