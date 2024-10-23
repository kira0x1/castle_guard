namespace Kira;

using System;

public sealed class WaypointManager : Component, Component.ExecuteInEditor
{
    [Property, Range(0, 100f)] private float GizmoRadius { get; set; } = 5f;
    [Property, Range(0, 50f)] private float zOffset { get; set; } = 5f;

    [Property] private Color StartColor { get; set; } = Color.Cyan;
    [Property] private Color EndColor { get; set; } = Color.Magenta;

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
            var wpPos = wp.localPos;
            wpPos.z += zOffset;

            var nextPos = wp.NextWaypoint.localPos;
            nextPos.z += zOffset;

            Gizmo.Draw.LineSphere(wpPos, GizmoRadius);
            Gizmo.Draw.Line(wpPos, nextPos);
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
        int inbetweenCount = children.Count > 2 ? children.Count - 2 : 0;
        Gradient.ColorFrame[] cframes = new Gradient.ColorFrame[inbetweenCount];

        for (int i = 0; i < inbetweenCount; i++)
        {
            cframes[i].Value = Color.Lerp(StartColor, EndColor, (i * 1f) / (inbetweenCount * 1f));
            cframes[i].Time = i;
        }

        Gradient gradient = new Gradient(cframes);

        List<Waypoint> points = new List<Waypoint>();

        var firstNode = children[0];
        var lastNode = children[^1];

        var firstWp = new Waypoint(firstNode.WorldPosition, firstNode.LocalPosition);
        var lastWp = new Waypoint(lastNode.WorldPosition, lastNode.LocalPosition);

        firstWp.PrevWaypoint = lastWp;
        lastWp.NextWaypoint = firstWp;


        firstWp.color = StartColor;
        lastWp.color = EndColor;

        points.Add(firstWp);

        for (int i = 1; i < children.Count - 1; i++)
        {
            Waypoint wp = new Waypoint(children[i].WorldPosition, children[i].LocalPosition);
            wp.PrevWaypoint = points[i - 1];

            points[i - 1].NextWaypoint = wp;
            wp.color = Color.Lerp(StartColor, EndColor, i / (children.Count * 1f));
            points.Add(wp);
        }

        lastWp.PrevWaypoint = points[^1];
        points[^1].NextWaypoint = lastWp;
        points.Add(lastWp);

        waypoints = points;
    }
}