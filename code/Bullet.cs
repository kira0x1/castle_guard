namespace Kira;

public class Bullet : Component
{
    public GameObject Attacker { get; set; }
    public GameObject Target { get; set; }
    public float Damage { get; set; }

    private Vector3 velocity;

    private const float bulletSpeed = 800;
    private const float minDistance = 35f;

    protected override void OnUpdate()
    {
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
            var info = new DamageInfo(Damage, Attacker, null);
            enemy.TakeDamage(info, WorldTransform.Forward * (Damage * 5f));
        }

        GameObject.Destroy();
    }

    private void Smooth()
    {
        var tpos = Target.WorldPosition.WithZ(WorldPosition.z);
        WorldPosition = Vector3.SmoothDamp(WorldPosition, tpos, ref velocity, 0.6f, Time.Delta);
    }
}