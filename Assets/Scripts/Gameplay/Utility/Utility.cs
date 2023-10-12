using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{

    public enum LocationType { Defense, Attack, Resource }
    public enum LocationStatus { NONE, Free, Building, Built, Training }
    public enum LocationSelectionStatus { Unselected, Selected, Unavailable }
    public enum DefenseTypes { Tower1, Tower2, Tower3, Tower4 }

    public enum ResourceTypes { Supply, Wood, Stone, Ore, Time }
    public enum UpgradeTypes { NONE, Farm, Mill, Blacksmith, Quary }
    public enum UnitTypes { NONE, Baracks, Stables, Workshop, Airport }
    public enum UnitStatus { GoingToFlag, AttackGoingToFlag, IdleFlag, Attacking }
    public enum StatsTypes { MoveSpeed, Health, Armor, Attack, AttackSpeed }

    public static IEnumerator ExecuteWithDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
