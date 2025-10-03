using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask PlayerMask;

    public Animator LeftArm;
    public Animator RightArm;

    public Throwable LeftThrowable;
    public GameObject LeftHand;
    public Throwable RightThrowable;
    public GameObject RightHand;

    public Vector3 ItemOffset;

    TimeSince TimeSinceLeftPunch;
    TimeSince TimeSinceRightPunch;

    public float PunchTime = 0.4f;
    public float PunchDamage = 20;
    public float PunchForce = 100;
    public float PunchRange = 4;

    public float ThrowForce = 500;

    public float SwingHandDistance = 0.5f;

    PlayerMovement Movement;
    Vector3 startArmPos;
    Quaternion startArmRot;
    void Start()
    {
        startArmPos = LeftArm.transform.localPosition;
        startArmRot = RightArm.transform.localRotation;
        Movement = GetComponent<PlayerMovement>();
    }
    void Update()
    {
        ThrowablePos();

        if (Movement.MoveMode == PlayerMovement.MoveModes.Swing)
        {
            if (LeftThrowable != null)
                Throw(LeftThrowable);

            if (RightThrowable != null)
                Throw(RightThrowable);

            return;
        }

        if (LeftThrowable == null)
            LeftArm.SetBool("Grab", false);

        if (RightThrowable == null)
            RightArm.SetBool("Grab", false);

        if (Input.GetButton("Use"))
        {
            Grabbing();
            return;
        }

        Attacking(true, TimeSinceLeftPunch, LeftThrowable);
        Attacking(false, TimeSinceRightPunch, RightThrowable);
    }

    private void LateUpdate()
    {
        HandPositioning();
    }

    void HandPositioning()
    {
        if (LeftThrowable != null)
        {
            LeftArm.SetBool("Grab", true);
            PositionHand(LeftArm.gameObject, LeftHand, LeftThrowable.LeftGrab.transform.position);
            RotateHand(LeftArm.gameObject, LeftHand, LeftThrowable.LeftGrab.transform.rotation);
        }

        if (RightThrowable != null)
        {
            RightArm.SetBool("Grab", true);
            PositionHand(RightArm.gameObject, RightHand, RightThrowable.RightGrab.transform.position);
            RotateHand(RightArm.gameObject, RightHand, RightThrowable.RightGrab.transform.rotation);
        }
    }

    void PositionHand(GameObject Arm, GameObject Hand, Vector3 target)
    {
        Vector3 handOffset = Hand.transform.position - Arm.transform.position;

        Arm.transform.position = target - handOffset;
    }

    void RotateHand(GameObject Arm, GameObject Hand, Quaternion targetRot)
    {
        Quaternion handLocalRot = Quaternion.Inverse(Arm.transform.rotation) * Hand.transform.rotation;

        Arm.transform.rotation = targetRot * Quaternion.Inverse(handLocalRot);
    }

    void ThrowablePos()
    {
        if (LeftThrowable != null)
        {
            LeftThrowable.transform.localPosition = LeftThrowable.transform.localPosition.LerpTo(ItemOffset.WithX(-ItemOffset.x), 5 * Time.deltaTime);
            LeftThrowable.transform.localRotation = Quaternion.Slerp(LeftThrowable.transform.localRotation, Quaternion.identity, 5 * Time.deltaTime);
        }

        if (RightThrowable != null)
        {
            RightThrowable.transform.localPosition = RightThrowable.transform.localPosition.LerpTo(ItemOffset, 5 * Time.deltaTime);
            RightThrowable.transform.localRotation = Quaternion.Slerp(RightThrowable.transform.localRotation, Quaternion.identity, 5 * Time.deltaTime);
        }
    }

    bool leftGrabbing;
    bool rightGrabbing;
    void Grabbing()
    {
        bool wasLeftG = leftGrabbing;
        bool wasRightG = rightGrabbing;
        leftGrabbing = false;
        rightGrabbing = false;

        if (Input.GetButton("Fire1") && LeftThrowable == null)
        {
            leftGrabbing = true;
            LeftArm.SetBool("Grab", true);
        }

        if (Input.GetButton("Fire2") && RightThrowable == null)
        {
            rightGrabbing = true;
            RightArm.SetBool("Grab", true);
        }

        if ((Input.GetButtonUp("Fire1") || Input.GetButtonUp("Fire2")) && wasLeftG && wasRightG)
        {
            Pickup(true, true);
        }
        if (Input.GetButtonUp("Fire1") && wasLeftG)
        {
            Pickup(true, false);
        }
        else if (Input.GetButtonUp("Fire2") && wasRightG)
        {
            Pickup(false, false);
        }
    }

    void Attacking(bool left, TimeSince timeSince, Throwable throwable)
    {
        if (throwable != null)
        {
            if (Input.GetButtonDown(left ? "Fire1" : "Fire2"))
            {
                Throw(throwable);
            }
            return;
        }

        if (Input.GetButtonDown(left ? "Fire1" : "Fire2") && timeSince > PunchTime)
            Punch(left);
    }

    void Punch(bool leftHand)
    {
        if (leftHand)
            TimeSinceLeftPunch = 0;
        else
            TimeSinceRightPunch = 0;

        var arm = leftHand ? LeftArm : RightArm;

        arm.SetBool("Punch", true);

        RaycastHit ray;

        if (!Physics.SphereCast(Camera.main.transform.position, 0.5f, Camera.main.transform.forward, out ray, PunchRange, PlayerMask))
            return;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var piorityRay, PunchRange, PlayerMask))
            ray = piorityRay;

        if (ray.collider.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForceAtPosition(Camera.main.transform.forward * PunchForce + Movement.Velocity * 50, ray.point);
        }
    }

    void Pickup(bool left, bool both)
    {
        var rayCast = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out var ray, PunchRange, PlayerMask);

        if (!rayCast)
            return;

        if (!ray.collider.transform.root.TryGetComponent<Throwable>(out var throwable))
            return;

        Pickup(throwable, left, both);
    }

    void Pickup(Throwable throwable, bool left, bool both)
    {
        throwable.transform.SetParent(LeftArm.transform.root);

        throwable.rb.isKinematic = true;

        if (both)
        {
            LeftThrowable = throwable;
            RightThrowable = throwable;
            return;
        }

        if (left)
            LeftThrowable = throwable;
        else
            RightThrowable = throwable;
    }

    void Throw(Throwable throwable)
    {
        throwable.rb.isKinematic = false;

        if (LeftThrowable == throwable)
        {
            throwable.rb.AddForce(Camera.main.transform.forward * ThrowForce);
            LeftThrowable = null;
        }
            
        if (RightThrowable == throwable)
        {
            throwable.rb.AddForce(Camera.main.transform.forward * ThrowForce);
            RightThrowable = null;
        }

        throwable.transform.SetParent(null);
    }
}
