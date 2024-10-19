namespace Kira;

using Sandbox.Citizen;

public sealed class EnemyController : Component
{
    [Property, Range(0, 10)] private float WaitTime { get; set; } = 0f;
    [Property, Range(10, 200)] private float minDistance { get; set; } = 50f;

    private NavMeshAgent agent;
    private CitizenAnimationHelper anim;
    private WaypointManager wpManager;
    private Waypoint curWp;

    private TimeSince timeSinceStop;
    private bool isWaiting;

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

        if (isWaiting && timeSinceStop < WaitTime)
        {
            return;
        }

        if (isWaiting && timeSinceStop > WaitTime)
        {
            UpdateNextDestination();
        }

        float dist = Vector3.DistanceBetween(agent.AgentPosition, curWp.pos);

        if (dist < minDistance)
        {
            timeSinceStop = 0;
            if (WaitTime > 0)
            {
                agent.Stop();
                agent.MoveTo(agent.AgentPosition);

                agent.Velocity = Vector3.Zero;
                agent.UpdateRotation = false;
                isWaiting = true;
            }
            else
            {
                UpdateNextDestination();
            }
        }

        agent.MoveTo(curWp.pos);
    }

    private void UpdateNextDestination()
    {
        curWp = curWp.NextWaypoint;
        agent.UpdateRotation = true;
        agent.UpdatePosition = true;
        isWaiting = false;
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