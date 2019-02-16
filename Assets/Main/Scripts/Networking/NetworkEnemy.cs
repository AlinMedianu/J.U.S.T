using Photon.Pun;
using UnityEngine;

public class NetworkEnemy : MonoBehaviourPunCallbacks, IPunObservable
{
    private float walkingAnimationSpeed;
    private Vector3 networkPosition = Vector3.zero;
    private Quaternion networkRotation = Quaternion.identity;
    //private Animator enemyMover;
    //private NavMeshAgent aI; 

    //private void Awake()
    //{
    //    enemyMover = GetComponent<Animator>();
    //    aI = GetComponent<NavMeshAgent>();
    //}


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(aI.speed);
        }
        else
        {
            // Network player, receive data
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            //walkingAnimationSpeed = (float)stream.ReceiveNext();
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, networkRotation, 0.1f);
            //enemyMover = 
        }
    }
}
