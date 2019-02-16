using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    void Update()
    {
        if (Input.anyKeyDown)
        {
            GameObject.Find("Canvas").GetComponent<Com.SHUPDP.JUST.MainMenuLobby>().OnKeyPress();
        }
    }
}
