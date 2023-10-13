using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    public Utility.StatsTypes type;
    [SerializeField]
    public float value;
    [SerializeField]
    public float valueMAX;

    public void SetMax()
    {
        valueMAX = value;
    }
}
