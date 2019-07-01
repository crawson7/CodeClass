using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextReader
{
    public static List<Vector2> ReadPoints(string fileName)
    {
        List<Vector2> list = new List<Vector2>();
        TextAsset file = Resources.Load<TextAsset>(fileName);
        if(file == null)
        {
            Debug.LogError("Cound Not find File: " + fileName + " in the Resources Folder");
            return list;
        }

        string[] pairs = file.text.Split(new char[] { '\n' });
        for (int i = 0; i < pairs.Length; i++)
        {
            string[] parts = pairs[i].Split(new char[] { ','});
            if(parts.Length != 2)
            {
                Debug.LogError("ERROR: Your points on Line " + i + " are not formatted correclty.");
                return new List<Vector2>();
            }

            int x, y;
            if (int.TryParse(parts[0], out x) && int.TryParse(parts[1], out y))
            {
                // Data points successfully converted.
                Vector2 vec = new Vector2(x, y);
                list.Add(vec);
            }
            else
            {
                Debug.LogError("ERROR: Could not convert points data: " + pairs[i] + " to a Vector3");
            }
        }
        return list;
    }
}
