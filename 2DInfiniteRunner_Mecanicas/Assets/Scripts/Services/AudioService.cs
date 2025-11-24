using UnityEngine;

public class AudioService : IAudioService
{
    private AudioSource _sfxSource;
    private AudioSource _musicSource;

    public AudioService(AudioSource sfxSource, AudioSource musicSource)
    {
        _sfxSource = sfxSource;
        _musicSource = musicSource;
    }

    public void PlayOneShot(AudioClip clip)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, float targetPitch = 1f)
    {
        if (_musicSource.clip == clip) { _musicSource.pitch = targetPitch; if (!_musicSource.isPlaying) _musicSource.Play(); return; }
        _musicSource.clip = clip;
        _musicSource.loop = true;
        _musicSource.pitch = targetPitch;
        _musicSource.Play();
    }

    public void SetMusicPitch(float pitch)
    {
        _musicSource.pitch = pitch;
    }

    public void StopMusic()
    {
        _musicSource.Stop();
    }
}
