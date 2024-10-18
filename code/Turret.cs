namespace Kira;

public sealed class Turret : Component
{
    [Property, Group("Combat"), Range(1, 100)] public float Damage { get; set; } = 20;

    /// <summary>
    /// Shots per second
    /// 5 shots per second: 5/1 = 0.2f = 200ms each shot
    /// </summary>
    [Property, Group("Combat"), Range(1, 100)] public float FireRate { get; set; } = 5;
    [Property, Group("Combat"), Range(0, 3)] private float TurnSpeed { get; set; } = 0.1f;

    [Property] public GameObject Target { get; set; }
    [Property] private GameObject Nozzle { get; set; }

    private Vector3 velocity;

    protected override void OnUpdate()
    {
        var npos = Nozzle.WorldPosition;
        var targetPos = Target.WorldPosition;

        var fwd = Nozzle.LocalTransform.Forward;
        var inbetween = Target.WorldPosition - Nozzle.WorldPosition;

        var d = Vector3.Dot(fwd, inbetween.Normal);
        Log.Info($"{d:F2}");

        var lookDir = Rotation.LookAt(inbetween);
        lookDir = lookDir.Angles().WithPitch(0).WithRoll(0);

        var curRot = Nozzle.LocalRotation;
        Nozzle.LocalRotation = Rotation.SmoothDamp(curRot, lookDir, ref velocity, TurnSpeed, Time.Delta);
    }
}