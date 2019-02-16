using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

[RequireComponent(typeof(Stats))]
public class PlayerUIDisplay : MonoBehaviour
{
    PlayerInterface playerInterface;

    private Image healthbar;

    private Image HBLeft;
    private Image HBRight;


    private GameObject xHair;
    private void Awake()
    {
        playerInterface = GetComponent<PlayerInterface>();
        xHair = UnityEngine.GameObject.Find("Crosshair");

        HBLeft = transform.Find("Health Bar").transform.Find("HBLeft").GetComponent<Image>();
        HBRight = transform.Find("Health Bar").transform.Find("HBRight").GetComponent<Image>();
    }

    private void OnEnable()
    {
        playerInterface.Aim += DecreaseCursorSize;
        playerInterface.LowerAim += IncreaseCursorSize;
    }

    private void OnDisable()
    {
        playerInterface.Aim -= DecreaseCursorSize;
        playerInterface.LowerAim -= IncreaseCursorSize;

        if (PhotonNetwork.InRoom)
        {
            IncreaseCursorSize();
        }
    }

    public void UpdateHealthBar(float currentHealth)
    {
        HBLeft.fillAmount = currentHealth / 100;
        HBRight.fillAmount = currentHealth / 100;
    }

    public void DecreaseCursorSize()
    {
        xHair.GetComponent<RectTransform>().sizeDelta = new Vector2(12, 12);
        Camera.main.GetComponent<CameraController>().FocusCamera();
    }

    public void IncreaseCursorSize()
    {
        xHair.GetComponent<RectTransform>().sizeDelta = new Vector2(24, 24);
        Camera.main.GetComponent<CameraController>().UnfocusCamera();
    }

    private void LateUpdate()
    {
        xHair.transform.position = Input.mousePosition;

    }
}