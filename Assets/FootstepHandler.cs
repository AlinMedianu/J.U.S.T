using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepHandler : MonoBehaviour
{
	public AudioSource audioSource;
	public AudioClip[] footsteps;

	public void PlayFootstep()
	{
		int footstepNum = Random.Range(0, footsteps.Length);
		audioSource.PlayOneShot(footsteps[footstepNum]);
	}
}
