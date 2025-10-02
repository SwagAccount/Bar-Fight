using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RotateAgent : MonoBehaviour
{
    public Transform target; // Destination to reach
    private NavMeshPath path;

    void Start()
    {
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        if (!NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            return;

        if (path.corners.Length < 2)
            return;

        Vector3 targetDir = (path.corners[1] - transform.position).WithY(0).normalized;
        Quaternion lookRot = Quaternion.LookRotation(targetDir, Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 5f);
    }
}
