using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKilledPopUp : MonoBehaviour {

    Animation anim;
    AudioSource audioSource;

    private void Awake()
    {
        anim = GetComponent<Animation>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        anim.Play();
        audioSource.Play();
    }

    public void TurnOff()
    {
        this.gameObject.SetActive(false);
    }
}
