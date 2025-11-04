using UnityEngine;

public interface IAudioService
{
    void PlayOneShot(AudioClip clip);
}

public class AudioService : IAudioService
{
    AudioSource s;
    public AudioService(AudioSource source) { s = source; }
    public void PlayOneShot(AudioClip clip) { if (clip != null) s.PlayOneShot(clip); }
}
