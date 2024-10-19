namespace Kira;

public class EnemyStats : Component
{
    public float Health { get; set; } = 100;
    public float MaxHealth { get; set; } = 100;

    [Property]
    private EnemyController Controller { get; set; }

    public void TakeDamage(DamageInfo info, Vector3 velocity = default)
    {
        Health -= info.Damage;
        Controller.OnHit(info, velocity);
    }
}