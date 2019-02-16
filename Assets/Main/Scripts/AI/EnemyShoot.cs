using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [SerializeField]
    private float timeBetweenShots = 0.5f;
    [SerializeField]
    private float inacuracy = 30f;
    [SerializeField]
    private GameObject bulletPrefab;
    private WaitForSeconds awaitShooting;

    public event Action OnKill;

    private void Awake()
    {
        awaitShooting = new WaitForSeconds(timeBetweenShots);
    }

    private void OnEnable()
    {
        OnKill += StopAllCoroutines;
    }

    public IEnumerator Shoot()
    {
        float randomAngle = UnityEngine.Random.Range(-inacuracy / 2, inacuracy / 2);
        Quaternion shootAngle = (transform.rotation * Quaternion.AngleAxis(randomAngle, Vector3.up));
        Vector3 shootingDirection = shootAngle * Vector3.forward;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, shootingDirection, out hit, 10.0f) && hit.collider.CompareTag("Player"))
            hit.collider.gameObject.GetPhotonView().RPC("RPCHasBeenHit", RpcTarget.All, -1);
        Instantiate(bulletPrefab, transform.position, shootAngle).GetComponent<BulletController>().TagToIgnore = "Enemy";
        yield return awaitShooting;
        StartCoroutine(Shoot());
    }

    [PunRPC]
    public void PlayerDied()
    {
        OnKill();
    }

    private void OnDisable()
    {
        OnKill -= StopAllCoroutines;
    }
}
