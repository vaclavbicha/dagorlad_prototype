using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnTrigger : MonoBehaviour
{
    public bool LoggEvents = true;

    public delegate void EventDelegate(GameObject sender, Collider2D otherCollider);

    public Dictionary<string, EventDelegate> EnterTriggerEvents = new Dictionary<string, EventDelegate>();
    public Dictionary<string, EventDelegate> StayTriggerEvents = new Dictionary<string, EventDelegate>();
    public Dictionary<string, EventDelegate> ExitTriggerEvents = new Dictionary<string, EventDelegate>();

    public void AddEvent(string TriggerType, string tag, EventDelegate @delegate)
    {
        //Debug.Log($"event {TriggerType} + {tag}");
        switch (TriggerType)
        {
            case "Enter":
                if (!EnterTriggerEvents.ContainsKey(tag))
                {
                    EnterTriggerEvents.Add(tag, @delegate);
                }
                else
                {
                    EnterTriggerEvents[tag] += @delegate;
                }
                break;

            case "Stay":
                if (!StayTriggerEvents.ContainsKey(tag))
                {
                    StayTriggerEvents.Add(tag, @delegate);
                }
                else
                {
                    StayTriggerEvents[tag] += @delegate;
                }
                break;

            case "Exit":
                if (!ExitTriggerEvents.ContainsKey(tag))
                {
                    ExitTriggerEvents.Add(tag, @delegate);
                }
                else
                {
                    ExitTriggerEvents[tag] += @delegate;
                }
                break;
        }
    }
    public void RemoveEvent(string TriggerType, string tag, EventDelegate @delegate)
    {
        switch (TriggerType)
        {
            case "Enter":
                if (EnterTriggerEvents.ContainsKey(tag))
                {
                    if (EnterTriggerEvents[tag].GetInvocationList().ToList().Contains(@delegate))
                    {
                        if (EnterTriggerEvents[tag].GetInvocationList().Length <= 1) EnterTriggerEvents.Remove(tag);
                        else EnterTriggerEvents[tag] -= @delegate;
                    }
                    else Debug.LogError("DELEGATE TO BE REMOVED NOT FOUND");
                }
                else
                {
                    Debug.LogError("YOU TRIED TO REMOVE ON Trigger EVENT THAT DOESN'T EXIST");
                }
                break;

            case "Stay":
                if (StayTriggerEvents.ContainsKey(tag))
                {
                    if (StayTriggerEvents[tag].GetInvocationList().ToList().Contains(@delegate))
                    {
                        if (StayTriggerEvents[tag].GetInvocationList().Length <= 1) StayTriggerEvents.Remove(tag);
                        else StayTriggerEvents[tag] -= @delegate;
                    }
                    else Debug.LogError("DELEGATE TO BE REMOVED NOT FOUND");
                }
                else
                {
                    Debug.LogError("YOU TRIED TO REMOVE ON Trigger EVENT THAT DOESN'T EXIST");
                }
                break;

            case "Exit":
                if (ExitTriggerEvents.ContainsKey(tag))
                {
                    if (ExitTriggerEvents[tag].GetInvocationList().ToList().Contains(@delegate))
                    {
                        if (ExitTriggerEvents[tag].GetInvocationList().Length <= 1) ExitTriggerEvents.Remove(tag);
                        else ExitTriggerEvents[tag] -= @delegate;
                    }
                    else Debug.LogError("DELEGATE TO BE REMOVED NOT FOUND");
                }
                else
                {
                    Debug.LogError("YOU TRIED TO REMOVE ON Trigger EVENT THAT DOESN'T EXIST");
                }
                break;
        }
    }
    public void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (EnterTriggerEvents.TryGetValue(otherCollider.gameObject.tag, out EventDelegate @event))
        {
            if (LoggEvents)
            {
                string eventsnames = "=";
                for (int i = 0; i < @event.GetInvocationList().Length; i++)
                {
                    eventsnames += " " + @event.GetInvocationList()[i].Method.Name;
                }
                Debug.Log($"{name} OnTriggerEnter : {otherCollider.gameObject.name} tag[{otherCollider.gameObject.tag}] -- [{@event.GetInvocationList().Length}] events {eventsnames}");
            }
            @event.Invoke(gameObject, otherCollider);
        }
    }
    public void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (StayTriggerEvents.TryGetValue(otherCollider.gameObject.tag, out EventDelegate @event))
        {
            if (LoggEvents)
            {
                string eventsnames = "=";
                for (int i = 0; i < @event.GetInvocationList().Length; i++)
                {
                    eventsnames += " " + @event.GetInvocationList()[i].Method.Name;
                }
                Debug.Log($"{name} OnTriggerEnter : {otherCollider.gameObject.name} tag[{otherCollider.gameObject.tag}] -- [{@event.GetInvocationList().Length}] events {eventsnames}");
            }
            @event.Invoke(gameObject, otherCollider);
        }
    }
    public void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (ExitTriggerEvents.TryGetValue(otherCollider.gameObject.tag, out EventDelegate @event))
        {
            if (LoggEvents)
            {
                string eventsnames = "=";
                for (int i = 0; i < @event.GetInvocationList().Length; i++)
                {
                    eventsnames += " " + @event.GetInvocationList()[i].Method.Name;
                }
                Debug.Log($"{name} OnTriggerEnter : {otherCollider.gameObject.name} tag[{otherCollider.gameObject.tag}] -- [{@event.GetInvocationList().Length}] events {eventsnames}");
            }
            @event.Invoke(gameObject, otherCollider);
        }
    }
}