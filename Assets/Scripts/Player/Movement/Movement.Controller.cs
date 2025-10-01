using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Movement : MonoBehaviour
{
    public virtual void LaunchUpwards(float amount)
    {
        Velocity = Velocity.WithY(amount);
    }

    void Move()
    {
        controller.Move(Velocity * Time.deltaTime);
    }
}
