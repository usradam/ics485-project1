using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Vector3 pointA;
    [SerializeField] Vector3 pointB;
    [SerializeField] float speed = 2f;
    [SerializeField] bool useStartPositionAsPointA = true;
    [SerializeField] AudioClip hitSound;
    [SerializeField] float hitSoundVolume = 1f;
    
    private bool movingToB = true;

    void Start()
    {
        // Use the enemy's starting position as Point A if enabled
        if (useStartPositionAsPointA)
        {
            pointA = transform.position;
        }
    }

    void Update()
    {
        // Move between two points
        Vector3 target = movingToB ? pointB : pointA;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        // Switch direction when reaching destination
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            movingToB = !movingToB;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if hit by the player (sphere/car)
        if (collision.gameObject.CompareTag("Player"))
        {
            // Play hit sound at this position
            if (hitSound != null)
            {
                AudioSource.PlayClipAtPoint(hitSound, transform.position, hitSoundVolume);
            }
            
            // Notify game manager
            MainMenu mainMenu = FindObjectOfType<MainMenu>();
            if (mainMenu != null)
            {
                mainMenu.OnEnemyKilled();
            }
            
            // Destroy this enemy
            Destroy(gameObject);
        }
    }
}
