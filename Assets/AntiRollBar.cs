using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiRollBar : MonoBehaviour
{
    [SerializeField] WheelCollider backLeft;
    [SerializeField] WheelCollider backRight;
    [SerializeField] float AntiRoll = 5000.0f;

    private Rigidbody carRigidbody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carRigidbody = GetComponent<Rigidbody>();
        carRigidbody.centerOfMass = new Vector3(0f, -0.5f, 0f);
    }

    void FixedUpdate()
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRight = 1.0f;

        bool groundedLeft = backLeft.GetGroundHit(out hit);
        if (groundedLeft)
        {
            travelLeft = (-backLeft.transform.InverseTransformPoint(hit.point).y - backLeft.radius) / backLeft.suspensionDistance;
        }
        bool groundedRight = backRight.GetGroundHit(out hit);
        if (groundedRight)
        {
            travelRight = (-backRight.transform.InverseTransformPoint(hit.point).y - backRight.radius) / backRight.suspensionDistance;
        }

        float antiRollForce = (travelLeft - travelRight) * AntiRoll;
        if (groundedLeft)
        {
            carRigidbody.AddForceAtPosition(backLeft.transform.up * -antiRollForce, backLeft.transform.position);
        }
        if (groundedRight)
        {
            carRigidbody.AddForceAtPosition(backRight.transform.up * antiRollForce, backRight.transform.position);
        }
    }
}
