using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public class NetworkMovement : MonoBehaviourPunCallbacks, IPunObservable {

    private Vector3 networkPosition = Vector3.zero;
    private Quaternion networkRotation = Quaternion.identity;

    private void Start()
    {
        gameObject.name = GetComponent<Stats>().Name;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            // Network player, receive data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, 0.1f);
        }
    }
}
