using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    Camera camera;

    public float FOV = 110;

    public float FOVSpeed = 0.05f;

    public LayerMask mask;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (camera == null)
            return;

        var fov = FOV;

        if (Physics.Raycast(transform.position, transform.forward, out var ray, 1024, mask) && ray.collider.transform.root.TryGetComponent<ZoomObject>(out var zoom))
            fov *= zoom.ZoomPercent;

        var vel = 0f;

        camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, fov, ref vel, FOVSpeed);
    }
}
