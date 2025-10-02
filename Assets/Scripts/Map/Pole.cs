using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pole : MonoBehaviour
{
    public float Length = 5;
    public float InteractDistance;

    private void Update()
    {
        var player = FindFirstObjectByType<PlayerMovement>();
        if (player.IsUnityNull())
            return;

        var dis = Vector3.Distance(player.transform.position, ClosestPoint(player.transform.position));

        if (dis < 5 && player.MoveMode != PlayerMovement.MoveModes.Slide && Input.GetButton("Use"))
            player.StartSwing(this);
            
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * Length);
    }

    public Vector3 ClosestPoint(Vector3 point)
    {
        return point.ClosestPointOnLine(transform.position, transform.position + transform.right * Length);
    }
}
