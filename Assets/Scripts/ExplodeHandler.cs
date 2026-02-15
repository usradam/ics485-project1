using UnityEngine;

public class ExplodeHandler : MonoBehaviour
{
    [SerializeField] GameObject intactCarMesh;
    [SerializeField] GameObject[] carParts;
    [SerializeField] GameObject[] wheelMeshObjects;
    [SerializeField] float explosionUpwardForce = 200f;
    [SerializeField] float explosionForwardMultiplier = 10f;
    [SerializeField] float explosionTorque = 50f;
    
    private Rigidbody[] partRigidbodies;
    private bool hasExploded = false;

    private void Awake()
    {
        // Cache rigidbodies from car parts
        if (carParts != null && carParts.Length > 0)
        {
            partRigidbodies = new Rigidbody[carParts.Length];
            for (int i = 0; i < carParts.Length; i++)
            {
                if (carParts[i] != null)
                {
                    partRigidbodies[i] = carParts[i].GetComponent<Rigidbody>();
                }
            }
        }
        else
        {
            // Auto-detect: Get all child rigidbodies
            partRigidbodies = GetComponentsInChildren<Rigidbody>(true);
        }
    }

    public void Explode(Vector3 externalForce)
    {
        if (hasExploded) return;
        hasExploded = true;

        // Hide the intact car mesh
        if (intactCarMesh != null)
        {
            intactCarMesh.SetActive(false);
        }
        
        // Also hide wheel mesh containers (they'll be replaced by exploding parts)
        if (wheelMeshObjects != null)
        {
            foreach (GameObject wheelMesh in wheelMeshObjects)
            {
                if (wheelMesh != null)
                {
                    wheelMesh.SetActive(false);
                }
            }
        }

        // Explode all car parts
        if (partRigidbodies != null)
        {
            foreach (Rigidbody rb in partRigidbodies)
            {
                if (rb == null) continue;

                // Unparent so parts can fly independently
                rb.transform.parent = null;
                
                // Enable physics
                rb.gameObject.SetActive(true);
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                
                // Enable collider if present
                MeshCollider meshCollider = rb.GetComponent<MeshCollider>();
                if (meshCollider != null)
                {
                    meshCollider.enabled = true;
                }
                
                // Apply explosion forces
                rb.AddForce(Vector3.up * explosionUpwardForce + externalForce * explosionForwardMultiplier, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * explosionTorque, ForceMode.Impulse);
            }
        }
    }

    public bool IsExploded()
    {
        return hasExploded;
    }
}
