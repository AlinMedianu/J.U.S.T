using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{

    public GameObject[] availableWeaponPrefabs;

    private int currentWeapon = 0;

    // Use this for initialization
    void Start()
    {
        Instantiate(availableWeaponPrefabs[currentWeapon], transform, false);
    }

    // Update is called once per frame
    void Update()
    {

    }

}
