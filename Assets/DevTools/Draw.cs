using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Draw
{
    public static void Line(float thickness, Color color, bool close, List<Vector2> points)
    {
        GameObject go = new GameObject("Line");
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.startWidth = thickness * 0.25f;
        line.endWidth = thickness * 0.25f;
        line.alignment = LineAlignment.View;
        line.startColor = color;
        line.endColor = color;
        line.loop = close;

        Material m = Resources.Load("LineMaterial") as Material;
        line.material = m;
        Vector3[] positions = new Vector3[points.Count];
        for(int i=0;i<points.Count; i++)
        {
            positions[i] = new Vector3(points[i].x, points[i].y, 0);
        }
        line.positionCount = points.Count;
        line.SetPositions(positions);
    }

    public static void Point(Color color, Vector2 point)
    {
        GameObject go = new GameObject("Point");
        LineRenderer line = go.AddComponent<LineRenderer>();
        line.startWidth = 0.3f;
        line.endWidth = 0.3f;
        line.alignment = LineAlignment.View;
        line.startColor = color;
        line.endColor = color;

        Material m = Resources.Load("LineMaterial") as Material;
        line.material = m;
        line.numCapVertices = 8;
        line.positionCount = 2;
        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(point.x, point.y, 0);
        positions[1] = new Vector3(point.x + 0.01f, point.y);
        line.SetPositions(positions);
    }
}
