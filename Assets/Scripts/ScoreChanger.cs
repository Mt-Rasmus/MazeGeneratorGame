
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/*
 * Class that keeps track of score and updates the "score board" canvas element
 * Also spawns particle effects and plays sound when all coins are collected
 * */

public class ScoreChanger : MonoBehaviour
{
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private GameObject fireworksPrefab;
    [SerializeField]
    private AudioClip fireWorksclip, applauseClip;
    private GameObject coinSign;
    private AudioSource audioSource;
    private ParticleSystem fireWorksParticelSystem;
    private Camera camera;
    private CoinCreator coinCreator;
    private int score = 0, maxScore;
    private int nrOfFireworks = 7;

    private void Start()
    {
        coinCreator = GameObject.FindGameObjectWithTag("CoinHolder").GetComponent<CoinCreator>();
        camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        coinSign = GameObject.FindGameObjectWithTag("CoinSign");
        fireWorksParticelSystem = fireworksPrefab.GetComponent<ParticleSystem>();
        audioSource = GetComponent<AudioSource>();
    }

    public void UpdateScore()
    {
        score++;
    }

    // Sets the maximum score based on number of coins in scene
    public void SetMaxScore(int maxCoins)
    {
        maxScore = maxCoins;
        UpdateScoreBoard();
    }

    /*
     * Method to update score board and call methods to start fireworks,
     * when all coins are collected
     */
    public void UpdateScoreBoard()
    {       
        scoreText.text = score + "/" + maxScore;

        if (score == maxScore)
        {
            SetOffFireworks();
            audioSource.PlayOneShot(applauseClip, 0.6f);
            scoreText.color = Color.green;
            coinSign.gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        }              
    }

    // Method to reset the score and scoreboard text upon creating new maze
    public void ResetScore()
    {
        score = 0;
        scoreText.text = "";
        scoreText.color = Color.white;
        coinSign.gameObject.transform.localScale = new Vector3(1, 1, 1);
    }

    // Method to instantiate particle systems resembling fire works timed one after the other
    private void SetOffFireworks()
    {
        Vector2[] posArray = coinCreator.GetSpawnPoints();
        int positionIndex;
        Vector2 randomPos;
        Vector3 spawnPos;
        float timeToNext;
        Color[] colorAray = {Color.red, Color.green, Color.yellow, Color.blue, Color.white, Color.cyan, Color.magenta};

        for (int i = 0; i < nrOfFireworks; i++)
        {
            positionIndex = Random.Range(1, posArray.Length);
            randomPos = posArray[positionIndex]; 
            spawnPos = new Vector3(randomPos.x, randomPos.y, 0); // Save a random cell position from the cell grid
            timeToNext = 0.5f * i; // Time of instantiation of next firework
            StartCoroutine(SpawnFireworks(timeToNext, spawnPos, colorAray[i]));
        }
    }

    // Instantiates one timed particle system (firework) + audioclip
    private IEnumerator SpawnFireworks(float timeToNext, Vector3 spawnPos, Color color)
    {
        yield return new WaitForSeconds(timeToNext);
        var main = fireWorksParticelSystem.main;
        main.startColor = color;
        // Adjusting the size of the fireworks according to camera size
        fireworksPrefab.transform.localScale = new Vector3(0.1f * camera.orthographicSize, 0.1f * camera.orthographicSize, 0.1f * camera.orthographicSize);
        GameObject fireWorksInstance = Instantiate(fireworksPrefab, spawnPos, fireworksPrefab.transform.rotation);
        audioSource.PlayOneShot(fireWorksclip, 0.6f);
        Destroy(fireWorksInstance, 1.75f);
    }
}
