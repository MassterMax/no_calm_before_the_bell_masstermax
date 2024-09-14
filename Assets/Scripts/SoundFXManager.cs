using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
    [SerializeField] AudioSource soundFxObject;
    public static SoundFXManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
    {
        PlaySoundFXClip(audioClip, spawnTransform, volume, audioClip.length);

    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume, float newClipLength)
    {
        AudioSource audioSource = Instantiate(soundFxObject, spawnTransform.position, Quaternion.identity);
        //assign the audioClip
        audioSource.clip = audioClip;
        //assign volume
        audioSource.volume = volume;
        //play sound
        audioSource.Play();
        //get length of sound FX clip
        float clipLength = newClipLength;
        //destroy the clip after it is done playing
        Destroy(audioSource.gameObject, clipLength);

    }
}
