using System.Linq;
using UnityEngine;
using System.Collections;
using System;

public class EnemyTeamate : EnemyMove
{
    private bool playerLost = true;
    private int nextLocationID = -1;
    private float timer;
    private const float timerResetter = 1f;
    private const float stoppingThreshold = 1f;
    [SerializeField]
    private float shootingRange = 4f;
    [SerializeField]
    private float recruitRange = 5f;
    [SerializeField]
    private float chasingSpeed = 3f;
    private Vector3 playerWitnessPosition = Vector3.down;
    private Location[] locations;
    private EnemyShoot shootingScript;
    private EnemyTeamate[] teammates;
    private Coroutine shoot = null;

    public int TeamID { private get; set; }

    public Vector3 PlayerWitnessPosition
    {
        set
        {
            playerWitnessPosition = value;
            Destination = value;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        shootingScript = GetComponentInChildren<EnemyShoot>();
        Behave += Search;
        Behave += Chase;
        Behave += Shoot;
        TeamID = -1;
        detectionScript.OnPlayerDetected += () => { playerLost = false; };
        detectionScript.OnPlayerLost += () => { playerLost = true; };
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateTeammates();
        shootingScript.OnKill += PlayerDied;
        locations = (from location in GameObject.Find("Locations").transform.GetChildrenPositions()
                     select new Location(location)).ToArray();
        if (detectionScript.Target && !playerLost)
            PlayerDetected();
    }

    private void UpdateTeammates()
    {
        teammates = (from ally in FindObjectsOfType<EnemyTeamate>()                
                     where ally.TeamID == TeamID
                     select ally).ToArray();
    }

    protected override IEnumerator React(WaitForSeconds awaitReaction, Transform player)
    {
        foreach (EnemyTeamate teammate in teammates)
            if(teammate && teammate.aI)
            {
                teammate.animationScript.IsStopped = teammate.aI.isStopped = true;
                yield return awaitReaction;
                teammate.animationScript.IsStopped = teammate.aI.isStopped = false;
            }
        if (teammates.Any(teammate => teammate.detectionScript.Target))
            foreach(EnemyTeamate teammate in teammates)
                if (teammate)
                {
                    teammate.currentState = EnemyState.Primary;
                    teammate.detectionScript.Target = player;
                }
        react = null;
    }

    private Vector3 NextLocation()
    {
        Location nextLocation = ClosestUnvisitedLocation();
        if (nextLocationID == -1)
        {
            ResetLocations();
            nextLocation = ClosestUnvisitedLocation();
        }
        return nextLocation.Position;
    }

    private Location ClosestUnvisitedLocation()
    {
        Location nextLocation = (from location in locations
                                 where !location.Visited
                                 orderby Vector3.Distance(location.Position, transform.position)
                                 select location).DefaultIfEmpty(new Location(Vector3.down)).FirstOrDefault();
        nextLocationID = Array.IndexOf(locations, nextLocation);
        return nextLocation;
    }

    private void ResetLocations()
    {
        int noLocations = locations.Length;
        for (int i = 0; i < noLocations; ++i)
            locations[i].Visited = false;
    }

    private void Search()
    {
        animationScript.MovementRelativeSpeed = aI.speed = animationScript.InitialSpeed;
        if (aI.remainingDistance < aI.stoppingDistance + stoppingThreshold && 
            aI.remainingDistance >= aI.stoppingDistance && timer >= timerResetter)
        {
            timer = 0;
            if (Destination != playerWitnessPosition)
            {
                Vector3 nextLocation = NextLocation();
                foreach (EnemyTeamate teamate in teammates)
                    if(teamate)
                    {
                        teamate.Destination = nextLocation;
                        teamate.locations[nextLocationID].Visited = true;
                    }
                RecruitAllies();
            }
            else
                playerWitnessPosition = Vector3.down;
        }
        if (timer < timerResetter)
            timer += Time.deltaTime;
    }

    private void Chase()
    {
        animationScript.MovementRelativeSpeed = aI.speed = chasingSpeed;
        Destination = detectionScript.Target.position;
        RecruitAllies();
        if (Vector3.Distance(transform.position, detectionScript.Target.position) < shootingRange)
            currentState = EnemyState.Secondary;
        else if (playerLost)
            currentState = EnemyState.Initial;
    }

    private void Shoot()
    {
        Destination = transform.position;
        transform.LookAt(detectionScript.Target);
        if(shoot == null)
            shoot = StartCoroutine(shootingScript.Shoot());
        if (Vector3.Distance(transform.position, detectionScript.Target.position) > shootingRange)
        {
            currentState = EnemyState.Primary;
            shootingScript.StopAllCoroutines();
            shoot = null;
        }
    }

    private void RecruitAllies()
    {
        var nearbyAllies = from ally in FindObjectsOfType<EnemyRogue>()
                           where ally.enabled && 
                           Vector3.Distance(transform.position, ally.transform.position) < recruitRange
                           select ally;
        foreach (EnemyRogue nearbyAlly in nearbyAllies)
            nearbyAlly.GetRecruited(TeamID, Destination);
        if (nearbyAllies.ToArray().Length > 0)
            SyncroniseTeam();

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
        detectionScript.OnPlayerDetected -= () => { playerLost = false; };
        detectionScript.OnPlayerLost -= () => { playerLost = true; };
        shootingScript.OnKill -= PlayerDied;
    }

    private void SyncroniseTeam()
    {
        foreach (EnemyTeamate teammate in teammates)
            if (teammate && teammate != this)
                teammate.UpdateTeammates();
    }

    private void OnDestroy()
    {
        if (teammates != null)
            SyncroniseTeam();
    }
}

public struct Location
{
    public bool Visited { get; set; }

    public Vector3 Position { get; private set; }

    public Location(Vector3 position)
    {
        Visited = false;
        Position = position;
    }
}
