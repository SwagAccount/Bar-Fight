using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableTrigger : MonoBehaviour
{
    public List<GameObject> Enable;
    public List<GameObject> Disable;
    public List<GameObject> Listen;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        foreach(var go in Enable)
        {
            if (go != null)
                go.SetActive(true);
        }

        foreach (var go in Disable)
        {
            if (go != null)
                go.SetActive(false);
        }
    }

    bool done;
    private void FixedUpdate()
    {
        if (done)
            return;

        if (Listen == null || Listen.Count <= 0)
            return;

        foreach (var go in Listen)
        {
            if (go != null)
                return;
        }

        foreach (var go in Enable)
        {
            if (go != null)
                go.SetActive(true);
        }

        foreach (var go in Disable)
        {
            if (go != null)
                go.SetActive(false);
        }

        done = true;
    }
}
