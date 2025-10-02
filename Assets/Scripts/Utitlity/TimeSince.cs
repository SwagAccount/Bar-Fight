using UnityEngine;

[System.Serializable]
public struct TimeSince
{
    [SerializeField]
    private float _timeStamp;

    public static implicit operator float(TimeSince t) => Time.time - t._timeStamp;

    public static implicit operator TimeSince(float secondsAgo)
    {
        TimeSince ts = new TimeSince();
        ts._timeStamp = Time.time - secondsAgo;
        return ts;
    }

    public float Seconds => Time.time - _timeStamp;

    public void Reset() => _timeStamp = Time.time;

    public void SetSecondsAgo(float secondsAgo) => _timeStamp = Time.time - secondsAgo;

    public override string ToString() => Seconds.ToString("F3");
}


[System.Serializable]
public struct TimeSinceUnscaled
{
    [SerializeField]
    private float _timeStamp;

    public static implicit operator float(TimeSinceUnscaled t) => Time.unscaledTime - t._timeStamp;

    public static implicit operator TimeSinceUnscaled(float secondsAgo)
    {
        TimeSinceUnscaled ts = new TimeSinceUnscaled();
        ts._timeStamp = Time.unscaledTime - secondsAgo;
        return ts;
    }

    public float Seconds => Time.unscaledTime - _timeStamp;

    public void Reset() => _timeStamp = Time.unscaledTime;

    public void SetSecondsAgo(float secondsAgo) => _timeStamp = Time.unscaledTime - secondsAgo;

    public override string ToString() => Seconds.ToString("F3");
}