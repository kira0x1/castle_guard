namespace Kira;

public class Bullet : Component
{
    [Property] public ParticleSystem OnHitParticles { get; set; }

    public GameObject Attacker { get; set; }
    public GameObject Target { get; set; }
    public float Damage { get; set; }

    private const float bulletSpeed = 800;
    private const float minDistance = 35f;

    private TimeSince timeSinceSpawn = 0;

    protected override void OnUpdate()
    {
        if (timeSinceSpawn > 8)
        {
            GameObject.Destroy();
            return;
        }

        var tpos = Target.WorldPosition.WithZ(WorldPosition.z);
        WorldPosition += LocalTransform.Forward * bulletSpeed * Time.Delta;

        float dist = Vector3.DistanceBetween(WorldPosition, tpos);

        if (dist <= minDistance)
        {
            OnHit();
        }
    }

    private void OnHit()
    {
        var enemy = Target.GetComponent<EnemyStats>();
        if (enemy.IsValid())
        {
            var info = new DamageInfo(Damage, Attacker, Attacker);
            enemy.TakeDamage(info, WorldTransform.Forward * (Damage * 5f), WorldPosition);
        }

        GameObject.Destroy();
    }
}