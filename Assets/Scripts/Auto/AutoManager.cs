using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoManager : PersistentSingleton<AutoManager>
{
    [SerializeField] AudioSource sFXPlayer;
    const float MIN_PITCH = 0.9f;
    const float MAX_PITCH = 1.1f;
    public void PlaySFX(AudioDate audioDate)
    {

        sFXPlayer.PlayOneShot(audioDate.audioClip, audioDate.volume);
    }
    public void PlayRandomSFX(AudioDate audioDate)
    {
        sFXPlayer.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
        PlaySFX(audioDate);
    }

    public void PlayRandomSFX(AudioDate[] audioDate)
    {
        PlayRandomSFX(audioDate[Random.Range(0, audioDate.Length)]);
    }
}
[System.Serializable]public class AudioDate
{
    public AudioClip audioClip;
    public float volume;

}
