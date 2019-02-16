using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Weapon : MonoBehaviour
{
    public GameObject bullet;
    private Transform muzzle;
    private AudioSource audioSource;
    public AudioClip[] firingSounds;

    private PhotonView photonView;
    private PlayerInterface playerInterface;

    public float unscopedInaccuracy = 30;
    public float scopedInaccuracy = 15;
    public float effectiveRange = 10;
    public float fireRate = 1.0f;
    public int ammo = 12;

    private float currentInaccuracy;
    private float timeSinceLastShot;
    private int currentAmmo;

    private bool isShooting;

    private void Awake()
    {
        muzzle = transform.Find("Muzzle");
        photonView = GetComponentInParent<PhotonView>();
        playerInterface = GetComponentInParent<PlayerInterface>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        playerInterface.Aim += Aim;
        playerInterface.LowerAim += LowerAim;
        playerInterface.StartShooting += StartShooting;
        playerInterface.Reload += Reload;
        playerInterface.StopShooting += StopShooting;
        playerInterface.StartShooting += StartShooting;
    }

    private void OnDisable()
    {
        playerInterface.Aim -= Aim;
        playerInterface.LowerAim -= LowerAim;
        playerInterface.Reload -= Reload;
        playerInterface.StartShooting -= StartShooting;
        playerInterface.StopShooting -= StopShooting;

        isShooting = false;
        timeSinceLastShot = 0.0f;
    }

    // Use this for initialization
    void Start()
    {
        currentInaccuracy = unscopedInaccuracy;
        timeSinceLastShot = 0.0f;
        currentAmmo = ammo;
        isShooting = false;
    }

    public void Aim()
    {
        currentInaccuracy = scopedInaccuracy;
    }

    public void LowerAim()
    {
        currentInaccuracy = unscopedInaccuracy;
    }

    public void StartShooting()
    {
        isShooting = true;
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    void Shoot()
    {
        Debug.Log("Time since last shot: " + timeSinceLastShot);
        Debug.Log("Current ammo: " + currentAmmo);
        if (timeSinceLastShot > fireRate && currentAmmo > 0)
        {
            Debug.Log("Weapon shoot called");
            float randomAngle = Random.Range(-currentInaccuracy / 2, currentInaccuracy / 2);
            Quaternion shootAngle = (muzzle.transform.rotation * Quaternion.AngleAxis(randomAngle, Vector3.up));
            Vector3 shootingDirection = shootAngle * Vector3.forward;
            if (photonView.IsMine)
            {
                RaycastHit hit;
                if (Physics.Raycast(muzzle.transform.position, shootingDirection, out hit, effectiveRange))
                {
                    Debug.Log("Raycast hit " + hit.collider.gameObject.name);
                    if (hit.collider.tag == "Player" || hit.collider.tag == "Enemy")
                    {
                        Debug.Log("Player hit");
                        hit.collider.gameObject.GetPhotonView().RPC("RPCHasBeenHit", RpcTarget.All, photonView.ViewID);
                    }
                }
                Debug.DrawRay(muzzle.transform.position, shootingDirection * effectiveRange, Color.green, 2.0f);
            }
            //default value to ignore
            Instantiate(bullet, muzzle.transform.position, shootAngle).GetComponent<BulletController>().TagToIgnore = "floor";
            
            PlayGunshot();
            timeSinceLastShot = 0.0f;
            --currentAmmo;
        }
    }

    public void Reload()
    {
        currentAmmo = ammo;
        timeSinceLastShot = 0.0f;
    }

    private void PlayGunshot()
    {
        audioSource.PlayOneShot(firingSounds[Random.Range(0, firingSounds.Length - 1)]);
    }

    private void Update()
    {
        if (isShooting)
        {
            Shoot();
        }
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(muzzle.transform.position, Quaternion.Euler(0, currentInaccuracy / 2, 0)*muzzle.transform.forward * 10);
        Debug.DrawRay(muzzle.transform.position, Quaternion.Euler(0, -currentInaccuracy / 2, 0) * muzzle.transform.forward * 10);
    }
}
