using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public string buildingName;
    public Sprite Icon;
    public Sprite Flag;
    public Utility.LocationType locationType;
    public Amount buildTime;
    public Amount[] cost;
    public GameObject loadingBarPrefab;
    public GameObject Rally_Point;
    public Utility.UnitTypes unitType;
}
