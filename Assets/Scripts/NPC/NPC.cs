using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class NPC : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform target;
    public float rotationSpeed = 5f;

    [Header("References")]
    public Animator Animator;
    private Rigidbody rb;
    private NavMeshPath path;
    private Vector3 lastAnimatorPos;

    [Header("Punch")]
    public GameObject PunchPoint;
    public float PunchRadius = 1;
    public float PunchRange = 1.5f;

    GenericEvent genericEvent;

    float seed;
    void Start()
    {
        seed = Random.Range(0, 1000);

        genericEvent = Animator.GetComponent<GenericEvent>();
        genericEvent.EventListen += EventListen;

        rb = GetComponent<Rigidbody>();
        if (Animator == null)
            Animator = GetComponent<Animator>();

        path = new NavMeshPath();
        lastAnimatorPos = Animator.transform.localPosition;
    }

    public void EventListen(AnimationEvent @event)
    {
        if (@event.stringParameter == "punch")
            Punch();
    }

    void Punch()
    {
        var colliders = Physics.OverlapSphere(PunchPoint.transform.position, PunchRadius);
        foreach(var collider in colliders)
        {
            if (!collider.TryGetComponent<Player>(out var player ))
                return;

            player.Health -= 35 ;

            break;
        }
    }

    void Update()
    {
        if (target == null)
            return;

        if (!NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path))
            return;

        if (path.corners.Length < 2)
            return;

        Vector3 toNext = (path.corners[1] - transform.position).WithY(0);
        Vector3 targetDir = toNext.normalized;

        if (targetDir.sqrMagnitude > 0.001f)
        {
            Quaternion lookRot = Quaternion.LookRotation(targetDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
        }

        var dis = transform.position.DistanceTo(target.position);

        Animator.SetBool("Punch", dis < PunchRange);

        ApplyRootMotionVelocity();

        UpdateDirectionValue();
    }

    void ApplyRootMotionVelocity()
    {
        Vector3 delta = Animator.transform.localPosition - lastAnimatorPos;
        Vector3 velocity = delta / Time.deltaTime;

        rb.velocity = transform.rotation * velocity.WithY(rb.velocity.y);

        Animator.transform.localPosition = Vector3.zero;
        lastAnimatorPos = Vector3.zero;
    }

    float dir = 0;
    void UpdateDirectionValue()
    {
        float perlin = (Mathf.PerlinNoise1D((Time.time + seed) * 0.2f) * 2) - 1;

        var dis = transform.position.DistanceTo(target.position);

        var targetDir = 0f;

        if (perlin > 0.2f && dis > 5)
            targetDir = 1;
        if ( perlin < -0.2f && dis > 5 )
            targetDir = -1;

        dir = Mathf.Lerp(dir, targetDir, 5 * Time.deltaTime);

        Animator.SetFloat("direction", dir);

    }
}
