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


    public void OnAnimFootstepPlay() //Call From Animation Event
    {
        PlayFootStepSound();
    }

    public void PlayFootStepSound()
    {
        if (FootstepAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (FootstepClips.Length == 0) { Debug.LogError("Footsteps Audio Clips Missing", gameObject); return; }

        AudioClip audioToPlay = FootstepClips[Random.Range(0, FootstepClips.Length)];

        PlaySound(audioToPlay, FootstepAudioSource);
    }

    public void PlayGunShootSound()
    {
        if (GunAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (GunShootClips.Length == 0) { Debug.LogError("Gun Audio Clips Missing", gameObject); return; }

        AudioClip audioToPlay = GunShootClips[Random.Range(0, GunShootClips.Length)];

        PlaySound(audioToPlay, GunAudioSource);
    }

    public void PlayReloadSound()
    {
        if (CommonAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (!ReloadClip) { Debug.LogError("Reload Audio Clips Missing", gameObject); return; }

        PlaySound(ReloadClip, CommonAudioSource);
    }

    public void PlayDeathSound()
    {
        if (CommonAudioSource == null) { Debug.LogError("Audio Source not Found", gameObject); return; }

        if (!DeathClip) { Debug.LogError("Death Audio Clips Missing", gameObject); return; }

        PlaySound(DeathClip, CommonAudioSource);
    }

    private void PlaySound(AudioClip clip, AudioSource source)
    {
        source.clip = clip;
        source.Play();
    }












}
