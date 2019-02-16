using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PlayerInterface))]
[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{

    PlayerInterface playerInterface;
    private AudioSource soundSource;

    public AudioClip flashlightSound;

    private void Awake()
    {
        playerInterface = GetComponent<PlayerInterface>();
        soundSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        playerInterface.ToggleFlashlight += PlayFlashlightClick;
    }

    private void OnDisable()
    {
        playerInterface.ToggleFlashlight -= PlayFlashlightClick;
    }

    public void PlayFlashlightClick()
    {
        soundSource.pitch = 1.0f;
        soundSource.PlayOneShot(flashlightSound);
    }
}
