namespace Kira;

using Sandbox.Citizen;

public sealed class EnemyController : Component
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

    private TimeSince timeSinceStop;
    private bool isWaiting;

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

        if (CurState == EnemyStates.MOVING)
        {
            if (isWaiting)
            {
                Log.Info("is waiting");
                if (timeSinceStop > WaitTime)
                {
                    UpdateNextDestination();
                }
                else
                {
                    return;
                }
            }

            var tpos = agent.GetLookAhead(100f);
            var path = Scene.NavMesh.GetSimplePath(agent.AgentPosition, curWp.pos);

            foreach (Vector3 p in path)
            {
                Gizmo.Draw.Color = Color.Blue;
                Gizmo.Draw.LineSphere(p, 15f);
            }

            if (path.Count > 0)
            {
                var endPoint = path[^1];
                float reachDist = Vector3.DistanceBetween(endPoint, curWp.pos);
                canReachPos = reachDist < 5f;

                if (!canReachPos && !hasTarget)
                {
                    // find closest barricade
                    var obstacles = Scene.GetAllComponents<Obstacle>().ToList();
                    Obstacle closestObs = obstacles[0];
                    float closestDist = 9000f;

                    foreach (Obstacle obs in obstacles)
                    {
                        float dist = Vector3.DistanceBetween(obs.WorldPosition, endPoint);
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            closestObs = obs;
                        }
                    }

                    obstacleTarget = closestObs;
                    hasTarget = true;
                    CurState = EnemyStates.FIGHTING;
                }
            }

            Gizmo.Draw.Color = Color.Blue;
            Gizmo.Draw.LineSphere(tpos, 10f);


            HandleWaypoints();
        }
        else if (CurState == EnemyStates.FIGHTING)
        {
            if (!hasTarget || !obstacleTarget.IsValid())
            {
                CurState = EnemyStates.MOVING;
                return;
            }

            Gizmo.Draw.Color = Color.Yellow;
            Gizmo.Draw.LineCircle(obstacleTarget.WorldPosition, 10f);

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

    private void HandleWaypoints()
    {
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