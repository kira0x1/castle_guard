using System;

public sealed class WaypointManager : Component, Component.ExecuteInEditor
{
    [Property, Range(0, 100f)] private float GizmoRadius { get; set; } = 5f;
    [Property] private Color GizmoColor { get; set; } = Color.Cyan;
    private List<Waypoint> waypoints = new List<Waypoint>();

    protected override void OnAwake()
    {
        LoadWaypoints();
    }

    protected override void DrawGizmos()
    {
        base.DrawGizmos();

        var children = GameObject.Children;
        for (var i = 0; i < children.Count; i++)
        {
            GameObject tr = children[i];
            tr.Name = $"Waypoint {i + 1}";
        }

        LoadWaypoints();

        foreach (Waypoint wp in waypoints)
        {
            Gizmo.Draw.Color = wp.color;
            Gizmo.Draw.LineSphere(wp.localPos, GizmoRadius);
            Gizmo.Draw.Line(wp.localPos, wp.NextWaypoint.localPos);
        }
    }

    public Waypoint GetWaypoint(int index)
    {
        if (index < 0)
        {
            return waypoints[0];
        }

        return index > waypoints.Count - 1 ? waypoints[^1] : waypoints[index];
    }

    public Waypoint GetRandomWaypoint()
    {
        return Random.Shared.FromList(waypoints);
    }

    private void LoadWaypoints()
    {
        var children = GameObject.Children;

        List<Waypoint> points = new List<Waypoint>();

        var firstNode = children[0];
        var lastNode = children[^1];

        var firstWp = new Waypoint(0, firstNode.WorldPosition, firstNode.LocalPosition);
        var lastWp = new Waypoint(children.Count - 1, lastNode.WorldPosition, lastNode.LocalPosition);

        firstWp.PrevWaypoint = lastWp;
        lastWp.NextWaypoint = firstWp;

        firstWp.color = Color.Cyan;
        lastWp.color = GizmoColor;

        points.Add(firstWp);

        for (int i = 1; i < children.Count - 1; i++)
        {
            Waypoint wp = new Waypoint(i, children[i].WorldPosition, children[i].LocalPosition);
            wp.PrevWaypoint = points[i - 1];

            points[i - 1].NextWaypoint = wp;
            wp.color = GizmoColor;
            points.Add(wp);
        }

        lastWp.PrevWaypoint = points[^1];
        points[^1].NextWaypoint = lastWp;
        points.Add(lastWp);

        waypoints = points;
    }
}

public class Waypoint
{
    public int index;
    public Vector3 pos;
    public Vector3 localPos;
    public Color color;

    public Waypoint PrevWaypoint { get; set; }
    public Waypoint NextWaypoint { get; set; }

    public Waypoint(int index, Vector3 pos, Vector3 localPos)
    {
        this.index = index;
        this.pos = pos;
        this.localPos = localPos;
    }
}