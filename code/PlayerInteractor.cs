namespace Kira;

public class PlayerInteractor : Component
{
    [Property] private GameObject RayObj { get; set; }
    [Property, Range(0, 200f)] private float InteractionDistance { get; set; } = 55f;

    public bool HasObstacle { get; set; }
    public Obstacle ObstacleTarget { get; set; }

    protected override void OnUpdate()
    {
        HandleTrace();

        if (!HasObstacle) return;

        if (Input.Pressed("Slot1"))
        {
            ObstacleTarget.TakeDamage(new DamageInfo(25f, GameObject, GameObject));
        }
    }

    private void HandleTrace()
    {
        var rayStart = RayObj.WorldPosition;
        var rayEnd = rayStart + RayObj.WorldTransform.Forward * InteractionDistance;
        var trace = Scene.Trace.Ray(rayStart, rayEnd).HitTriggers().Radius(30).Run();

        Gizmo.Draw.Color = Color.Magenta;
        Gizmo.Draw.Line(trace.StartPosition, trace.EndPosition);

        if (trace.Hit)
        {
            Gizmo.Draw.LineSphere(trace.EndPosition, 8f);

            if (trace.GameObject.Components.TryGet(out Obstacle obs))
            {
                Log.Info(obs.Health);
                ObstacleTarget = obs;
                HasObstacle = true;
            }
            else
            {
                HasObstacle = false;
            }
        }
        else
        {
            HasObstacle = false;
        }
    }
}