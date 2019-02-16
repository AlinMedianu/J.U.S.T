using System.Collections;
using UnityEngine;
using System.Linq;

public class EnemyScout : EnemyRogue
{
    private int waypointNumber = 0;
    private float timer;                                            //timer to prevent the AI from changing its destination twice in a row
    private const float timerResetter = 0.1f;                               //timer's reset value
    private const float stoppingThreshold = 0.1f; 
    private Vector3 playerWitnessPosition = Vector3.down;
    [SerializeField]
    private float patrolingSpeed = 2f;
    [SerializeField]
    private float fleeingSpeed = 5f;
    [SerializeField]
    private Waypoints waypoints;
    private EnemyRogue closestGroupedAlly = null;


    protected override void Awake()
    {
        base.Awake();
        Behave += Patrol;
        Behave += Flee;
        Behave += Surrender;
        timer = timerResetter;
        animationScript.IsStopped = false;
        animationScript.OnResting += ChooseNextDestination;
    }

    public void InitialiseWaypoints()                            //called by the "Reset waypoints" button 
    {
        transform.root.Find("Waypoints").SetChildrenPositions(waypoints.Positions);
        Destination = NextDestination();
    }

    protected override IEnumerator React(WaitForSeconds awaitReaction, Transform player)
    {
        animationScript.IsStopped = aI.isStopped = true;
        yield return awaitReaction;
        animationScript.IsStopped = aI.isStopped = false;
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            currentState = EnemyState.Primary;
            animationScript.OnResting -= ChooseNextDestination;
        }
        else
            currentState = EnemyState.Secondary;
        react = null;
    }

    private Vector3 NextDestination()
    {
        if (waypointNumber == waypoints.Positions.Length)
            waypointNumber = 0;
        return waypoints.Positions[waypointNumber++];
    }

    private void ChooseNextDestination()
    {
        if (timer >= timerResetter)
        {
            timer = 0;
            if (aI.velocity == Vector3.zero)    
                --waypointNumber;
            Destination = NextDestination();
        }
    }

    private void Patrol()
    {
        animationScript.MovementRelativeSpeed = aI.speed = patrolingSpeed;
        if (timer < timerResetter)
            timer += Time.deltaTime;
    }

    private void Flee()
    {
        animationScript.MovementRelativeSpeed = aI.speed = fleeingSpeed;
        if (!closestGroupedAlly)
            Destination = FleePosition();
        if (Destination != Vector3.down)
        {
            if (aI.remainingDistance < aI.stoppingDistance + stoppingThreshold && aI.remainingDistance > aI.stoppingDistance)
            {
                CreateTeam(playerWitnessPosition);
                closestGroupedAlly = null;
            }
        }
        else
            currentState = EnemyState.Secondary;
    }

    private Vector3 FleePosition()
    {
        playerWitnessPosition = detectionScript.Target.position;
        closestGroupedAlly = (from ally in FindObjectsOfType<EnemyRogue>()
                              where ally != this
                              orderby Vector3.Distance(ally.transform.position, transform.position)
                              select ally).DefaultIfEmpty(null).FirstOrDefault();
        GroupID = closestGroupedAlly.GroupID;
        return closestGroupedAlly ? closestGroupedAlly.transform.position : Vector3.down;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        animationScript.OnResting -= ChooseNextDestination;
    }
}