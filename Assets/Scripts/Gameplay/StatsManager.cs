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
        if (health != null) health.value -= value;
        else Debug.Log("Couldn't find health stat of : " + name + " of " + owner);
        return health.value > 0 ? false : true;
    }
    private void Start()
    {
        DisplayHP = GetComponentInChildren<TextMeshPro>();
        moveTo = GetComponent<MoveTo>();
    }
    public void Update()
    {
        foreach(var x in stats)
        {
            if (x.type == Utility.StatsTypes.Health)
            {
                DisplayHP.text = x.value.ToString();
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
}
