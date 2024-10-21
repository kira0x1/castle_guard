namespace Kira;

using System;
using Sandbox.Citizen;

public partial class EnemyController : Component
{
    [Property, Range(0, 10)] private float WaitTime { get; set; } = 0f;
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
    private Vector3 NextPathPointPos { get; set; }

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
        Log.Info(CurState);
        UpdateAnimator();

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
            if (!hasTarget || !obstacleTarget.IsValid())
            {
                CurState = EnemyStates.MOVING;
                return;
            }

            Gizmo.Draw.Color = Color.Yellow;
            Gizmo.Draw.LineSphere(obstacleTarget.WorldPosition, 10f);

            agent.MoveTo(obstacleTarget.WorldPosition);
            float dist = Vector3.DistanceBetween(agent.WorldPosition, obstacleTarget.WorldPosition);

            if (dist < 50)
            {
                if (timeSinceLastAttack > 0.3f)
                {
                    anim.TriggerJump();
                    obstacleTarget.prop.OnDamage(new DamageInfo(20f, GameObject, GameObject));
                    timeSinceLastAttack = 0f;
                }
            }
        }
    }

    private void HandleDeadState()
    {
    }

    private void HandleFightingState()
    {
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
}