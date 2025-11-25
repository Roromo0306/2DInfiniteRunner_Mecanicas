using UnityEngine;

[CreateAssetMenu(menuName = "Runner/AudioLibrary")]
public class AudioLibrary : ScriptableObject
{
    public AudioClip jump;
    public AudioClip death;
    public AudioClip hurt;
    public AudioClip pickupLife;
    public AudioClip pickupPowerup;
    public AudioClip music;
}
