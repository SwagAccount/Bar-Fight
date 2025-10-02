using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobAndSway : MonoBehaviour
{
    PlayerMovement player;
    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }

    Vector3 smoothedVel;
    void LateUpdate()
    {
        transform.position = Camera.main.transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation, Camera.main.transform.rotation, 10 * Time.deltaTime);

        smoothedVel = Vector3.Lerp(smoothedVel, player.Velocity, 5 * Time.deltaTime);

        transform.position -= smoothedVel * 0.005f;

        transform.position += Camera.main.transform.up *  Mathf.Sin(Time.time * 2) * 0.005f;
    }
}
