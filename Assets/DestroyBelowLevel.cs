using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBelowLevel : MonoBehaviour
{
    public float Level = -20;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > Level)
            return;

        Destroy(gameObject);
    }
}
