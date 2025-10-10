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

    TimeSince lastSound;
    public void SoundEvent(string soundPath)
    {
        if (lastSound < 0.2f)
            return;
        lastSound = 0;
        var soundEvent = Resources.Load<SoundEvent>(soundPath);
        if (soundEvent == null)
            return;

        soundEvent.Play(transform.position);
    }
}
