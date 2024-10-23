namespace Kira;

using Sandbox.Citizen;

public partial class EnemyController
{
    private void HandleMovingState()
    {
        // Can we get to our goal?
        // yes -> move to goal
        // no -> find closest obstacle, and destroy

        // The position of our next position
        Vector3 tpos = agent.GetLookAhead(100f);

        // Get a simplified path to where we want to go
        List<Vector3> path = Scene.NavMesh.GetSimplePath(agent.AgentPosition, curWp.pos);

        // gizmo
        foreach (Vector3 p in path)
        {
            Gizmo.Draw.Color = Color.Blue;
            Gizmo.Draw.LineSphere(p, 15f);
        }

        anim.HoldType = CitizenAnimationHelper.HoldTypes.None;
        canReachPos = CanReachPath(path);

        if (!canReachPos && !hasTarget && path.Count > 0)
        {
            // find closest barricade
            obstacleTarget = FindClosestObstacle(path[^1]);
            hasTarget = true;
            CurState = EnemyStates.FIGHTING;
            return;
        }

        Gizmo.Draw.Color = Color.Blue;
        Gizmo.Draw.LineSphere(tpos, 10f);

        HandleWaypoints();
    }

    private void DetectPlayer()
    {
        var startPos = DetectTransform.WorldPosition;
        var straightEnd = startPos + WorldTransform.Forward * DetectionDistance;
        var leftEnd = startPos + (WorldTransform.Left + WorldTransform.Forward).Normal * DetectionDistance;
        var rightEnd = startPos + (WorldTransform.Right + WorldTransform.Forward).Normal * DetectionDistance;

        SceneTraceResult leftTrace = Scene.Trace.Ray(startPos, leftEnd).IgnoreGameObjectHierarchy(GameObject).HitTriggers().Radius(DetectionRadius).Run();
        SceneTraceResult rightTrace = Scene.Trace.Ray(startPos, rightEnd).IgnoreGameObjectHierarchy(GameObject).HitTriggers().Radius(DetectionRadius).Run();
        SceneTraceResult straightTrace = Scene.Trace.Ray(startPos, straightEnd).IgnoreGameObjectHierarchy(GameObject).HitTriggers().Radius(DetectionRadius).Run();


        SceneTraceResult[] traces = { straightTrace, leftTrace, rightTrace };

        using (Gizmo.Scope("Detection"))
        {
            foreach (var t in traces)
            {
                Gizmo.Draw.Color = new Color(26, 160, 110);
                Gizmo.Draw.Arrow(t.StartPosition, t.EndPosition, 9f, 3);

                if (t.Hit)
                {
                    Gizmo.Draw.LineSphere(t.HitPosition, 8f);
                }
            }
        }
    }

    private bool CanReachPath(List<Vector3> path)
    {
        if (path.Count == 0) return false;
        Vector3 endPoint = path[^1];

        // Distance to our target position
        float reachDist = Vector3.DistanceBetween(endPoint, curWp.pos);
        return reachDist < 5f;
    }

    private Obstacle FindClosestObstacle(Vector3 From)
    {
        var obstacles = Scene.GetAllComponents<Obstacle>().ToList();
        Obstacle closestObstacle = obstacles[0];
        float closestDist = 9000f;

        foreach (Obstacle obs in obstacles)
        {
            float dist = Vector3.DistanceBetween(obs.WorldPosition, From);

            if (dist < closestDist)
            {
                closestDist = dist;
                closestObstacle = obs;
            }
        }

        return closestObstacle;
    }
}