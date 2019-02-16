using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerAnimations : MonoBehaviourPunCallbacks, IPunObservable
{
    private Animator anim;
    private float leftRightBlend = 0.0f;
    private float forwardBackBlend = 0.0f;
    private float animationSpeed = 1.0f;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(anim.GetBool("walking"));
            stream.SendNext(anim.GetFloat("Left-Right Blend"));
            stream.SendNext(anim.GetFloat("Forward-Back Blend"));
            stream.SendNext(anim.speed);
        }
        else
        {
            // Network player, receive data
            anim.SetBool("walking", (bool)stream.ReceiveNext());
            leftRightBlend = (float)stream.ReceiveNext();
            forwardBackBlend = (float)stream.ReceiveNext();
            animationSpeed = (float)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            anim.SetFloat("Left-Right Blend", Mathf.Lerp(anim.GetFloat("Left-Right Blend"), leftRightBlend, 0.1f));
            anim.SetFloat("Forward-Back Blend", Mathf.Lerp(anim.GetFloat("Forward-Back Blend"), forwardBackBlend, 0.1f));
            anim.speed = Mathf.Lerp(anim.speed, animationSpeed, 0.1f);
        }
    }
}
