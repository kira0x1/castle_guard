namespace Kira;

using System;

public sealed class Turret : Component
{
    /// <summary>
    /// Shots per second
    /// 5 shots per second: 5/1 = 0.2f = 200ms each shot
    /// </summary>
    [Property, Group("Combat"), Range(0.1f, 40)] public float FireRate { get; set; } = 5;
    [Property, Group("Combat"), Range(0, 3)] private float TurnSmoothing { get; set; } = 0.04f;
    [Property, Group("Combat"), Range(1, 100)] public float Damage { get; set; } = 20;
    [Property, Group("Combat"), Range(1, 100)] public float ForwardPredictionAim { get; set; } = 20;
    [Property, Group("Combat"), Range(1, 900)] public float MinShootDistance { get; set; } = 150;

    [Property, Group("Particles")] public ParticleSystem MuzzleFlash { get; set; }
    [Property, Group("Particles")] public ParticleSystem MuzzleSmoke { get; set; }

    [Property] public GameObject Target { get; set; }
    [Property] private GameObject Nozzle { get; set; }
    [Property] private GameObject FirePos { get; set; }
    [Property] private GameObject BulletPrefab { get; set; }

    [Property] private bool ShowShootRadius { get; set; }
    [Property] private bool ShowAimPrediction { get; set; }

    private Vector3 velocity;
    private TimeSince lastShootTime;
    private float fireRateMs;

    protected override void OnStart()
    {
        base.OnStart();
        fireRateMs = 1 / FireRate;
    }

    protected override void OnUpdate()
    {
        Aim();
        fireRateMs = 1 / FireRate;


        if (lastShootTime > fireRateMs)
        {
            float distance = Vector3.DistanceBetween(WorldPosition, Target.WorldPosition);
            if (distance < MinShootDistance)
            {
                Shoot();
                lastShootTime = 0;
            }
        }
    }

    protected override void DrawGizmos()
    {
        if (!ShowShootRadius) return;
        Gizmo.Draw.LineSphere(Vector3.Zero, MinShootDistance, 10);
    }

    private float GetAimAngle()
    {
        var fwd = Nozzle.LocalTransform.Forward;
        var inbetween = Target.WorldPosition - Nozzle.WorldPosition;
        return Vector3.Dot(fwd, inbetween.Normal);
    }

    private void Aim()
    {
        var agent = Target.GetComponent<NavMeshAgent>();
        var targetVel = agent.Velocity;
        var aimPredictionPos = Target.WorldPosition + targetVel * ForwardPredictionAim;
        var nozzleZ = Nozzle.WorldPosition.z;
        aimPredictionPos = aimPredictionPos.WithZ(nozzleZ);

        if (ShowAimPrediction)
        {
            Gizmo.Draw.Arrow(Nozzle.WorldPosition, aimPredictionPos);
        }

        var inbetween = aimPredictionPos - Nozzle.WorldPosition;

        var lookRot = Rotation.LookAt(inbetween);
        lookRot = lookRot.Angles().WithPitch(0).WithRoll(0);

        var curRot = Nozzle.LocalRotation;
        Nozzle.LocalRotation = Rotation.SmoothDamp(curRot, lookRot, ref velocity, TurnSmoothing, Time.Delta);
    }

    private void Shoot()
    {
        var clone = BulletPrefab.Clone(FirePos.WorldPosition, Nozzle.LocalRotation);
        Bullet bullet = clone.Components.Get<Bullet>();

        bullet.Damage = Damage;
        bullet.Attacker = GameObject;
        bullet.Target = Target;

        CreateParticle(MuzzleFlash, Nozzle.WorldTransform, 1f, ParticleToMuzzlePos);
        CreateParticle(MuzzleSmoke, Nozzle.WorldTransform, 1f, ParticleToMuzzlePos);
    }

    void ParticleToMuzzlePos(SceneParticles particles)
    {
        var pos = FirePos.WorldPosition;

        // Apply velocity to prevent muzzle shift when moving fast
        particles?.SetControlPoint(0, pos + velocity * 0.03f);
        particles?.SetControlPoint(0, Nozzle.WorldRotation);
    }

    public void CreateParticle(ParticleSystem particle, Transform transform, float scale, Action<SceneParticles> OnFrame = null)
    {
        SceneParticles particles = new(Scene.SceneWorld, particle);
        particles.SetControlPoint(0, transform.Position);
        particles?.SetControlPoint(0, transform.Rotation);
        particles?.SetNamedValue("scale", scale);

        particles?.PlayUntilFinished(Task, OnFrame);
    }

    public SceneTraceResult TraceBullet(Vector3 start, Vector3 end, float radius = 2.0f)
    {
        var tr = Scene.Trace.Ray(start, end)
            .UseHitboxes()
            .Size(radius)
            .IgnoreGameObjectHierarchy(GameObject)
            .Run();

        return tr;
    }
}