using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : Movement
{
    public float StandingHeight = 1.8f;

    public float CrouchHeight = 1;

    public float SlideHeight = 0.5f;

    public float RunSpeed = 190 * 0.0254f;

    public float SprintSpeed = 320 * 0.0254f;

    public float WalkSpeed = 90 * 0.0254f;

    public float CrouchSpeed = 90 * 0.0254f;

    public float SlideFriction = 2f;

    public float SlideBoost = 500f * 0.0254f;
    public MoveModes MoveMode;

    public enum MoveModes
    {
        Walk,
        Crouch,
        Slide
    }

    public override void Update()
    {
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
        }

        base.Update();
    }
    float heightVelocity;
    private void Walk()
    {
        if (MaxSpeed == SprintSpeed && controller.isGrounded && Input.GetButtonDown("Duck"))
        {
            MoveMode = MoveModes.Slide;
            AddSpeed(SlideBoost, 1000);
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
        if (MoveMode != MoveModes.Slide)
        {
            base.GroundVelocity();
            return;
        }

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
