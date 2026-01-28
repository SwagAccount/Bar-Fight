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
        transform.SetParent(null);
    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.root.TryGetComponent<HealthComponent>(out var hc) && lastThrow < 10)
            hc.Health -= rb.velocity.magnitude * Damage;

        if (HealthComponent != null)
            HealthComponent.Health -= rb.velocity.magnitude * Damage;

        var point = GetAverageContactPoint(collision);

        Surface.DoImpact(collision.transform.gameObject, point, Mathf.Clamp01(rb.velocity.magnitude-1), forcePlay: lastThrow < 10);
        Surface.DoImpact(gameObject, point, Mathf.Clamp01(rb.velocity.magnitude-1), forcePlay: lastThrow < 10);
    }

    Vector3 GetAverageContactPoint(Collision collision)
    {
        if (collision.contactCount == 0)
            return Vector3.zero;

        Vector3 sum = Vector3.zero;

        foreach (var contact in collision.contacts)
            sum += contact.point;

        return sum / collision.contactCount;
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
