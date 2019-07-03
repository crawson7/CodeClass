using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ---------------------------------------------------------------------------------
/// INTRODUCE VECTOR2 STRUCT
/// INDRODUCTION TO DRAWING POINTS USING DRAW UTILITY
/// ---------------------------------------------------------------------------------
/// 
/// PART 1:
/// Create a Vector2 using new Vector2().
/// Use Debug.Log to print out the value of your Vector2.
/// 
/// PART 2:
/// Use the Draw Utility and a Vector2 to draw a Point on the screen
/// Draw.Point takes 2 parameters. A color and a Vector2.
/// The Camera can see points from (0,0) to (50,50)
/// 
/// PART 3:
/// Use the Draw Utility to draw points in all four corners of the screen.
/// 
/// PART 4:
/// Write a function that will loop through any list of Vector2 and draw each of its points.
/// Put your 4 corner points in a list.
/// Add A fifth point in the center of the screen.
/// Use your new "DrawPoints" function to draw all 5 points.
/// 
/// HOMEWORK:
/// Draw 50 points on the screen making a dotted line from the bottom left corner to the top right.
/// Step 1 - Create a list of Vector2s
/// Step 2 - Use a "for" loop to fill the list with Vector2s (what needs to change each loop?)
/// Step 3 - Use your draw points function to draw the list of points.
/// </summary>

public class Assignment2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // PART 4
    public void DrawPoints(List<Vector2> myList)
    {
        // TODO: Implement
    }
}
