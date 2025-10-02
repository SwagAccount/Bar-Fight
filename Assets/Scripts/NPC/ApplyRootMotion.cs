using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyRootMotion : MonoBehaviour
{
    public Animator Animator;
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void Update()
    {
        var lastPos = Animator.transform.localPosition;
        Animator.transform.localPosition = Vector3.zero;

        var velocity = lastPos / Time.deltaTime;

        rb.velocity = transform.rotation * velocity.WithY(rb.velocity.y);
    }
}
