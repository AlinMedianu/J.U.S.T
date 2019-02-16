using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AudioListenerFollow : MonoBehaviour
{

    private Transform target;

    private void Start()
    {
        target = GameObject.Find(PhotonNetwork.NickName).transform;
    }

    void Update()
    {
        if (target.gameObject.activeSelf)
        {
            transform.position = new Vector3(target.transform.position.x, 1.5f, target.transform.position.z);
        }

    }
}
