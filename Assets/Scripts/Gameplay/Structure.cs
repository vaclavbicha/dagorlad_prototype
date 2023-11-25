using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour
{
    public string buildingName;
    public Sprite Icon;
    public Sprite scrollIcon;
    public Sprite Flag1;
    public Sprite Flag3;
    public Sprite InfoSprite;
    public Sprite buildingSprite;
    public Utility.LocationType locationType;
    public Utility.UnitTypes unitType;
    public Utility.UpgradeTypes upgradeType;
    public Amount buildTime;
    public Amount[] cost;
    public Amount production;
    public GameObject loadingBarPrefab;
    public GameObject Rally_Point;
    public bool isAttackPoint = false;
    public MapLocation mapLocation;

    public int level;
    public Amount[] costUpgrade0;
    public Amount[] costUpgrade1;
    private int maxLevel = 3;

    //public List<OurUnit> currentArmy = new List<OurUnit>();

    public void Update()
    {
        if (Rally_Point) Rally_Point.GetComponentInChildren<SpriteRenderer>().color = isAttackPoint ? Color.red : Color.white;
    }
    public Amount[] ReturnCostOfLevel()
    {
        switch (level)
        {
            case 0:
                return costUpgrade0;
            case 1:
                return costUpgrade1;
            case 2:
                break;
        }
        return null;
    }
    public bool UpgradeStructure()
    {
        if (level + 1 < maxLevel)
        {
            level++;
            ChangeSprite();
            return true;
        }
        else return false;
    }
    public void ChangeSprite()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("StructuresSprites/" + buildingName + "_" + level.ToString());
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
