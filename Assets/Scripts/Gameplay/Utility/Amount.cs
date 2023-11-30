using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Amount
{
    [SerializeField]
    public Utility.ResourceTypes type;
    [SerializeField]
    public int value;

    public string GetValueText()
    {
        if(type == Utility.ResourceTypes.Time)
        {
            var minutes = Mathf.Floor(value / 60);
            var seconds = value - minutes * 60;
            var sectxt = seconds < 10 ? "0" + seconds.ToString() : seconds.ToString();
            return minutes.ToString() + ":" + sectxt;
        }
        else
        {
            return value.ToString();
        }
    }
}
