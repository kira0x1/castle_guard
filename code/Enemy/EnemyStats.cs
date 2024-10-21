namespace Kira;

using System;

public class EnemyStats : Component
{
    public float Health { get; set; } = 100;
    public float MaxHealth { get; set; } = 100;

    [Property]
    private ParticleSystem BloodParticles { get; set; }

    [Property]
    private EnemyController Controller { get; set; }

    public void TakeDamage(DamageInfo info, Vector3 velocity = default, Vector3 hitPos = default)
    {
        Health -= info.Damage;
        Controller.OnHit(info, velocity);

        Transform hitTransform = WorldTransform;
        hitTransform.Position = hitPos;

        CreateParticle(BloodParticles, hitTransform, 1f);
    }

    public void CreateParticle(ParticleSystem particle, Transform transform, float scale, Action<SceneParticles> OnFrame = null)
    {
        SceneParticles particles = new(Scene.SceneWorld, particle);
        particles.SetControlPoint(0, transform.Position);
        particles.SetControlPoint(0, transform.Rotation);
        particles.SetNamedValue("scale", scale);
        particles.PlayUntilFinished(Task, OnFrame);
    }
}