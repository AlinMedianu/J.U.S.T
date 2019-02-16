using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInterface))]
[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    PlayerInterface playerInterface;
    Animator anim;

    private void Awake()
    {
        playerInterface = GetComponent<PlayerInterface>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        playerInterface.Aim += EnterAimMode;
        playerInterface.LowerAim += ExitAimMode;
    }
    private void OnDisable()
    {
        playerInterface.Aim -= EnterAimMode;
        playerInterface.LowerAim -= ExitAimMode;
    }

    public void EnterAimMode()
    {
        anim.speed = 0.5f;
    }

    public void ExitAimMode()
    {
        anim.speed = 1.0f;
    }
}
