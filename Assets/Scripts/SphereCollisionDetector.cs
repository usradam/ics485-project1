using UnityEngine;

public class SphereCollisionDetector : MonoBehaviour
{
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private ExplodeHandler explodeHandler;
    [SerializeField] private float explosionSpeedThreshold = 1f;
    
    private Rigidbody rb;
    private bool isExploded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if we hit a wall or obstacle (not the ground)
        bool isWallCollision = collision.gameObject.CompareTag("Wall") || 
                               collision.gameObject.CompareTag("Obstacle") ||
                               !collision.gameObject.CompareTag("Ground");
        
        // Explode if moving fast enough and hit a wall/obstacle
        if (!isExploded && rb.linearVelocity.magnitude > explosionSpeedThreshold && isWallCollision)
        {
            if (explodeHandler != null)
            {
                Vector3 velocity = rb.linearVelocity;
                explodeHandler.Explode(velocity);
                isExploded = true;
                
                // Stop the sphere immediately
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                
                // Tell CarMovement to stop
                if (carMovement != null)
                {
                    carMovement.SetExploded();
                }
            }
        }
    }
}
