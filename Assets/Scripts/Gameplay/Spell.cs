using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spell : MonoBehaviour
{
    MoveTo moveTo;
    OnTrigger onTrigger;
    public uint delay = 1;
    public uint duration = 0;
    public bool hasStarted = false;
    public bool isBuff = false;
    public string AnimationTrigger;

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
        onTrigger.AddEvent("Enter", "Player", (sender, collider) => {
            if (unitsInRage.Find(x => x.name == collider.name) == null)
            {
                unitsInRage.Add(collider.gameObject);
                Effect(collider.gameObject);
            }
        });
        onTrigger.AddEvent("Exit", "Player", (sender, collider) => {
            if (unitsInRage.Find(x => x.name == collider.name) != null)
            {
                unitsInRage.Remove(collider.gameObject);
                Effect(collider.gameObject, true);
            }
        });
        On_SpellStart += (spell) => { GameManager.Instance.GetComponent<AudioManager>().Play(spell.name.Replace("(Clone)","") + "_On_SpellStart"); };
        if (gameObject.name.Contains("Reveal"))
        {
            On_SpellStart += (spell) => { spell.transform.GetChild(0).gameObject.SetActive(true); };
        }
    }
    public void SpellStart()
    {
        hasStarted = true;
        StartCoroutine(ApplyEffect());
    }
    public void Effect(GameObject unit, bool undo = false)
    {
        if(duration != 0 && hasStarted)
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
        Camera.main.GetComponent<Animator>().SetTrigger(AnimationTrigger);
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
        for(int i = 0; i < duration; i++)
        {
            yield return new WaitForSeconds(1);
            foreach(var x in affectedStatsAllies)
            {
                if(x.type == Utility.StatsTypes.Health && x.value > 0)
                {
                    foreach(var y in unitsInRage)
                    {
                        if (y.tag == "Enemy")
                        {
                            //y.GetComponent<StatsManager>().UpdateStats(affectedStatsEnemies, true);
                        }
                        else
                        {
                            y.GetComponent<StatsManager>().Heal(x.value);
                        }
                    }
                }
            }
        }
        On_SpellEnd?.Invoke(gameObject);
    }
    public void OnDestroy()
    {
        foreach (var x in unitsInRage)
        {
            if (duration != 0 && hasStarted && isBuff)
            {
                if (x.tag == "Enemy")
                {
                    x.GetComponent<StatsManager>().UpdateStats(affectedStatsEnemies, true);
                }
                else
                {
                    x.GetComponent<StatsManager>().UpdateStats(affectedStatsAllies, true);
                }
            }
        }
    }
}
