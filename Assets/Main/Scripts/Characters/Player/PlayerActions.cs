using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PlayerInterface))]
public class PlayerActions : MonoBehaviour
{
    PlayerInterface playerInterface;
    PhotonView photonView;
    Com.SHUPDP.JUST.GameManager gameManager;

    private Light torch;
    private bool flashlight;


    public GameObject muzzle;
    public GameObject bullet;

    private void Awake()
    {
        playerInterface = GetComponent<PlayerInterface>();
        photonView = GetComponent<PhotonView>();
        gameManager = GameObject.Find("Game Manager").GetComponent<Com.SHUPDP.JUST.GameManager>();
        torch = GetComponentInChildren<Light>();
    }

    private void OnEnable()
    {
        playerInterface.ToggleFlashlight += ToggleFlashlight;
        playerInterface.Interact += AttemptOpenDoor;
        playerInterface.Die += Die;
        playerInterface.GetKill += ShowPlayerKilledPopup;
        playerInterface.Respawn += ConfirmEnemyKill;
    }

    private void OnDisable()
    {
        playerInterface.ToggleFlashlight -= ToggleFlashlight;
        playerInterface.Interact -= AttemptOpenDoor;
        playerInterface.Die -= Die;
        playerInterface.GetKill -= ShowPlayerKilledPopup;

        playerInterface.Respawn -= ConfirmEnemyKill;
    }

    private void Start()
    {
        flashlight = true;
    }

    public void ToggleFlashlight()
    {
        flashlight = (flashlight) ? false : true;
        torch.enabled = flashlight;
    }

    //public void Shoot()
    //{
    //    float randomAngle = Random.Range(-currentInaccuracy / 2, currentInaccuracy / 2);
    //    Quaternion shootAngle = (transform.rotation * Quaternion.AngleAxis(randomAngle, Vector3.up));
    //    Vector3 shootingDirection = shootAngle * Vector3.forward;

    //    if (photonView.IsMine)
    //    {
    //        RaycastHit hit;
    //        if (Physics.Raycast(muzzle.transform.position, shootingDirection, out hit, 10.0f))
    //        {
    //            Debug.Log("Raycast hit " + hit.collider.gameObject.name);
    //            if (hit.collider.tag == "Player" || hit.collider.tag == "Enemy")
    //            {
    //                Debug.Log("Player hit");
    //                hit.collider.gameObject.GetPhotonView().RPC("RPCHasBeenHit", RpcTarget.All, photonView.ViewID);

    //            }
    //        }
    //        Debug.DrawRay(muzzle.transform.position, shootingDirection * 10, Color.green, 2.0f);
    //    }
    //    Instantiate(bullet, muzzle.transform.position, shootAngle).GetComponent<BulletController>().TagToIgnore = tag;
    //}

    public void AttemptOpenDoor()
    {
        Debug.Log("open door");
        RaycastHit hitInfo;
        Vector3 handPosition = transform.position + new Vector3(0, 1, 0);
        Debug.DrawRay(handPosition, transform.forward * 3.5f, Color.blue, 5);
        if (Physics.Raycast(handPosition, transform.forward, out hitInfo, 3.5f))
        {
            Debug.Log("open door2");
            Debug.Log(hitInfo.collider.transform.parent.gameObject.tag + " + " + hitInfo.collider.name);
            if (hitInfo.collider.transform.parent.gameObject.tag == "Door")
            {
                //DOOR OPEN SUCCESSFUL
                hitInfo.collider.transform.parent.gameObject.GetComponent<PhotonView>().RPC("OpenDoor", RpcTarget.All);
            }
        }
    }

    //Debugging purposes only in this class please!
    private void Update()
    {

    }

    public void Die()
    {
        Debug.Log("Call to gamemanager to respawn");
        gameManager.Respawn(gameObject);
    }

    public void ShowPlayerKilledPopup()
    {
        if (photonView.IsMine)
        {
            gameManager.ShowKillPopup();
        }
    }
    
    private void ConfirmEnemyKill()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            enemy.GetComponentsInChildren<PhotonView>()[1].RPC("PlayerDied", RpcTarget.All);    //the PhotonView paired with the shooting script
    }
}
