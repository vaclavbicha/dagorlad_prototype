using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{

    public enum LocationType { Defense, Attack, Resource }
    public enum LocationStatus { NONE, Free, Building, Built, Training, Upgrading }
    public enum LocationSelectionStatus { Unselected, Selected, Unavailable }
    public enum DefenseTypes { Tower1, Tower2, Tower3, Tower4 }

    public enum ResourceTypes { NONE, Supply, Wood, Gold, Time }
    public enum UpgradeTypes { NONE, Farm, Mill, Blacksmith, Quary }
    public enum UpgradeEffectTypes { NONE, Resource, Troops, Building, Spell }
    public enum UnitTypes { NONE, Baracks, Stables, Workshop, Airport }
    public enum UnitStatus { GoingToFlag, AttackGoingToFlag, IdleFlag, Attacking, Dead }
    public enum StatsTypes { MoveSpeed, Health, Armor, Attack, AttackSpeed }

    public static IEnumerator ExecuteWithDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}
