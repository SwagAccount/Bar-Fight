using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerMovement : Movement
{
    public float StandingHeight = 1.8f;

    public float CrouchHeight = 1;

    public float SlideHeight = 0.5f;

    public float SwingHeight = 0.5f;

    public float RunSpeed = 190 * 0.0254f;

    public float SprintSpeed = 320 * 0.0254f;

    public float WalkSpeed = 90 * 0.0254f;

    public float CrouchSpeed = 90 * 0.0254f;

    public float SlideFriction = 2f;

    public float SlideBoost = 500f * 0.0254f;
    public MoveModes MoveMode;

    public Pole SwingPoint;

    public enum MoveModes
    {
        Walk,
        Crouch,
        Slide,
        Swing
    }

    private MoveModes lastMode;
    Vector3 lastPos;
    float speed;

    TimeSince lastStep;

    public SoundEvent StepSound;

    public override void Update()
    {
        speed = (transform.position - lastPos).magnitude / Time.deltaTime;
        lastPos = transform.position;

        if (speed > MathF.Min(WalkSpeed, CrouchSpeed) / 2 && controller.isGrounded && MoveMode != MoveModes.Slide)
        {
            var cadence = 1 / (1.865f + 0.213f * speed);

            if (lastStep > cadence)
            {
                lastStep = 0;
                Step();
            }
        }

        switch (MoveMode)
        {
            case MoveModes.Walk:
                Walk();
                break;
            case MoveModes.Crouch:
                Crouch();
                break;
            case MoveModes.Slide:
                Slide();
                break;
            case MoveModes.Swing:
                Swing();
                break;

        }

        lastMode = MoveMode;

        base.Update();
    }

    public void Step()
    {
        var volume = Mathf.Clamp01(speed / SprintSpeed);
        StepSound.Play(transform.position, volume, forcePlay: true);
    }


    float heightVelocity;

    public void StartSwing(Pole pole)
    {
        SwingPoint = pole;
        MoveMode = MoveModes.Swing;
    }

    private void Swing()
    {
        if (SwingPoint.IsUnityNull() || Input.GetButtonDown("Jump"))
        {
            MoveMode = MoveModes.Walk;
            return;
        }

        var dir = SwingDirection();

        Height = Mathf.SmoothDamp(Height, SwingHeight, ref heightVelocity, 0.1f);
    }

    private void Walk()
    {

        if (MaxSpeed == SprintSpeed && controller.isGrounded && Input.GetButtonDown("Duck"))
        {
            transform.position += Vector3.up * (Height - SlideHeight);

            var speedMult = Mathf.Clamp01(speed / SprintSpeed);

            Height = SlideHeight;
            MoveMode = MoveModes.Slide;
            AddSpeed(SlideBoost * speedMult, 1000);
            Slide();
            return;
        }

        if (Input.GetButtonDown("Duck"))
        {
            MoveMode = MoveModes.Crouch;

            Crouch();
            return;
        }
        
        Height = Mathf.SmoothDamp(Height, StandingHeight, ref heightVelocity, 0.1f);

        MaxSpeed = RunSpeed;

        if (Input.GetButton("Run"))
            MaxSpeed = SprintSpeed;

        if (Input.GetButton("Walk"))
            MaxSpeed = WalkSpeed;
    }

   

    private void Slide()
    {
        Height = Mathf.SmoothDamp(Height, SlideHeight, ref heightVelocity, 0.1f);
        if (Input.GetButton("Jump") || Velocity.WithY(0).magnitude < CrouchSpeed)
        {
            ExitSlide();
            return;
        }
        MaxSpeed = WalkSpeed;
    }

    private void ExitSlide()
    {
        if (StandCheck())
        {
            MoveMode = MoveModes.Walk;

            if (!controller.isGrounded)
            {
                transform.position -= Vector3.up * (StandingHeight - CrouchHeight);
                Height = StandingHeight;
            }

            Walk();
            return;
        }

        MoveMode = MoveModes.Crouch;
        Crouch();
    }

    Vector3 previousWish;

    public override void GroundVelocity()
    {
        switch (MoveMode)
        {
            case MoveModes.Slide:
                SlideVelocity();
                return;
            case MoveModes.Swing:
                SwingVelocity();
                return;
        }

        base.GroundVelocity();
    }

    public override void AirVelocity()
    {
        switch (MoveMode)
        {
            case MoveModes.Swing:
                SwingVelocity();
                return;
        }

        base.AirVelocity();
    }

    public Vector3 SwingDirection()
    {
        var dirPos =
            transform.position.ClosestPointOnLine(SwingPoint.transform.position, SwingPoint.transform.position + SwingPoint.transform.forward, false)
            .WithY(transform.position.y);

        var dir = Quaternion.LookRotation(SwingPoint.transform.position - dirPos) * Vector3.down;

        return dir.normalized;
    }

    public void SwingVelocity()
    {
        Velocity = Velocity.ProjectVelocity(SwingDirection());

        var toPivot = transform.position - SwingPoint.ClosestPoint(transform.position);
        float currentDistance = toPivot.magnitude;
        float desiredDistance = Height + 1.2f;

        float stretch = currentDistance - desiredDistance;

        if (MathF.Abs(stretch) > 0.001f)
        {
            Velocity += -toPivot.normalized * stretch * 10f;
        }
    }

    public void SlideVelocity()
    {
        ApplyFriction(SlideFriction);

        var wish = Velocity.WithY(0);

        var currentSpeed = Vector3.Dot(Velocity, wish.normalized * 2);
        var addSpeed = Mathf.Clamp(MaxSpeed - currentSpeed, 0, MaxAccelM * MaxSpeed * Time.deltaTime);

        Velocity += addSpeed * wish;
    }

    private void Crouch()
    {
        if (!Input.GetButton("Duck") && StandCheck())
        {
            MoveMode = MoveModes.Walk;

            if (!controller.isGrounded)
            {
                transform.position -= Vector3.up * (StandingHeight - CrouchHeight);
                Height = StandingHeight;
            }

            Walk();
            return;
        }

        Height = Mathf.SmoothDamp(Height, CrouchHeight, ref heightVelocity, 0.1f);

        MaxSpeed = CrouchSpeed;

    }

    public bool StandCheck()
    {
        return true;
        //var previousHeight = Height;
        //Height = StandingHeight;

        //if (!IsGrounded)
        //    WorldPosition -= Vector3.Up * (StandingHeight - CrouchHeight);

        //var result = !IsStuck();

        //if (!IsGrounded)
        //    WorldPosition += Vector3.Up * (StandingHeight - CrouchHeight);

        //Height = previousHeight;

        //return result;
    }


}
