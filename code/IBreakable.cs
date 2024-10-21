namespace Kira;

public interface IBreakable
{
    public GameObject GameObj { get; set; }
    public bool IsBroken { get; set; }
    public float Health { get; set; }
    public void TakeDamage(DamageInfo damage);
}