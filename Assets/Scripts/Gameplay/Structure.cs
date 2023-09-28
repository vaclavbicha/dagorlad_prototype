using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public string buildingName;
    public Sprite Icon;
    public Sprite Flag;
    public Utility.LocationType locationType;
    public Utility.UnitTypes unitType;
    public Amount buildTime;
    public Amount[] cost;
    public GameObject loadingBarPrefab;
    public GameObject Rally_Point;
    public bool isAttackPoint = false;

    public void Update()
    {
        if (Rally_Point) Rally_Point.GetComponent<SpriteRenderer>().color = isAttackPoint ? Color.red : Color.white;
    }
}
