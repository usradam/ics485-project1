using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] Vector3 pointA;
    [SerializeField] Vector3 pointB;
    [SerializeField] float speed = 2f;
    [SerializeField] bool useStartPositionAsPointA = true;
    [SerializeField] AudioClip hitSound;
    [SerializeField] float hitSoundVolume = 10f;
    
    private bool movingToB = true;
    private Rigidbody rb;
    private bool isDead = false;
    private Vector3 lastPosition;
    private float stuckTimer = 0f;

    void Start()
    {
        // Use the enemy's starting position as Point A if enabled
        if (useStartPositionAsPointA)
        {
            pointA = transform.position;
        }
        
        // Get rigidbody and configure for collision without falling
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // Allow physics collisions
            rb.useGravity = false;  // Prevent falling
            rb.freezeRotation = true; // Prevent tumbling
            rb.mass = 1f; // Low mass so car doesn't bounce much
            rb.linearDamping = 2f; // Medium drag
            // Freeze Y position to keep enemy at same height
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        }
        
        lastPosition = transform.position;
    }

    void Update()
    {
        // Move between two points
        Vector3 target = movingToB ? pointB : pointA;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        // Use rigidbody to move for proper collision
        if (rb != null)
        {
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position = newPosition;
        }
        
        // Check if stuck on wall (not moving for 0.5 seconds)
        if (Vector3.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.deltaTime;
            if (stuckTimer > 0.5f)
            {
                // Stuck! Turn around
                movingToB = !movingToB;
                stuckTimer = 0f;
            }
        }
        else
        {
            stuckTimer = 0f;
            lastPosition = transform.position;
        }
        
        // Switch direction when reaching destination
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            movingToB = !movingToB;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Prevent multiple kills from the same enemy
        if (isDead) return;
        
        // Check if hit by the player (sphere/car)
        if (collision.gameObject.CompareTag("Player"))
        {
            isDead = true;
            
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
        else if (!collision.gameObject.CompareTag("Ground"))
        {
            // Hit a building/obstacle - bounce away and turn around
            if (rb != null && collision.contacts.Length > 0)
            {
                // Get the direction away from the collision
                Vector3 bounceDirection = collision.contacts[0].normal;
                bounceDirection.y = 0; // Keep it horizontal
                
                // Apply a push force away from the building
                rb.AddForce(bounceDirection * 5f, ForceMode.Impulse);
                
                // Turn around
                movingToB = !movingToB;
                stuckTimer = 0f;
            }
        }
    }
}
