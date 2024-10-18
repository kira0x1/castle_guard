namespace Kira;

public class Waypoint
{
    public Vector3 pos;
    public Vector3 localPos;
    public Color color;

    public Waypoint PrevWaypoint { get; set; }
    public Waypoint NextWaypoint { get; set; }

    public Waypoint(Vector3 pos, Vector3 localPos)
    {
        this.pos = pos;
        this.localPos = localPos;
    }
}