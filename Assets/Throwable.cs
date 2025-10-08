using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Throwable : MonoBehaviour
{
    public float Damage = 1;
    public GameObject LeftGrab;
    public GameObject RightGrab;
    public bool twoHands = false;
    public Rigidbody rb;
    HealthComponent HealthComponent;

    private void Start()
    {
        HealthComponent = GetComponent<HealthComponent>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.root.TryGetComponent<HealthComponent>(out var hc))
            hc.Health -= rb.velocity.magnitude * Damage * rb.mass;
        HealthComponent.Health -= rb.velocity.magnitude * Damage;
    }
}
