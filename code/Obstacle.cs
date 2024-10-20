namespace Kira;

public sealed class Obstacle : Component
{
    public float Health { get; set; } = 100;
    public Prop prop { get; set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        prop = Components.Get<Prop>();
        prop.OnPropBreak += OnPropBreak;
    }

    private void OnPropBreak()
    {
        prop.IsStatic = false;
        prop.Enabled = false;
        Scene.NavMesh.Generate(Scene.PhysicsWorld);
    }
}