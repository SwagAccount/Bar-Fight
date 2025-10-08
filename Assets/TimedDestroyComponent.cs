using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroyComponent : MonoBehaviour
{
    public float Duration;

    private TimeSince TimeSince;
    // Start is called before the first frame update
    void Start()
    {
        TimeSince = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (TimeSince < Duration)
            return;

        Destroy(gameObject);
    }
}
