using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

using ExitGames.Client.Photon;

public class Stats : MonoBehaviour
{
    PhotonView photonView;
    PlayerInterface playerInterface;
    PlayerUIDisplay playerUI;
    public int health;
    public int score;
    public string Name;

    public int scorePerHit = 10;
    public int scorePerKill = 100;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        playerInterface = GetComponent<PlayerInterface>();
        playerUI = GetComponent<PlayerUIDisplay>();
    }

    private void OnEnable()
    {
        playerInterface.HasBeenHit += TakeDamage;
        playerInterface.GetPoints += AddHitScore;
        playerInterface.GetKill += AddKillScore;
        playerInterface.Respawn += ResetHealth;
    }

    private void OnDisable()
    {
        playerInterface.HasBeenHit -= TakeDamage;
        playerInterface.GetPoints -= AddHitScore;
        playerInterface.GetKill -= AddKillScore;
        playerInterface.Respawn -= ResetHealth;

    }

    private void Start()
    {
        health = 100;
        score = 0;
        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable networkedPlayerScores = new ExitGames.Client.Photon.Hashtable
            {
                {Com.SHUPDP.JUST.JUSTConstantsAndDefinitions.scores, score}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(networkedPlayerScores);
        }
    }

    public void TakeDamage(int shooterId)
    {
        health -= 23;
        playerUI.UpdateHealthBar(health);
        if (photonView.IsMine)
        {
            if (shooterId != -1)
            {
                PhotonView shooter = PhotonView.Find(shooterId);
                if (shooter.gameObject.tag == "Player")
                {
                    shooter.RPC("RPCGetPoints", RpcTarget.All);
                }
                if (IsDead())
                {
                    if (shooter.gameObject.tag == "Player")
                    {
                        shooter.RPC("RPCGetKill", RpcTarget.All);
                    }
                    photonView.RPC("RPCDie", RpcTarget.All);
                }
            }
            else if (IsDead())
            {
                photonView.RPC("RPCDie", RpcTarget.All);
            }
        }
    }


    public void AddHitScore()
    {
        score += scorePerHit;
        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable networkedPlayerScores = new ExitGames.Client.Photon.Hashtable
            {
                {Com.SHUPDP.JUST.JUSTConstantsAndDefinitions.scores, score}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(networkedPlayerScores);
        }
    }

    public void AddKillScore()
    {
        Debug.Log("KILL SCORE ADDED");
        score += scorePerKill;
        if (photonView.IsMine)
        {
            ExitGames.Client.Photon.Hashtable networkedPlayerScores = new ExitGames.Client.Photon.Hashtable
            {
                {Com.SHUPDP.JUST.JUSTConstantsAndDefinitions.scores, score}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(networkedPlayerScores);
        }
    }

    private bool IsDead()
    {
        return !(health > 0);
    }

    public void ResetHealth()
    {
        health = 100;
        playerUI.UpdateHealthBar(health);
    }
}
