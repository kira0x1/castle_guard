namespace Kira;

public sealed class Obstacle : Component, IBreakable
{
    public GameObject GameObj { get; set; }
    public bool IsBroken { get; set; }
    public float Health { get; set; } = 100;
    public Prop prop { get; set; }

    public void TakeDamage(DamageInfo damage)
    {
        prop.OnDamage(damage);
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        prop = Components.Get<Prop>();
        prop.OnPropBreak += OnPropBreak;
    }

    private void OnPropBreak()
    {
        IsBroken = true;
        prop.IsStatic = false;
        prop.Enabled = false;
        Scene.NavMesh.Generate(Scene.PhysicsWorld);
    }
}