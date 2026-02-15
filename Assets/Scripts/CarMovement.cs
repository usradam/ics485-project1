using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] Rigidbody theRB;
    [SerializeField] float forwardAccel = 8f, reverseAccel = 4f, maxSpeed = 70f, turnStrength = 180, gravityForce = 10f, dragOnGround = 3f;

    private float speedInput, turnInput;

    private bool grounded;

    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float groundRayLength = 0.8f;
    [SerializeField] Transform groundRayPoint;


    // [SerializeField] Transform leftFrontWheel, rightFrontWheel;
    // [SerializeField] float maxWheelTurn = 25f;

    [SerializeField] ParticleSystem[] dustTrail;
    [SerializeField] float maxEmission = 25f;
    private float emissionRate;

    private bool isExploded = false;

    void Start()
    {
        theRB.transform.parent = null;
    }

    void Update()
    {
        // Stop accepting input if exploded
        if (isExploded)
        {
            return;
        }

        speedInput = 0f;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAccel * 1000f;
        }
        else if (Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAccel * 1000f;
        }
        turnInput = Input.GetAxis("Horizontal");

        if (grounded)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Input.GetAxis("Vertical"), 0f));
        }
        // leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, leftFrontWheel.localRotation.eulerAngles.z);

        // rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.localRotation.eulerAngles.x, turnInput * maxWheelTurn, rightFrontWheel.localRotation.eulerAngles.z);

        transform.position = theRB.transform.position;
    }

    private void FixedUpdate()
    {
        // Stop physics if exploded
        if (isExploded)
        {
            return;
        }

        grounded = false;
        RaycastHit hit;

        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        emissionRate = 0;

        if (grounded)
        {
            theRB.linearDamping = dragOnGround;

            if (Mathf.Abs(speedInput) > 0)
            {
                theRB.AddForce(transform.forward * speedInput);
                emissionRate = maxEmission;
            }
            
            if (theRB.linearVelocity.magnitude > maxSpeed)
            {
                theRB.linearVelocity = theRB.linearVelocity.normalized * maxSpeed;
            }
        }
        else
        {
            theRB.linearDamping = 0.1f;
            theRB.AddForce(Vector3.up * -gravityForce * 100);
        }

        foreach(ParticleSystem part in dustTrail)
        {
            var emissionModule = part.emission;
            emissionModule.rateOverTime = emissionRate;
        }
    }

    // Called from SphereCollisionDetector when explosion is triggered
    public void SetExploded()
    {
        isExploded = true;
        
        // Stop all dust trail particles
        if (dustTrail != null)
        {
            foreach (ParticleSystem part in dustTrail)
            {
                if (part != null)
                {
                    part.Stop();
                    var emissionModule = part.emission;
                    emissionModule.rateOverTime = 0;
                }
            }
        }
    }
}