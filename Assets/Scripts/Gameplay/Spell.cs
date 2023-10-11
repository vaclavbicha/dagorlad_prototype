using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    MoveTo moveTo;
    OnTrigger onTrigger;
    public uint delay = 1;
    public uint duration = 0;
    public Stat[] affectedStatsAllies;
    public Stat[] affectedStatsEnemies;

    public delegate void TargetEventDelegate(GameObject sender);
    public event TargetEventDelegate On_SpellStart;
    public event TargetEventDelegate On_SpellEnd;

    public List<GameObject> unitsInRage = new List<GameObject>();

    private void Start()
    {
        moveTo = GetComponent<MoveTo>();
        onTrigger = GetComponent<OnTrigger>();
        onTrigger.AddEvent("Enter", "Enemy", (sender, collider) => {
            if (unitsInRage.Find(x => x.name == collider.name) == null)
            {
                unitsInRage.Add(collider.gameObject);
                Effect(collider.gameObject);
            }
        });
        onTrigger.AddEvent("Exit", "Enemy", (sender, collider) => {
            if (unitsInRage.Find(x => x.name == collider.name) != null)
            {
                unitsInRage.Remove(collider.gameObject);
                Effect(collider.gameObject, true);
            }
        });
    }
    public void SpellStart()
    {
        StartCoroutine(ApplyEffect());
    }
    public void Effect(GameObject unit, bool undo = false)
    {
        if(duration != 0)
        {
            if (unit.tag == "Enemy")
            {
                unit.GetComponent<StatsManager>().UpdateStats(affectedStatsEnemies, undo);
            }
            else
            {
                unit.GetComponent<StatsManager>().UpdateStats(affectedStatsAllies, undo);
            }
        }
    }
    IEnumerator ApplyEffect()
    {
        On_SpellStart?.Invoke(gameObject);
        yield return new WaitForSeconds(delay);
        foreach (var x in unitsInRage)
        {
            if(x.tag == "Enemy")
            {
                x.GetComponent<StatsManager>().UpdateStats(affectedStatsEnemies);
            }
            else
            {
                x.GetComponent<StatsManager>().UpdateStats(affectedStatsAllies);
            }
        }
        On_SpellEnd?.Invoke(gameObject);
    }
}
