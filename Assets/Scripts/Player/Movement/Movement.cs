using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public partial class Movement : MonoBehaviour
{
    [HideInInspector] public CharacterController controller { get; set; }
    public CapsuleCollider collider { get; set; }

    [Header("Movement Variables")]
    public float MaxSpeed = 320f * 0.0254f;
    public float MaxAirSpeed = 30f * 0.0254f;
    public float MaxAccelM = 10f;
    public float MaxAirAccelM = 30f;
    public float Friction = 6f;
    public float JumpPower = 268f * 0.0254f;
    public float Gravity = 600f * 0.0254f;

    public float Radius = 16.0f * 0.0254f;

    public float Height = 64.0f * 0.0254f;

    public float StepHeight = 18.0f * 0.0254f;

    public float GroundAngle { get; set; } = 45.0f;

    public Vector3 Velocity;

    [HideInInspector] public Vector3 wishDirection;

    [HideInInspector] public Vector3 WishVelocity;

    [HideInInspector] public bool IgnoreMove { get; set; }
    [HideInInspector] public bool IgnoreCam { get; set; }
    [HideInInspector] public bool OverrideVelocity { get; set; }

    Vector3 EyeAngles;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        collider = GetComponent<CapsuleCollider>();
    }

    public void BuildWishVelocity()
    {
        var rot = EyeAngles.WithX(0f).ToRotation();

        var analogMove = Vector3Extensions.AnalogMove;

        var wishDirection = analogMove.normalized.RotateVector(rot);
        wishDirection = wishDirection.WithY(0);

        WishVelocity = wishDirection * MaxSpeed;
    }

    public virtual void GetWishDirection()
    {
        if (IgnoreMove)
        {
            wishDirection = Vector3.zero;
            return;
        }

        var dir = Vector3Extensions.AnalogMove.normalized;

        wishDirection = dir.x * forwardDirection + dir.z * -rightDirection;
    }

    public Vector3 forwardDirection => Camera.main?.transform.forward.WithY(0).normalized ?? default;
    public Vector3 rightDirection => Camera.main?.transform.right.WithY(0).normalized ?? default;

    public virtual void Update()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        UpdateHeight();

        BuildWishVelocity();

        UpdateCamera();

        GetWishDirection();

        if (!OverrideVelocity)
            WalkMove();

        Move();
    }

    public virtual void UpdateHeight()
    {
        if (!controller.IsUnityNull())
        {
            controller.radius = Radius;
            controller.height = Height;
            controller.center = Vector3.up * Height / 2;
        }

        if (!collider.IsUnityNull())
        {
            collider.radius = Radius;
            collider.height = Height;
            collider.center = Vector3.up * Height / 2;
        }
    }

    public virtual void UpdateCamera()
    {
        if (Camera.main.IsUnityNull())
            return;

        UpdateEyeAngles();

        Camera.main.transform.position = CameraPosition();
        Camera.main.transform.rotation = CameraRotation();
    }

    private void UpdateEyeAngles()
    {
        var eyeAngles = EyeAngles;

        if (!IgnoreCam)
            eyeAngles += Vector3Extensions.AnalogLook * 1000 * Time.deltaTime;

        eyeAngles = eyeAngles.WithX(Mathf.Clamp(eyeAngles.x,-89f, 89f));

        EyeAngles = eyeAngles;
    }

    public virtual Vector3 CameraPosition()
    {
        return transform.position + transform.up * (Height - 0.05f);
    }

    public virtual Quaternion CameraRotation()
    {
        return EyeAngles.ToRotation() * transform.rotation;
    }

    public virtual void WalkMove()
    {
        ApplyHalfGravity();

        if (controller.isGrounded)
            GroundVelocity();
        else
            AirVelocity();

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            LaunchUpwards(JumpPower);
        }

        ApplyHalfGravity();
    }

    public void ApplyHalfGravity()
    {
        if (controller.isGrounded)
            return;

        Velocity += Vector3.down * Gravity / 2 * Time.deltaTime;
    }

    public virtual void GroundVelocity()
    {
        ApplyFriction(Friction);

        AddSpeed(MaxSpeed, MaxAccelM);
    }

    public virtual void AirVelocity()
    {
        AddSpeed(MaxAirSpeed, MaxAirAccelM);
    }

    public void AddSpeed(float maxSpeed, float maxAccel)
    {
        var currentSpeed = Vector3.Dot(Velocity, wishDirection);
        var addSpeed = Mathf.Clamp(maxSpeed - currentSpeed, 0, maxAccel * maxSpeed * Time.deltaTime);

        Velocity += addSpeed * wishDirection;
    }

    public void ApplyFriction(float friction)
    {
        var speed = Velocity.magnitude;

        float newspeed = 0f;

        float drop = speed * friction * Time.deltaTime;


        newspeed = speed - drop;
        if (newspeed < 0)
        {
            newspeed = 0;
        }

        if (speed > 0)
            newspeed /= speed;

        Velocity *= newspeed;
    }
}
