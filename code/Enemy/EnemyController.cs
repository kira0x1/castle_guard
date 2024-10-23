namespace Kira;

using System;
using Sandbox.Citizen;

public partial class EnemyController : Component
{
    [Property] private bool StandStill { get; set; }
    [Property, Range(0, 10)] private float WaitTime { get; set; } = 0f;
    [Property, Range(0, 2f)] private float PunchCooldown { get; set; } = 0.8f;
    [Property, Range(10, 200)] private float minDistance { get; set; } = 50f;

    private NavMeshAgent agent;
    private bool canReachPos;
    private CitizenAnimationHelper anim;
    private WaypointManager wpManager;
    private Waypoint curWp;

    private TimeSince timeSinceLastAttack;

    private enum EnemyStates
    {
        MOVING,
        FIGHTING,
        DEAD
    }

    private EnemyStates CurState;
    private bool hasTarget;
    private Obstacle obstacleTarget;

    protected override void OnStart()
    {
        agent = Components.Get<NavMeshAgent>();
        anim = Components.Get<CitizenAnimationHelper>();

        wpManager = Scene.Components.GetAll<WaypointManager>().FirstOrDefault();

        if (!wpManager.IsValid()) return;
        curWp = wpManager.GetWaypoint(0);
    }

    protected override void OnUpdate()
    {
        UpdateAnimator();
        if (StandStill) return;

        // Log.Info(CurState);

        switch (CurState)
        {
            case EnemyStates.MOVING:
                HandleMovingState();
                break;
            case EnemyStates.FIGHTING:
                HandleFightingState();
                break;
            case EnemyStates.DEAD:
                HandleDeadState();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (CurState == EnemyStates.MOVING)
        {
            HandleMovingState();
        }
        else if (CurState == EnemyStates.FIGHTING)
        {
            HandleFightingState();
        }
    }

    private void HandleDeadState()
    {
    }

    private void LeaveFightingState()
    {
        hasTarget = false;
        CurState = EnemyStates.MOVING;
    }

    private void HandleFightingState()
    {
        if (!hasTarget || !obstacleTarget.IsValid())
        {
            LeaveFightingState();
            return;
        }

        Gizmo.Draw.Color = Color.Yellow;
        Gizmo.Draw.LineSphere(obstacleTarget.WorldPosition, 10f);

        agent.MoveTo(obstacleTarget.WorldPosition);
        float dist = Vector3.DistanceBetween(agent.WorldPosition, obstacleTarget.WorldPosition);

        if (dist < 150)
        {
            anim.HoldType = CitizenAnimationHelper.HoldTypes.Punch;
        }

        if (dist < 50)
        {
            if (timeSinceLastAttack > PunchCooldown)
            {
                anim.Target.Set("holdtype_attack", Random.Shared.Float(0, 2f));
                anim.Target.Set("b_attack", true);

                obstacleTarget.prop.OnDamage(new DamageInfo(20f, GameObject, GameObject));
                timeSinceLastAttack = 0f;
            }
        }
    }

    private void HandleWaypoints()
    {
        float dist = Vector3.DistanceBetween(agent.AgentPosition, curWp.pos);

        if (dist < minDistance)
        {
            UpdateNextDestination();
        }

        agent.MoveTo(curWp.pos);
    }

    private void UpdateNextDestination()
    {
        curWp = curWp.NextWaypoint;
        agent.UpdateRotation = true;
        agent.UpdatePosition = true;
    }

    private void UpdateAnimator()
    {
        anim.WithVelocity(agent.Velocity);
        anim.WithWishVelocity(agent.WishVelocity);
    }

    public void OnHit(DamageInfo damageInfo, Vector3 force = default)
    {
        anim.ProceduralHitReaction(damageInfo, 10f, force);
    }

    public void OnNavGenerated()
    {
        // If were not fighting a player or key structure, and just trying to get to break an obstacle then stop fighting and go to move state
        if (CurState == EnemyStates.FIGHTING)
        {
            LeaveFightingState();
        }
    }
}