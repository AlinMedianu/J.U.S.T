using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void SetPitch(float pitch)
    {
        audioSource.pitch = pitch;
    }

    public void SetPitch(float min, float max)
    {
        audioSource.pitch = Random.Range(min, max);
    }

    public void PlaySound(int index)
    {
        audioSource.clip = audioClips[index];
		audioSource.Play();
    }
}
