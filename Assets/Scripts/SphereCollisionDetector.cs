using System;
using UnityEngine;

public class SphereCollisionDetector : MonoBehaviour
{
    [SerializeField] private CarMovement carMovement;
    [SerializeField] private ExplodeHandler explodeHandler;
    [SerializeField] private float explosionSpeedThreshold = 1f;
    
    private Rigidbody rb;
    private bool isExploded = false;

    public event Action<SphereCollisionDetector> OnPlayerCrashed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Don't explode if we hit the ground or an enemy
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Enemy"))
        {
            return;
        }
        
        // Explode if moving fast enough and didn't hit ground or enemy
        if (!isExploded && rb.linearVelocity.magnitude > explosionSpeedThreshold)
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
                OnPlayerCrashed?.Invoke(this);
            }
        }
    }
}
