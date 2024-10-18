namespace Kira;

public class EnemyStats : Component
{
    public float Health { get; set; } = 100;
    public float MaxHealth { get; set; } = 100;

    [Property]
    private EnemyController Controller { get; set; }

    public void TakeDamage(float damage, GameObject attacker)
    {
        Health -= damage;
        DamageInfo info = new DamageInfo(damage, attacker, attacker);
        Controller.OnHit(info);
    }
}