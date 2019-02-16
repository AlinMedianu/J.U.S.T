using UnityEngine;
using Photon.Pun;

public class DoorController : MonoBehaviour
{
    private bool open;
    [SerializeField]
    private AudioClip[] doorSounds;
    private Animator opener;

    public bool Open
    {
        get
        {
            return open;
        }
    }

    void Start()
    {
        open = false;
        opener = GetComponentInParent<Animator>();
    }

    [PunRPC]
    public void OpenDoor()
    {
        Debug.Log("function on door ran");
        if (!open)
		{
            opener.Play("Open");
            GetComponent<AudioSource>().PlayOneShot(doorSounds[0]);
			open = true;
		}
        else
		{
            opener.Play("Close");
            GetComponent<AudioSource>().PlayOneShot(doorSounds[1]);
			open = false;
		}
    }
}
