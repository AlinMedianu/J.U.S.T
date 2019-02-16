using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveButton : MonoBehaviourPunCallbacks {

    public void LeaveRoom()
    {
        Debug.Log("Leave Room Button CAlled");
        PhotonNetwork.LeaveRoom();
        Cursor.visible = true;
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
}
