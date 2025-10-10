using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    public float ExplosionForce = 200;
    HealthComponent HealthComponent;
    public void Start()
    {
        HealthComponent = GetComponent<HealthComponent>();
    }

    public void Update()
    {
        if (HealthComponent.Health > 0)
            return;

        Destructed.SetActive(true);
        Destructed.transform.SetParent(null);
        Destructed.AddComponent<TimedDestroyComponent>().Duration = 20;

        foreach (Transform child in Destructed.transform)
        {
            if (!child.TryGetComponent<Rigidbody>(out var rb))
                continue;

            rb.AddForce((rb.worldCenterOfMass - transform.position).normalized * 200);
        }

        Destroy(gameObject);
    }
    public GameObject Destructed;
}
