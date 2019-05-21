
using UnityEngine;

/*
 * Class to store properties of every individual "cell" in the maze
 */

public class Cell
{
    [HideInInspector]
    public GameObject box, topWall, botWall, leftWall, rightWall; // Storing cell/box prefab and four walls surrounding each cell

    private int x, y; // x and y coordinate of object
    private bool empty = false;

    // Constructor
    public Cell(int i, int j, GameObject currBox)
    {
        x = i;
        y = j;
        box = currBox;
        empty = false;
    }

    // "Default" contructor
    public Cell()
    {
        empty = true;
    }

    // Returns x-coordinate of cell
    public int GetX()
    {
        return x;
    }

    // Returns y-coordinate of cell
    public int GetY()
    {
        return y;
    }

    // Return true if the cell is "empty"
    public bool IsEmpty()
    {
        return empty;
    }



}
