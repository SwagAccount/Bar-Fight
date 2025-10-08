using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GenericEvent : MonoBehaviour
{
    public Action<AnimationEvent> EventListen;
    public void Event(AnimationEvent @event)
    {
        EventListen?.Invoke(@event);
    }
}
