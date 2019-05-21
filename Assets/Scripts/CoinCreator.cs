
using System.Collections;
using UnityEngine;

/*
 * Class which minds creating coins on the cell grid
 * The coins can be "picked up" by the player character, increasing the score
 */


public class CoinCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject coinPrefab; 
    private ScoreChanger scoreChanger;
    private Vector2[] cellPositions; // Array to store all cell positions in the cellgrid
    private int maxCoins;

    // Start is called before the first frame update
    void Start()
    {
        scoreChanger = GameObject.FindGameObjectWithTag("ScoreMaster").GetComponent<ScoreChanger>();
    }

    // Method that saves the coordinates of every cell in the created maze
    public void SetSpawnPoints(Vector2[] cellPts)
    {
        cellPositions = cellPts;
    }

    // Returns the coordinates of every cell in the created maze
    public Vector2[] GetSpawnPoints()
    {
        return cellPositions;
    }

    public void GenerateCoins()
    {
        maxCoins = cellPositions.Length / 4; // Nr of coins to spawn
        scoreChanger.SetMaxScore(maxCoins); // Call method to update max score on "Score board"
        Hashtable coinTable = new Hashtable(); // Hashtable to store cell indices
        int positionIndex;
        bool foundSlot = false;

        for (int i = 0; i < maxCoins; i++)
        {
            positionIndex = Random.Range(1, cellPositions.Length); // Pick random index of a cell position

            while (!foundSlot)
            {
                if (coinTable.ContainsKey(positionIndex))
                    positionIndex = Random.Range(1, cellPositions.Length); // If position is "taken", index
                else
                    foundSlot = true; // Empty slot found, exit loop
            }
            
            Vector2 coinPos = cellPositions[positionIndex];
            coinTable.Add(positionIndex, 0); // Add index to hashmap
            GameObject coin = Instantiate(coinPrefab, new Vector3(coinPos.x, coinPos.y, 0f), Quaternion.identity); // Spawn coin at the selected random position
            coin.transform.parent = gameObject.transform; // Parent coin object parent object to prevent scene clutter

            foundSlot = false;
        }
    }
}
