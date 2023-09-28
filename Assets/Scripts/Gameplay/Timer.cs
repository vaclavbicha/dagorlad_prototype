using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public string name;

    public float timeStarted;
    public float timeFinish;
    public float duration;

    public bool ping;
    public float pingTimer;
    public float pingDuration;

    public delegate void EventDelegate(Timer timer);
    public event EventDelegate On_PingAction;

    public delegate void RemoveEventDelegate(Timer timer);
    public event RemoveEventDelegate On_Duration_End;

    public void AddTimer(string timername, float _duration, bool _ping, float _pingDuration = 1f)
    {
        timeStarted = Time.time;
        name = timername;
        duration = _duration;
        ping = _ping;
        pingDuration = _pingDuration;
        timeFinish = Time.time + duration;
        pingTimer = Time.time + pingDuration;
    }
    public void Update()
    {
        if (pingTimer <= Time.time)
        {
            pingTimer = Time.time + pingDuration;
            On_PingAction?.Invoke(this);
        }
        if (timeFinish <= Time.time)
        {
            On_Duration_End?.Invoke(this);
        }
    }
    public void SetRemaningTimeTo(float remaningDuration)
    {
        timeFinish = Time.time + remaningDuration;
    }
}
