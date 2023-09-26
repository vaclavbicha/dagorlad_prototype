using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    public bool LoggEvents = true;

    public delegate void EventDelegate(GameObject sender, Collision2D collision);

    public Dictionary<string, EventDelegate> EnterCollisionEvents = new Dictionary<string, EventDelegate>();
    public Dictionary<string, EventDelegate> StayCollisionEvents = new Dictionary<string, EventDelegate>();
    public Dictionary<string, EventDelegate> ExitCollisionEvents = new Dictionary<string, EventDelegate>();

    public void AddEvent(string CollisionType, string tag, EventDelegate @delegate)
    {
        switch (CollisionType)
        {
            case "Enter":
                if (!EnterCollisionEvents.ContainsKey(tag))
                {
                    EnterCollisionEvents.Add(tag, @delegate);
                }
                else
                {
                    EnterCollisionEvents[tag] += @delegate;
                }
                break;

            case "Stay":
                if (!StayCollisionEvents.ContainsKey(tag))
                {
                    StayCollisionEvents.Add(tag, @delegate);
                }
                else
                {
                    StayCollisionEvents[tag] += @delegate;
                }
                break;

            case "Exit":
                if (!ExitCollisionEvents.ContainsKey(tag))
                {
                    ExitCollisionEvents.Add(tag, @delegate);
                }
                else
                {
                    ExitCollisionEvents[tag] += @delegate;
                }
                break;
        }
    }
    public void RemoveEvent(string CollisionType, string tag, EventDelegate @delegate)
    {
        switch (CollisionType)
        {
            case "Enter":
                if (EnterCollisionEvents.ContainsKey(tag))
                {
                    if (EnterCollisionEvents[tag].GetInvocationList().ToList().Contains(@delegate))
                    {
                        if (EnterCollisionEvents[tag].GetInvocationList().Length <= 1) EnterCollisionEvents.Remove(tag);
                        else EnterCollisionEvents[tag] -= @delegate;
                    }
                    else Debug.LogError("DELEGATE TO BE REMOVED NOT FOUND");
                }
                else
                {
                    Debug.LogError("YOU TRIED TO REMOVE ON COLLISION EVENT THAT DOESN'T EXIST");
                }
                break;

            case "Stay":
                if (StayCollisionEvents.ContainsKey(tag))
                {
                    if (StayCollisionEvents[tag].GetInvocationList().ToList().Contains(@delegate))
                    {
                        if (StayCollisionEvents[tag].GetInvocationList().Length <= 1) StayCollisionEvents.Remove(tag);
                        else StayCollisionEvents[tag] -= @delegate;
                    }
                    else Debug.LogError("DELEGATE TO BE REMOVED NOT FOUND");
                }
                else
                {
                    Debug.LogError("YOU TRIED TO REMOVE ON COLLISION EVENT THAT DOESN'T EXIST");
                }
                break;

            case "Exit":
                if (ExitCollisionEvents.ContainsKey(tag))
                {
                    if (ExitCollisionEvents[tag].GetInvocationList().ToList().Contains(@delegate))
                    {
                        if (ExitCollisionEvents[tag].GetInvocationList().Length <= 1) ExitCollisionEvents.Remove(tag);
                        else ExitCollisionEvents[tag] -= @delegate;
                    }
                    else Debug.LogError("DELEGATE TO BE REMOVED NOT FOUND");
                }
                else
                {
                    Debug.LogError("YOU TRIED TO REMOVE ON COLLISION EVENT THAT DOESN'T EXIST");
                }
                break;
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (EnterCollisionEvents.TryGetValue(collision.collider.tag, out EventDelegate @event))
        {
            if (LoggEvents)
            {
                string eventsnames = "=";
                for (int i = 0; i < @event.GetInvocationList().Length; i++)
                {
                    eventsnames += " " + @event.GetInvocationList()[i].Method.Name;
                }
                Debug.Log($"{name} OnCollisionEnter : {collision.collider.name} tag[{collision.collider.tag}] -- [{@event.GetInvocationList().Length}] events {eventsnames}");
            }
            @event.Invoke(gameObject, collision);
        }
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        if (StayCollisionEvents.TryGetValue(collision.collider.tag, out EventDelegate @event))
        {
            if (LoggEvents)
            {
                string eventsnames = "=";
                for (int i = 0; i < @event.GetInvocationList().Length; i++)
                {
                    eventsnames += " " + @event.GetInvocationList()[i].Method.Name;
                }
                Debug.Log($"{name} OnCollisionEnter : {collision.collider.name} tag[{collision.collider.tag}] -- [{@event.GetInvocationList().Length}] events {eventsnames}");
            }
            @event.Invoke(gameObject, collision);
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        if (ExitCollisionEvents.TryGetValue(collision.collider.tag, out EventDelegate @event))
        {
            if (LoggEvents)
            {
                string eventsnames = "=";
                for (int i = 0; i < @event.GetInvocationList().Length; i++)
                {
                    eventsnames += " " + @event.GetInvocationList()[i].Method.Name;
                }
                Debug.Log($"{name} OnCollisionEnter : {collision.collider.name} tag[{collision.collider.tag}] -- [{@event.GetInvocationList().Length}] events {eventsnames}");
            }
            @event.Invoke(gameObject, collision);
        }
    }
}