using UnityEngine;

public interface IAudioService : IService
{
    void PlayOneShot(AudioClip clip);
    void PlayMusic(AudioClip clip, float targetPitch = 1f);
    void SetMusicPitch(float pitch);
    void StopMusic();
}
