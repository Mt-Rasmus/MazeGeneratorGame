
using UnityEngine;

/*
 * Class for a user input controlled player character
 */

public class Player : MonoBehaviour
{
    private float speed = 3.0f;
    private Vector3 moveChange; // Direction the player moves in
    private Rigidbody2D rigidBody;
    private Animator animator;
    private ScoreChanger scoreObject;
    [SerializeField]
    private GameObject particlePrefab;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private AudioClip fireWorks, spawnSound, coinSound;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        scoreObject = GameObject.FindGameObjectWithTag("ScoreMaster").GetComponent<ScoreChanger>();
        SpawnEffect();
        // Playing "spawning" sound effects for the player character
        audioSource.PlayOneShot(fireWorks);
        audioSource.PlayOneShot(spawnSound);
        
    }

    // Update is called once per frame
    void Update()
    {
        moveChange = Vector3.zero;
        // Updating character position based off user input
        // Using GetAxisRaw to get user input as raw values, i.e. without acceleration, just binary input from arrow keys.
        moveChange.x = Input.GetAxisRaw("Horizontal");
        moveChange.y = Input.GetAxisRaw("Vertical");

        if (moveChange != Vector3.zero) // Character moving
        {
            MovePlayer();
            animator.SetFloat("walkX", moveChange.x);
            animator.SetFloat("walkY", moveChange.y);
            animator.SetBool("moving", true);
        }
        else
            animator.SetBool("moving", false);       
    }

    // Method to move the player according to user input, time and speed
    private void MovePlayer() 
    {
        rigidBody.MovePosition(
                transform.position + moveChange * speed * Time.deltaTime
            );
    }

    // Method to invoke a particle system effect upon spawning player
    private void SpawnEffect()
    {
        Vector3 particlePos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject particles = Instantiate(particlePrefab, particlePos, particlePrefab.transform.rotation);
        Destroy(particles, 1.75f);
    }

    // Method to handle collision, and "collecting" scoins
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("coin"))
        {
            scoreObject.UpdateScore(); // Update score
            scoreObject.UpdateScoreBoard(); // Update scoreboards
            audioSource.PlayOneShot(coinSound, 0.6f); // Play coin sound effect upon "collecting coin"
            Destroy(collision.gameObject); // Destroy coin when it's touched by the player character
        }
    }
}
