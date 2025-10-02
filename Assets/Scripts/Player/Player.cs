using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator Animator;

    TimeSince TimeSincePunch;

    public float PunchTime = 0.4f;
    public float PunchDamage = 20;
    public float PunchForce = 100;
    public float PunchRange = 4;

    PlayerMovement Movement;
    void Start()
    {
        Movement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        if (Movement.MoveMode != PlayerMovement.MoveModes.Swing)
            Attacking();
    }

    void Attacking()
    {
        if (Input.GetButtonDown("Fire1") && TimeSincePunch > PunchTime)
            Punch(true);

        else if (Input.GetButtonDown("Fire2") && TimeSincePunch > PunchTime)
            Punch(false);
    }

    void Punch(bool leftHand)
    {
        TimeSincePunch = 0;
        Animator.SetBool(!leftHand ? "PunchRight" : "PunchLeft", true);

        RaycastHit ray;

        if (!Physics.SphereCast(Camera.main.transform.position, 0.5f, Camera.main.transform.forward, out ray, PunchRange))
            return;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var piorityRay, PunchRange))
            ray = piorityRay;

        if (ray.collider.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForceAtPosition(Camera.main.transform.forward * PunchForce + Movement.Velocity * 50, ray.point);
        }
    }
}
