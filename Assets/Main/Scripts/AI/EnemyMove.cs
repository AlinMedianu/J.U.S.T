using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyAnimations))]
[RequireComponent(typeof(EnemyDetection))]
[RequireComponent(typeof(EnemyInteractions))]
[RequireComponent(typeof(NetworkEnemy))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyMove : MonoBehaviour
{
    [SerializeField]
    protected float reactionTime = 3f;
    protected EnemyState currentState;
    protected EnemyAnimations animationScript;
    protected EnemyDetection detectionScript;   
    protected NavMeshAgent aI;
    protected Action Behave;
    protected WaitForSeconds awaitReaction;
    protected Coroutine react = null;

    public Vector3 Destination
    {
        get
        {
            return transform.root.Find("Destination").position;
        }
        set
        {
            if (transform.root.Find("Destination").position != value)
            {
                transform.root.Find("Destination").position = value;
                aI.SetDestination(transform.root.Find("Destination").position);
                aI.stoppingDistance = currentState != EnemyState.Primary ? 0 : 1;
            }
        }
    }

    protected virtual void Awake()
    {
        animationScript = GetComponent<EnemyAnimations>();
        detectionScript = GetComponent<EnemyDetection>();       
        aI = GetComponent<NavMeshAgent>();
        currentState = EnemyState.Initial;
        awaitReaction = new WaitForSeconds(reactionTime);
    }

    protected virtual void OnEnable()
    {
        detectionScript.OnPlayerDetected += PlayerDetected;
    }

    protected void PlayerDetected()
    {
        if (react == null && currentState == EnemyState.Initial) 
            react = StartCoroutine(React(awaitReaction, detectionScript.Target));
    }

    private void Update()
    {
        Behave.GetInvocationList()[(int)currentState].DynamicInvoke();
    }

    protected abstract IEnumerator React(WaitForSeconds awaitReaction, Transform player);

    protected virtual void OnDisable()
    {
        detectionScript.OnPlayerDetected -= PlayerDetected;
    }
}
public enum EnemyState { Initial, Primary, Secondary } 