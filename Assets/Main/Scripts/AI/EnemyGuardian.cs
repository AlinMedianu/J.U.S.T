using System.Collections;
using UnityEngine;

public class EnemyGuardian : EnemyRogue
{
    [SerializeField]
    private float shootingRange = 4f;
    private EnemyShoot shootingScript;
    private Coroutine shoot = null;

    protected override void Awake()
    {
        base.Awake();
        shootingScript = GetComponentInChildren<EnemyShoot>();
        Behave += Idle;
        Behave += Shoot;
        Behave += Surrender;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        shootingScript.OnKill += PlayerDied;
    }

    protected override IEnumerator React(WaitForSeconds awaitReaction, Transform player)
    {
        animationScript.IsStopped = aI.isStopped = true;
        yield return awaitReaction;
        animationScript.IsStopped = aI.isStopped = false;
        if (UnityEngine.Random.Range(0, 2) == 0)
            currentState = EnemyState.Primary;
        else
            currentState = EnemyState.Secondary;
        react = null;
    }

    private void Idle()
    {
        
    }

    private void Shoot()
    {
        Destination = transform.position;
        transform.LookAt(detectionScript.Target);
        if (shoot == null)
            shoot = StartCoroutine(shootingScript.Shoot());
        if (Vector3.Distance(transform.position, detectionScript.Target.position) > shootingRange)
            CreateTeam(detectionScript.Target.position);
    }

    private void PlayerDied()
    {
        detectionScript.Target = null;
        if (Destination != transform.position)
            Destination = transform.position;
        currentState = EnemyState.Initial;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        shootingScript.OnKill -= PlayerDied;
    }
}