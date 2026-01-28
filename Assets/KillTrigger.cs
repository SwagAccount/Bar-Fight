using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("twat");

        if (other.TryGetComponent<Player>(out var hc))
            hc.Health -= 100000;
    }
}
