using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public string buildingName;
    public Sprite Icon;
    public Sprite Flag;
    public Sprite InfoSprite;
    public Utility.LocationType locationType;
    public Utility.UnitTypes unitType;
    public Utility.UpgradeTypes upgradeType;
    public Amount buildTime;
    public Amount[] cost;
    public GameObject loadingBarPrefab;
    public GameObject Rally_Point;
    public bool isAttackPoint = false;
    public MapLocation mapLocation;

    //public List<OurUnit> currentArmy = new List<OurUnit>();

    public void Update()
    {
        if (Rally_Point) Rally_Point.GetComponentInChildren<SpriteRenderer>().color = isAttackPoint ? Color.red : Color.white;
    }
    //public void AttackEnemiesInRange()
    //{
    //    //Sorting + Target selection
    //    foreach(var x in currentArmy)
    //    {
    //        x.Attack(Rally_Point.GetComponent<Draggable>().EnemiesInRange[0]);
    //    }
    //}
    //public void StopAttackEnemiesInRange(GameObject enemy)
    //{
    //    //Disengage
    //    var enemyStats = enemy.GetComponent<StatsManager>();
    //    foreach (var x in currentArmy)
    //    {
    //        if(x.currentTarget == enemyStats)
    //        {
    //            x.StopAttack();
    //        }
    //    }
    //}
}
