using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InGamePlayerDisplay : MonoBehaviourPunCallbacks
{
    private int ownerId;

    public Text PlayerNameText;
    public Text PlayerScore;

    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
        PlayerScore.text = "0";
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer.ActorNumber == ownerId)
        {
            object score;
            if (targetPlayer.CustomProperties.TryGetValue(Com.SHUPDP.JUST.JUSTConstantsAndDefinitions.scores, out score))
            {
            PlayerScore.text = score.ToString();
            }
        }
    }
}
