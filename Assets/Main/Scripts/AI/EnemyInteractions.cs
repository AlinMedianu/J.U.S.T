using Photon.Pun;
using UnityEngine;

public class EnemyInteractions : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent && other.transform.parent.parent && 
            other.transform.parent.parent.name == "Door" && 
            !other.transform.parent.GetComponent<DoorController>().Open)
            other.transform.parent.GetComponent<PhotonView>().RPC("OpenDoor", RpcTarget.All);
    }
}
