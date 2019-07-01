using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ---------------------------------------------------------------------------------
/// MORE COMPLEX DRAWING USING COLORS
/// READING IN A DATA FILE
/// ---------------------------------------------------------------------------------
/// 
/// PART 1:
/// Write a function DrawManySquares that can draw multiple squares in the center of the screen where 
/// the first square is 2x2 and each square after that is 2 units bigger than the last square,
/// but still centered around the previous square.
/// 
/// PART 2:
/// Modify your previous function so that the color of each square changes as it moves out from the center.
/// Use the Color.HSVtoRGB to change the color hue for each ring.
/// 
/// PART 3:
/// Review the drawing text file in the Resources folder. What do you t hink it will draw.
/// Use the TextReader.ReadDrawing to read in the drawing data
/// Write a function that will use the DrawingData to draw its picture on screen.
/// 
/// HOMEWORK:
/// Create your own simple drawing on graph paper. 
/// Then write down the colors and points data into your own drawing data file
/// Save the file in resources and then call your drawing function passing the name of your new data file.
/// </summary>

public class Assignment4 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DrawFromDataFile("SimpleX");
    }

    // PART 1 & 2
    public void DrawManySquares()
    {
        // TODO:
    }

    // PART 3 & HOMEWORK
    public void DrawFromDataFile(string fileName)
    { 
        // TODO:
    }
}
