namespace Kira;

public class PlayerInteractor : Component
{
    [Property] private GameObject RayObj { get; set; }
    [Property, Range(0, 200f)] private float InteractionDistance { get; set; } = 55f;


    protected override void OnUpdate()
    {
        var rayStart = RayObj.WorldPosition;
        var rayEnd = rayStart + RayObj.WorldTransform.Forward * InteractionDistance;
        var trace = Scene.Trace.FromTo(rayStart, rayEnd).Radius(20).Run();

        Gizmo.Draw.Color = Color.Magenta;
        Gizmo.Draw.Line(trace.StartPosition, trace.EndPosition);

        if (trace.Hit)
        {
            Gizmo.Draw.LineSphere(trace.EndPosition, 12f);
        }
    }
}