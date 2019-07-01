using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PART 1:
/// Use the Draw utility to draw a Line going from one corner of the screen to the other.
/// Draw.Line takes 4 parameters. Thickness, color, a bool, and a list of Vector2s.
/// The bool tells the function if the line should be a closed loop.
/// If it is set to true, it will draw an extra line from the last point back to the start point.
/// 
/// PART 2:
/// Use the Draw utilty to draw a square in the center of the screen.
/// 
/// 
/// PART 3: 
/// Write a function that will Draw vertical Lines from top to bottom all the way across the screen.
/// There should be 1 unit of space between each Line. That means that there will be 50 lines.
/// 
/// PART 4:
/// Write a function that will draw a grid on the screen.
/// Your function takes in two parameters. The width of the grid and the height of the grid.
/// The size of your grid needs to match the values for width and height.
/// 
/// HOMEWORK:
/// Write a function DrawManySquares that can draw multiple squares in the center of the screen where 
/// the first square is 2x2 and each square after that is 2 units bigger than the last square,
/// but still centered around the previous square.
/// </summary>

public class Assignment3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // PART 3
    public void DrawVerticalLines()
    {
        // TODO:
    }

    // PART 4
    public void DrawGrid(int width, int height)
    {
        // TODO: 
    }

    // HOMEWORK
    public void DrawManySquares(int howMany)
    {
        // TODO: Write a function to draw multiple squares in the center of the screen.
    }
}
