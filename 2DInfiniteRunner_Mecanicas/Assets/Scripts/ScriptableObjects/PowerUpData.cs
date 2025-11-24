using UnityEngine;

public enum PowerUpType { Shield, SlowTime, DoublePoints, DoubleJump }

[CreateAssetMenu(menuName = "Runner/PowerUpData")]
public class PowerUpData : ScriptableObject
{
    public string id;
    public PowerUpType type;
    public GameObject prefab;
    public float duration; // seconds
    public AudioClip pickupSound;
}
