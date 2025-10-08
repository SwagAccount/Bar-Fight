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

    public float Health = 100f;

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

    bool rightSwing;
    bool wasntSwing;
    bool swinging => Movement.MoveMode == PlayerMovement.MoveModes.Swing;
    void Update()
    {
        if (Health <= 0)
        {
            Camera.main.GetComponent<Rigidbody>().isKinematic = false;
            Destroy(gameObject);
            return;
        }

        ThrowablePos();

        if (swinging)
        {

            if (wasntSwing)
            {
                bool leftFree = LeftThrowable == null;
                bool rightFree = RightThrowable == null;

                if (leftFree && !rightFree)
                {
                    rightSwing = false;
                }
                else if (rightFree && !leftFree)
                {
                    rightSwing = true;
                }
                else
                {
                    rightSwing = false;
                }
            }

            if (RightThrowable != null && rightSwing)
            {
                Throw(RightThrowable);
            }
            if (LeftThrowable != null && !rightSwing)
            {
                Throw(LeftThrowable);
            }

            wasntSwing = false;

            return;
        }

        wasntSwing = true;

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

        var targetPosL = LeftArm.transform.parent.TransformPoint( startArmPos );
        var targetRotL = LeftArm.transform.parent.rotation * startArmRot;

        var targetPosR = targetPosL;
        var targetRotR = targetRotL;

        if (LeftThrowable != null)
        {
            LeftArm.SetBool("Grab", true);
            targetPosL = PositionHand(LeftArm.gameObject, LeftHand, LeftThrowable.LeftGrab.transform.position);
            targetRotL = RotateHand(LeftArm.gameObject, LeftHand, LeftThrowable.LeftGrab.transform.rotation);
        }

        if (RightThrowable != null)
        {
            RightArm.SetBool("Grab", true);
            targetPosR = PositionHand(RightArm.gameObject, RightHand, RightThrowable.RightGrab.transform.position);
            targetRotR = RotateHand(RightArm.gameObject, RightHand, RightThrowable.RightGrab.transform.rotation);
        }

        if (swinging && !rightSwing)
        {
            LeftArm.SetBool("Grab", true);
            targetPosL = PositionHand(LeftArm.gameObject, LeftHand, Movement.SwingPoint.ClosestPoint(transform.position));
            targetRotL = RotateHand(LeftArm.gameObject, LeftHand, LeftArm.transform.parent.rotation * quaternion.Euler(-90, 0, 0));
        }

        if (swinging && rightSwing)
        {
            RightArm.SetBool("Grab", true);
            targetPosR = PositionHand(RightArm.gameObject, RightHand, Movement.SwingPoint.ClosestPoint(transform.position));
            targetRotR = RotateHand(RightArm.gameObject, RightHand, LeftArm.transform.parent.rotation * quaternion.Euler(-90, 0, 0));
        }

        LeftArm.transform.position = targetPosL;
        LeftArm.transform.rotation = targetRotL;

        RightArm.transform.position = targetPosR;
        RightArm.transform.rotation = targetRotR;
    }

    Vector3 PositionHand(GameObject Arm, GameObject Hand, Vector3 target)
    {
        Vector3 handOffset = Hand.transform.position - Arm.transform.position;

        return target - handOffset;
    }

    Quaternion RotateHand(GameObject Arm, GameObject Hand, Quaternion targetRot)
    {
        Quaternion handLocalRot = Quaternion.Inverse(Arm.transform.rotation) * Hand.transform.rotation;

        return targetRot * Quaternion.Inverse(handLocalRot);
    }

    void ThrowablePos()
    {
        if (LeftThrowable != null)
        {
            LeftThrowable.transform.localPosition = LeftThrowable.transform.localPosition.LerpTo(ItemOffset.WithX(-ItemOffset.x), 5 * Time.deltaTime);
            LeftThrowable.transform.localRotation = Quaternion.identity;
        }

        if (RightThrowable != null)
        {
            RightThrowable.transform.localPosition = RightThrowable.transform.localPosition.LerpTo(ItemOffset, 5 * Time.deltaTime);
            RightThrowable.transform.localRotation = Quaternion.identity;
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
        if (!left == rightSwing && swinging)
            return;

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

        if (both && throwable.twoHands)
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
