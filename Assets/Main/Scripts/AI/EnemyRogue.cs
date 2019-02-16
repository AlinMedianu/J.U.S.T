using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(EnemyTeamate))]
public abstract class EnemyRogue : EnemyMove
{
    private bool inGroup = false;
    protected static int nextGroupID = -1;
    [SerializeField][Range(0, 100)]
    protected int chanceToDisobey = 10;
    [SerializeField]
    protected float timeBetweenDisobeyAttempts = 5f;
    [SerializeField]
    private float groupRange = 1.5f;
    [SerializeField]
    private float surrenderingSpeed = 1.5f;
    private EnemyTeamate teamBehaviour;
    private WaitForSeconds awaitDisobeying;
    protected Coroutine disobey = null;

    public bool InGroup
    {
        get
        {
            if(inGroup && GroupID == -1)
            {

                ++nextGroupID;
                GroupID = nextGroupID;
                foreach (EnemyRogue groupMate in FindGroupedAllies())
                {
                    groupMate.GroupID = nextGroupID;
                    groupMate.inGroup = inGroup;
                }
            }
            return inGroup;
        }
    }

    public int GroupID { get; set; }

    public EnemyState CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private void Start()
    {
        GroupID = nextGroupID;
        inGroup = FindGroupedAllies().ToArray().Length > 1;
        teamBehaviour = GetComponent<EnemyTeamate>();
        teamBehaviour.enabled = false;
        awaitDisobeying = new WaitForSeconds(timeBetweenDisobeyAttempts);
    }

    private IEnumerable<EnemyRogue> FindGroupedAllies()
    {
        return from ally in FindObjectsOfType<EnemyRogue>()
               where ally != this && Vector3.Distance(transform.position, ally.transform.position) < groupRange
               select ally;
    }

    protected void Surrender()
    {
        if (disobey == null)
            disobey = StartCoroutine(Disobey(awaitDisobeying));
        animationScript.MovementRelativeSpeed = aI.speed = surrenderingSpeed;
        Destination = detectionScript.Target.position + detectionScript.Target.forward * 3;
    }

    protected IEnumerator Disobey(WaitForSeconds awaitDisobeying)
    {
        yield return awaitDisobeying;
        if (UnityEngine.Random.Range(0, 100) < chanceToDisobey)
            currentState = EnemyState.Primary;
        disobey = null;
    }

    protected void CreateTeam(Vector3 playerWitnessPosition)
    {
        var groupedAllies = from ally in FindObjectsOfType<EnemyRogue>()
                            where ally.InGroup && ally.GroupID == GroupID
                            select ally;
        foreach (EnemyRogue groupedAlly in groupedAllies)
        {
            groupedAlly.teamBehaviour.TeamID = GroupID;
            groupedAlly.teamBehaviour.enabled = true;
            groupedAlly.teamBehaviour.PlayerWitnessPosition = playerWitnessPosition;
            groupedAlly.enabled = false;
        }
    }

    public void GetRecruited(int teamID, Vector3 playerWitnessPosition)
    {
        teamBehaviour.TeamID = teamID;
        teamBehaviour.enabled = true;
        teamBehaviour.PlayerWitnessPosition = playerWitnessPosition;
        enabled = false;
    }

}
