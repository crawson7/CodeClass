using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //for(int i=1;i<10; i++)
        //{
        //    List<Vector2> list = new List<Vector2>();
        //    list.Add(new Vector2(i, i));
        //    list.Add(new Vector2(i, -i));
        //    list.Add(new Vector2(-i, -i));
        //    list.Add(new Vector2(-i, i));
        //    Color c = Color.HSVToRGB((float)i/10.0f, 1.0f, 1.0f);
        //    Line.Draw(1, c, true, list);
        //}

        List<Vector2> myPoints = TextReader.ReadPoints("Points");
        Draw.Line(1.0f, Color.green, true, myPoints);
    }
}
