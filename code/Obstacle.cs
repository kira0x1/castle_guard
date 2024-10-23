namespace Kira;

using System;

public sealed class Obstacle : Component, IBreakable
{
    public GameObject GameObj { get; set; }
    public bool IsBroken { get; set; }
    public float MaxHealth { get; set; }
    public float Health { get; set; } = 100;
    public Prop prop { get; set; }

    protected override void OnAwake()
    {
        base.OnAwake();
        prop = Components.Get<Prop>();

        MaxHealth = prop.Health;
        Health = prop.Health;

        prop.OnPropTakeDamage += OnPropTakeDamage;
        prop.OnPropBreak += OnPropBreak;
    }

    public void TakeDamage(DamageInfo damage)
    {
        prop.OnDamage(damage);
        Health = prop.Health;
    }

    private void OnPropTakeDamage(DamageInfo info)
    {
        Health = prop.Health;
    }

    private void OnPropBreak()
    {
        IsBroken = true;
        prop.IsStatic = false;
        prop.Enabled = false;

        StageManager.Instance.GenerateNav();
    }
}