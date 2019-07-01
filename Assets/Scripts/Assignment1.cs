using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PART 1:
/// Set the welcome message to print to the console, and run the program.
/// Change the welcome message to something new.
/// 
/// PART 2:
/// Write the functionality for the Add, Subtract and Multipy Functions.
/// Print the result to the console. What should the result be?
/// 
/// PART 3:
/// Write a for loop that adds the number 6, 4183 times.
/// Use the multiply function to multiply 6 times 4183 to verify your result.
/// Print both answers to the console.
/// 
/// HOMEWORK:
/// Write a function the will add all of the numbers in the list together, and print out the result.
/// Pass the HomeworkList to your function and run the program. The result should print 120. 
/// Create your own list of int and fill it with 10 numbers.
/// </summary>

public class Assignment1 : StartScript
{
    public List<int> HomeworkList { get { return new List<int> { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15}; } }

    public override void Run()
    {
        string welcomeMessage = "Welcome to Your first Assignment!";
        Debug.Log("");

        int c = Add(5, 10);
        int d = Add(2, 3);
        int e = Subtract(c, d);
        int f = Multiply(e, 10);
        // TODO: Print the value of "f" to the console.
    }

    public int Add(int a, int b)
    {
        // TODO: return the correct result for adding a and b.
        return 0;
    }

    public int Subtract(int a, int b)
    {
        // TODO: Return the correct result for subtracting b from a.
        return 0;
    }

    public int Multiply(int a, int b)
    {
        // TODO: return the correct result for multiplying a times b.
        return 0;
    }

    // HOMEWORK
    public void AddList(List<int> numbers)
    {
        // TODO: Add the list of numbers together and print the result
    }

    public void ForLoopExample()
    {
        // Define an int bucket and call it i and set it's value to 0...
        // As long as i is less than some number. In this case 10...
        // After each loop perform i++ which means add one to i.

        for(int i=0;i<10;i++)
        {
            // Do something 10 times.
        }
    }

}
