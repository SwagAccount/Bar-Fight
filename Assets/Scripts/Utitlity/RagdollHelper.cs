using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollHelper : MonoBehaviour
{
    public List<Rigidbody> RagdollRigidbodies = new();

    public bool startKinematic;

    void Start()
    {
        FindAllRigidbodies();
        SetKinematic(startKinematic);

    }

    public void SetKinematic(bool value)
    {
        foreach (var body in RagdollRigidbodies)
        {
            body.isKinematic = value;
        }
    }

    public void FindAllRigidbodies()
    {
        RagdollRigidbodies.Clear();
        RagdollRigidbodies.AddRange(GetComponentsInChildren<Rigidbody>(includeInactive: true));
    }
}
