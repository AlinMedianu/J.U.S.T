using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class EndGame : MonoBehaviour {

    public GameObject playerListUIPrefab;
    // Use this for initialization
    void Start()
    {
        Cursor.visible = true;

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object playerscore;
            p.CustomProperties.TryGetValue(Com.SHUPDP.JUST.JUSTConstantsAndDefinitions.scores, out playerscore);
            Debug.Log(p.NickName);
            Debug.Log(playerscore);
            GameObject playerItem = Instantiate(playerListUIPrefab, this.transform);
            playerItem.transform.GetChild(0).GetComponent<Text>().text = p.NickName;
            playerItem.transform.GetChild(1).GetComponent<Text>().text = "$" + ((int)playerscore).ToString();

        }
    }
}
