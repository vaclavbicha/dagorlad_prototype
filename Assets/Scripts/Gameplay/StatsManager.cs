using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public string owner;
    public Stat[] stats;
    public TextMeshPro DisplayHP;
    MoveTo moveTo;

    public delegate void TargetEventDelegate(GameObject sender);
    public event TargetEventDelegate On_Death;

    public Stat GetStat(Utility.StatsTypes type)
    {
        foreach (var x in stats)
        {
            if (x.type == type) return x;
        }
        return null;
    }
    public bool TakeRawDamage(float value)
    {
        var health = GetStat(Utility.StatsTypes.Health);
        var armor = GetStat(Utility.StatsTypes.Armor);

        if (health != null)
        {
            if(armor != null)
            {
                health.value -= Mathf.Abs(value - armor.value);
            }
            else
            {
                health.value -= value;
            }
        }
        else Debug.Log("Couldn't find health stat of : " + name + " of " + owner);
        return health.value > 0 ? false : true;
    }
    private void Start()
    {
        DisplayHP = GetComponentInChildren<TextMeshPro>();
        moveTo = GetComponent<MoveTo>();

        foreach(var x in stats)
        {
            x.SetMax();
        }
    }
    public void Update()
    {
        foreach(var x in stats)
        {
            if (x.type == Utility.StatsTypes.Health)
            {
                if (x.value >= 0) DisplayHP.text = x.value.ToString();
                else
                {
                    DisplayHP.text = 0.ToString();
                    On_Death?.Invoke(gameObject);
                }
            }
            if(moveTo != null)
            {
                if (x.type == Utility.StatsTypes.MoveSpeed)
                {
                    moveTo.Speed = x.value;
                }
            }
        }
    }
    public void Heal(float value)
    {
        foreach(var x in stats)
        {
            if(x.type == Utility.StatsTypes.Health)
            {
                if(x.value <= x.valueMAX - value)
                {
                    x.value += value;
                }
            }
        }
    }
    public void UpdateStats(Stat[] inc, bool revert = false)
    {
        foreach(var x in inc)
        {
            foreach(var y in stats)
            {
                if(x.type == y.type)
                {
                    y.value += revert ? -x.value : x.value;
                    Debug.Log("NEW VALUE " + x.value);
                }
            }
        }
    }
}
