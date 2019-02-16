using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyAnimations : MonoBehaviour
{
    private const float stoppingThreshold = 0.1f;
    private float initialSpeed;
    private NavMeshAgent aI;
    private Animator mover;

    public event Action OnMoving;
    public event Action OnResting;

    public bool IsStopped
    {
        set
        {
            mover.SetBool("walking", !value);
        }
    }

    public float MovementRelativeSpeed
    {
        set
        {
            mover.speed = value / initialSpeed;
        }
    }

    public float InitialSpeed
    {
        get
        {
            return initialSpeed;
        }
    }

    private void Awake()
    {
        aI = GetComponent<NavMeshAgent>();
        mover = GetComponent<Animator>();
        mover.SetFloat("Forward-Back Blend", 1);
        MovementRelativeSpeed = initialSpeed = GetComponent<NavMeshAgent>().speed;
        OnMoving += () => { IsStopped = false; };
        OnResting += () => { IsStopped = true; };
    }

    private void Update()
    {
        CheckIfStopped();
    }

    private void CheckIfStopped()
    {
        if (aI.remainingDistance > aI.stoppingDistance + stoppingThreshold ||
            aI.remainingDistance < aI.stoppingDistance)
            OnMoving();
        else
            OnResting();
    }

    private void OnDisable()
    {
        OnMoving -= () => { IsStopped = false; };
        OnResting -= () => { IsStopped = true; };
    }
}
