using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioHandler : MonoBehaviour
{
    [Header("Audio Source")]
    public AudioSource FootstepAudioSource;
    public AudioSource CommonAudioSource;
    public AudioSource GunAudioSource;

    [Header("Sound Clips")]
    public AudioClip ReloadClip;
    public AudioClip DeathClip;

    public AudioClip[] FootstepClips;
    public AudioClip[] GunShootClips;

    /// <summary>
    /// Called from an Animation Event to play footstep sounds.
    /// </summary>
    public void OnAnimFootstepPlay()
    {
        PlayFootStepSound();
    }

    /// <summary>
    /// Plays a random footstep sound.
    /// </summary>
    private void PlayFootStepSound()
    {
        if (FootstepAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (FootstepClips.Length == 0) { Debug.LogError("Footsteps Audio Clips Missing", gameObject); return; }

        AudioClip audioToPlay = FootstepClips[Random.Range(0, FootstepClips.Length)];

        PlaySound(audioToPlay, FootstepAudioSource);
    }

    /// <summary>
    /// Plays a random gunshot sound.
    /// </summary>
    public void PlayGunShootSound()
    {
        if (GunAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (GunShootClips.Length == 0) { Debug.LogError("Gun Audio Clips Missing", gameObject); return; }

        AudioClip audioToPlay = GunShootClips[Random.Range(0, GunShootClips.Length)];

        PlaySound(audioToPlay, GunAudioSource);
    }

    /// <summary>
    /// Plays the reload sound.
    /// </summary>
    public void PlayReloadSound()
    {
        if (CommonAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (!ReloadClip) { Debug.LogError("Reload Audio Clips Missing", gameObject); return; }

        PlaySound(ReloadClip, CommonAudioSource);
    }

    /// <summary>
    /// Plays the death sound.
    /// </summary>
    public void PlayDeathSound()
    {
        if (CommonAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (!DeathClip) { Debug.LogError("Death Audio Clips Missing", gameObject); return; }

        PlaySound(DeathClip, CommonAudioSource);
    }

    /// <summary>
    /// Plays a specified audio clip on the provided audio source.
    /// </summary>
    private void PlaySound(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }












}
