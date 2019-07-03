using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ---------------------------------------------------------------------------------
/// GAME OBJECTS AND TRANSFORMS
/// THE UPDATE LOOP
/// MOVING OBJECTS OVER TIME AND SIMPLE LOGIC
/// ---------------------------------------------------------------------------------
/// 
/// PART 1:
/// Change the position of the cube when the program starts.
/// Create a new Vector3 and give it coordinates.
/// Set the position property on the cube to equal your Vector3.
/// 
/// PART 2:
/// Use the update loop to move the cube up over time.
/// Use Time.deltaTime to move the cube 1 meter per second.
/// Change it's speed to 3 meters per second.
/// What happens to the cube if you let the game run for more than a couple seconds?
/// 
/// Part 3:
/// Put some logic in your update loop to make the cube change direction when it reaches the top of the screen.
/// Then Add some more logic to make the cube change direction again once it reaches the bottom of the screen.
/// The cube should now move up and down over and over again without leaving the screen view.
/// 
/// HOMEWORK:
/// Create another public Transform reference in your assignment. Add another cube to your scene and Assign it's reference.
/// Make this second cube move side to side instead of up and down.
/// </summary>

public class Assignment5 : MonoBehaviour
{
    public Transform myCube;

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // This is called once every frame.
    private void Update()
    {
        
    }
}
