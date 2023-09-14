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
}
