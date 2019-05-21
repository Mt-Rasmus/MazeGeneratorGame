
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Class which handles the creating of a "perfect" maze through user input,
 * using prefabs and the Depth First Search method + Backtracking
 * It also handles the destruction of the maze and instantiation of the player character
 */

public class CellCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject blockHolder, wallHolder, blockPrefab, coinHolder, wall, playerPrefab;
    private GameObject playerInstance;
    private CoinCreator coinCreator;
    private ScoreChanger scoreChanger;
    private int width = 4, height = 4; // variables to store width and height of maze
    private float w; // variable to store block width
    private int nrOfCreatedCells;
    private int genYieldLimitGrid = 600; 
    private int genYieldLimitMaze = 600;
    private Cell[] cells;
    private Hashtable visitedCellsTable; // hashtable to store indices of visited cells
    private bool generatingMaze = false;

    private void Start()
    {
        nrOfCreatedCells = 0;
        w = blockPrefab.GetComponent<Renderer>().bounds.size.x; // width of a block
        visitedCellsTable = new Hashtable();
        coinCreator = coinHolder.GetComponent<CoinCreator>();
        scoreChanger = GameObject.FindGameObjectWithTag("ScoreMaster").GetComponent<ScoreChanger>();
    }

    /* 
     * Method to start the process of generating perfect maze
     * Called through pressing (Re)generate maze button in GUI 
     * First removes old maze 
     */
    public void GenerateMaze()
    {
        // If a maze is not currently being generated, proceed to generate new maze
        if(!generatingMaze)
        {
            generatingMaze = true;
       
            if (cells != null && cells.Length > 0)
                DestroyOldMaze(); // Call to destroy old maze to make way for a new one

            visitedCellsTable.Clear(); // Clear hashtable of visited cell indices

            StartCoroutine(GenerateGrid()); // Calling method to create the block/cell grid    
        }
    }

    // Spawn player after delay
    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(0.25f);
        // Instantiate player character in position of bottom left cell
        playerInstance = Instantiate(playerPrefab, new Vector3(-(w * width / 2) + w / 2, -(w * height / 2) + w / 2, -0.01f), Quaternion.identity);
        generatingMaze = false;
    }

    // Method to remove all object within the previously generated maze to make way for the new one
    private void DestroyOldMaze()
    {
        if (playerInstance)
            DestroyImmediate(playerInstance); // Remove player character from the scene

        // Remove all blocks/cells from the scene
        DestroyAllChildObjects(blockHolder);

        // Remove all walls from the scene
        DestroyAllChildObjects(wallHolder);

        // Remove uncollected coins from the scene
        DestroyAllChildObjects(coinHolder);
        
        scoreChanger.ResetScore(); // Set score back to zero
    }

    private void DestroyAllChildObjects(GameObject parentObj)
    {
        int children = parentObj.transform.childCount;
        for (int i = 0; i < children; i++)
            DestroyImmediate(parentObj.transform.GetChild(0).gameObject);
    }

    // Method to create the base grid; all needed cells and walls
    private IEnumerator GenerateGrid()
    {      
        cells = new Cell[width * height];  // Definíng array storing all cell objects
        Vector2[] cellPositions = new Vector2[width * height]; // Array to store all cell positions in grid
        float cellPosX;
        float cellPosY;

        int addWidth = 0; // Variable to help indexing the cell array correctly

        // Nested loop to create the grid with no walls missing
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                cellPosX = i * w - (w * width / 2) + w / 2; // x coordinate of current cell to be placed
                cellPosY = j * w - (w * height / 2) + w / 2; // y coordinate of current cell to be placed

                GameObject currBox = Instantiate(blockPrefab, new Vector3(cellPosX, cellPosY, 0), Quaternion.identity);
                cellPositions[i + addWidth] = new Vector2(cellPosX, cellPosY);

                currBox.transform.parent = blockHolder.transform;

                // Instantiating walls for the current cell
                GameObject leftwall = Instantiate(wall, new Vector3(currBox.transform.position.x - w / 2, currBox.transform.position.y, -0.01f), Quaternion.identity);
                GameObject topwall = Instantiate(wall, new Vector3(currBox.transform.position.x, currBox.transform.position.y + w / 2, -0.01f), Quaternion.Euler(new Vector3(0,0,90)));
                GameObject rightwall = Instantiate(wall, new Vector3(currBox.transform.position.x + w / 2, currBox.transform.position.y, -0.01f), Quaternion.identity);
                GameObject botwall = Instantiate(wall, new Vector3(currBox.transform.position.x, currBox.transform.position.y - w / 2, -0.01f), Quaternion.Euler(new Vector3(0,0,90)));

                // Add new cell to cell array
                cells[i + addWidth] = new Cell(i, j, currBox);

                // Add walls to cell object
                cells[i + addWidth].leftWall = leftwall;
                cells[i + addWidth].topWall = topwall;
                cells[i + addWidth].rightWall = rightwall;
                cells[i + addWidth].botWall = botwall;

                // Parent the cell walls
                cells[i + addWidth].leftWall.transform.parent = wallHolder.transform;
                cells[i + addWidth].topWall.transform.parent = wallHolder.transform;
                cells[i + addWidth].rightWall.transform.parent = wallHolder.transform;
                cells[i + addWidth].botWall.transform.parent = wallHolder.transform;

                nrOfCreatedCells++;

                // Yields after genYieldLimit cells to not have the whole application window "freeze" too long for large mazes
                if (nrOfCreatedCells % genYieldLimitGrid == 0)
                    yield return null;

            }
            addWidth += width;
        }
        coinCreator.SetSpawnPoints(cellPositions); // Give coinCreator all cell positions (needed to position coins)
        coinCreator.GenerateCoins();
        StartCoroutine(DepthFirstSearch()); // Start main algorithm
    }

    // Method that perfroms main algorithm to create maze (remove appropriate walls) - Depth First Search + Backtracking
    private IEnumerator DepthFirstSearch()
    {
        int firstCellIdx = 0;
        Cell current = cells[firstCellIdx];
        visitedCellsTable.Add(firstCellIdx, 0); // Add first cell to hashtable of visited cells
        Stack<Cell> cellStack = new Stack<Cell>();
        int visitedCells = 0;

        Cell next = LookupNeighbors(current.GetX(), current.GetY()); // returns one random unvisited neighboring cell, or an empty one

        while (!next.IsEmpty())
        {
            cellStack.Push(current);
            RemoveWalls(current, next);
            current = next;
            next = LookupNeighbors(current.GetX(), current.GetY()); // returns one random unvisited neighboring cell, or an empty one
            visitedCells++;

            // Backtracking
            while (next.IsEmpty() && cellStack.Count > 0)
            {
                current = cellStack.Pop();
                next = LookupNeighbors(current.GetX(), current.GetY());
            }

            // Yields after genYieldLimit cells to not have the whole application window "freeze" too long for large mazes
            if (visitedCells == 1 || visitedCells % genYieldLimitMaze == 0)
            {
                yield return null;
            }
        }
        StartCoroutine(SpawnPlayer()); // Spawning player character after maze has been created
    }

    // Helper method to find neighbor index
    private int GetNeighborIndex(int i, int j)
    {
        if (i < 0 || j < 0 || i > width - 1 || j > height - 1)
            return -1;

        return i + j * width;
    }

    /* 
     * Method to check which neighboring cells the current cell has, and if they are visited or not.
     * Returns one random unvisited cell, or an empty one if all neighbors are visited.
    */ 
    private Cell LookupNeighbors(int i, int j)
    {
        List<Cell> neighbors = new List<Cell>(); // List storing neighboring cells of another
        List<int> neighborIdxList = new List<int>(); // List storing neighbor indices
        Cell left = new Cell();
        Cell top = new Cell();
        Cell right = new Cell();
        Cell bot = new Cell();
        int neighborIdx;

        if (GetNeighborIndex(i - 1, j) != -1 && cells.Length > GetNeighborIndex(i - 1, j))
        {
            neighborIdx = GetNeighborIndex(i - 1, j);
            left = cells[neighborIdx];
            if (!visitedCellsTable.ContainsKey(neighborIdx))
            {
                neighbors.Add(left);
                neighborIdxList.Add(neighborIdx);
            }             
        }

        if (GetNeighborIndex(i, j + 1) != -1 && cells.Length > GetNeighborIndex(i, j + 1))
        {
            neighborIdx = GetNeighborIndex(i, j + 1);
            top = cells[neighborIdx];
            if (!visitedCellsTable.ContainsKey(neighborIdx))
            {
                neighbors.Add(top);
                neighborIdxList.Add(neighborIdx);
            }               
        }

        if (GetNeighborIndex(i + 1, j) != -1 && cells.Length > GetNeighborIndex(i + 1, j))
        {
            neighborIdx = GetNeighborIndex(i + 1, j);
            right = cells[neighborIdx];
            if (!visitedCellsTable.ContainsKey(neighborIdx))
            {
                neighbors.Add(right);
                neighborIdxList.Add(neighborIdx);
            }               
        }

        if (GetNeighborIndex(i, j - 1) != -1 && cells.Length > GetNeighborIndex(i, j - 1))
        {
            neighborIdx = GetNeighborIndex(i, j - 1);
            bot = cells[neighborIdx];
            if (!visitedCellsTable.ContainsKey(neighborIdx))
            {
                neighbors.Add(bot);
                neighborIdxList.Add(neighborIdx);
            }               
        }

        if (neighbors.Count > 0)
        {
            int rand = Random.Range(0, neighbors.Count);

            Cell[] nCells = neighbors.ToArray();
            int[] nIdxs = neighborIdxList.ToArray();

            visitedCellsTable.Add(nIdxs[rand], 0); // Save index of visited cell in Hashtable
            
            return nCells[rand]; // Return one random neighbor
        }
        else // If no neighbor is found, return empty cell
            return new Cell();
    }

    // Method to remove appropriate walls when visiting a "new" cell
    private void RemoveWalls(Cell curr, Cell next)
    {
        int x = next.GetX() - curr.GetX();
        int y = next.GetY() - curr.GetY();

        // right
        if (x == 1 && y == 0)
        {
            Destroy(curr.rightWall);
            Destroy(next.leftWall);
        }
        // up
        if (x == 0 && y == 1)
        {
            Destroy(curr.topWall);
            Destroy(next.botWall);
        }
        // left
        if (x == -1 && y == 0)
        {
            Destroy(curr.leftWall);
            Destroy(next.rightWall);
        }
        // down
        if (x == 0 && y == -1)
        {
            Destroy(curr.botWall);
            Destroy(next.topWall);
        }
    }

    // Method called by slider input to change maze width
    public void ChangeWidth(float inputWidth)
    {
        width = (int)inputWidth;
    }

    // Method called by slider input to change maze height
    public void ChangeHeight(float inputHeight)
    {
        height = (int)inputHeight;
    }

}
