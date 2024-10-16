using Sandbox.Citizen;

public sealed class EnemyController : Component
{
    private NavMeshAgent agent;
    private CitizenAnimationHelper anim;

    private WaypointManager wpManager;
    private Waypoint curWp;

    private float waitTime = 0f;
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

        if (isWaiting && timeSinceStop < waitTime)
        {
            return;
        }

        if (isWaiting && timeSinceStop > waitTime)
        {
            UpdateNextDestination();
        }

        float dist = Vector3.DistanceBetween(agent.AgentPosition, curWp.pos);

        if (dist < 70f)
        {
            timeSinceStop = 0;
            if (waitTime > 0)
            {
                agent.Stop();
                agent.MoveTo(agent.AgentPosition);

                agent.Velocity = Vector3.Zero;
                agent.UpdateRotation = false;
                isWaiting = true;
                return;
            }

            UpdateNextDestination();
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
}