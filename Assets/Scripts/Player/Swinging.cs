using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public sealed class SwingMovement : MonoBehaviour
{
	public Movement Movement;
	public bool Swinging;

	public float Friction = 3;
	public float Length = 1f;

    Vector3 vaultPosition = Vector3.zero;
	public Pole poleObject;
	float vaultDistance;

	Vector3 playerEnd => transform.position + Vector3.up * Movement.Height;
	void Update()
	{
		if ( Swinging )
			Swing();
		else
			Idle();
	}

    void Swing()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Swinging = false;
            Movement.OverrideVelocity = false;
            return;
        }

        Vector3 poleForwardPos = poleObject.transform.position + poleObject.transform.forward;

        var directionToPlayer = playerEnd - poleForwardPos;
        var distance = directionToPlayer.magnitude;

        if (distance < Length + 0.5f)
            return;

        Movement.Velocity += -directionToPlayer.normalized * (distance - Length) * Time.deltaTime;

        Vector3 targetPlayerEnd = poleForwardPos + directionToPlayer.normalized * Length;

        transform.position = targetPlayerEnd - Vector3.up * Movement.Height;
    }



    void Idle()
	{

	}
}
