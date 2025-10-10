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

    public TimeSince lastThrow;

    private void Start()
    {
        lastThrow = Time.time + 10000;
        HealthComponent = GetComponent<HealthComponent>();
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (lastThrow > 10)
            return;

        if (collision.transform.root.TryGetComponent<HealthComponent>(out var hc))
            hc.Health -= rb.velocity.magnitude * Damage;
        HealthComponent.Health -= rb.velocity.magnitude * Damage;
    }

    public void SetLayer( int layer)
    {
        SetLayer(gameObject, layer);
    }

    void SetLayer(GameObject gameObject, int layer)
    {
        gameObject.layer = layer;

        foreach (Transform child in gameObject.transform)
        {
            if (!child.gameObject.activeSelf)
                continue;

            if (child != null)
                SetLayer(child.gameObject, layer);
        }
    }
}
