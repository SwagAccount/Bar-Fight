using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class Vector3Extensions
{
    public static Vector3 WithX(this Vector3 v3, float value) => new Vector3(value, v3.y, v3.z);
    public static Vector3 WithY(this Vector3 v3, float value) => new Vector3(v3.x, value, v3.z);
    public static Vector3 WithZ(this Vector3 v3, float value) => new Vector3(v3.x, v3.y, value);

    public static Quaternion ToRotation(this Vector3 v3) => Quaternion.Euler(v3);

    public static Vector3 RotateVector(this Vector3 v, Quaternion q )
    {
        var u = new Vector3(q.x, q.y, q.z);
        float s = q.w;

        return 2.0f * Vector3.Dot(u, v) * u
             + (s * s - Vector3.Dot(u, u)) * v
             + 2.0f * s * Vector3.Cross(u, v);
    }

    public static Vector3 AnalogMove => new Vector3(Input.GetAxisRaw("Vertical"), 0, -Input.GetAxisRaw("Horizontal"));

    public static Vector3 AnalogLook => new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0);

    public static Vector3 ClosestPointOnLine(this Vector3 point, Vector3 lineStart, Vector3 lineEnd, bool clamp = true)
    {
        var lineDir = lineEnd - lineStart;
        var lineLengthSq = lineDir.sqrMagnitude;

        if (lineLengthSq == 0)
            return lineStart;

        var t = Vector3.Dot(point - lineStart, lineDir) / lineLengthSq;

        if (clamp)
            t = Mathf.Clamp(t, 0f, 1f);

        return lineStart + t * lineDir;
    }

    public static Vector3 ProjectVelocity(this Vector3 velocity, Vector3 direction)
    {
        Vector3 dirNormalized = direction.normalized;
        float scalar = Vector3.Dot(velocity, dirNormalized);
        return dirNormalized * scalar;
    }

}