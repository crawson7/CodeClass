using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextReader
{
    public static List<Vector2> ReadPoints(string fileName)
    {
        List<Vector2> list = new List<Vector2>();
        TextAsset file = Resources.Load<TextAsset>(fileName);
        if (file == null)
        {
            Debug.LogError("Cound Not find File: " + fileName + " in the Resources Folder");
            return list;
        }

        string[] pairs = file.text.Split(new char[] { '\n' });
        for (int i = 0; i < pairs.Length; i++)
        {
            list.Add(ParsePoint(pairs[i]));
        }
        return list;
    }

    public static DrawingData ReadDrawing(string fileName)
    {
        DrawingData drawFile = new DrawingData();
        TextAsset file = Resources.Load<TextAsset>(fileName);
        if (file == null)
        {
            Debug.LogError("Cound Not find File: " + fileName + " in the Resources Folder");
            return drawFile;
        }

        drawFile = ParseDrawing(file.text);
        return drawFile;
    }

    public static DrawingData ParseDrawing(string file)
    {
        DrawingData drawFile = new DrawingData();
        string[] lines = file.Replace("\r\n", "\n")
                               .Replace("\r", "\n")
                               .Split(new string[] { "line\n" }, System.StringSplitOptions.RemoveEmptyEntries);
        //string[] lines = file.Split(new string[] { "line" }, System.StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < lines.Length; i++)
        {
            DrawingLine line = ReadDrawingLine(lines[i]);
            drawFile.Lines.Add(line);
        }
        return drawFile;
    }

    public static DrawingLine ReadDrawingLine(string text)
    {
        DrawingLine line = new DrawingLine();
        string[] pairs = text.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        string colorString = pairs[0];
        line.Color = ParseColor(colorString);
        for (int i = 1; i < pairs.Length; i++)
        {
            Vector2 point = ParsePoint(pairs[i]);
            line.Points.Add(point);
        }
        return line;
    }

    public static Vector2 ParsePoint(string point)
    {
        string[] parts = point.Split(new char[] { ',' });
        if (parts.Length != 2)
        {
            Debug.LogError("ERROR: Your point " + point + " is not formatted correclty.");
            return Vector2.zero;
        }

        int x, y;
        if (int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y))
        {
            // Data points successfully converted.
            Vector2 vec = new Vector2(x, y);
            return vec;
        }
        else
        {
            Debug.LogError("ERROR: Could not convert points data: " + point + " to a Vector3");
        }
        return Vector2.zero;
    }

    public static Color ParseColor(string c)
    {
        string[] parts = c.Split(new char[] { ',' });
        float h, s, v;

        if (parts.Length != 3)
        {
            Debug.LogError("ERROR: Your points on color " + c + " are not formatted correclty.");
            return Color.white;
        }

        if (float.TryParse(parts[0], out h) && float.TryParse(parts[1], out s) && float.TryParse(parts[2], out v))
        {
            // Data points successfully converted.
            Color color = Color.HSVToRGB(h, s, v);
            return color;
        }
        else
        {
            Debug.LogError("ERROR: Could not convert color string: " + c);
        }
        return Color.white;
    }
}

public class DrawingData
{
    public List<DrawingLine> Lines = new List<DrawingLine>();

    public override string ToString()
    {
        return "This drawing has " + Lines.Count + " lines.";
    }
}

public class DrawingLine
{
    public Color Color;
    public List<Vector2> Points = new List<Vector2>();
}
