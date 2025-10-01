using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pole : MonoBehaviour
{
    public float Length = 5;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * Length);
    }

    public Vector3 ClosestPoint(Vector3 point)
    {
        var lineStart = transform.position;
        var lineEnd = transform.position + transform.right * Length;

        var lineDir = lineEnd - lineStart;
        var lineLengthSq = lineDir.sqrMagnitude;

        if (lineLengthSq == 0)
            return lineStart;

        var t = Vector3.Dot(point - lineStart, lineDir) / lineLengthSq;

        t = Mathf.Clamp(t, 0f, 1f);

        return lineStart + t * lineDir;
    }
}
