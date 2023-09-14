using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utility
{

    public enum LocationType { Defense, Attack, Resource }
    public enum LocationStatus { NONE, Free, Building, Built }
    public enum LocationSelectionStatus { Unselected, Selected, Unavailable }
    public enum DefenseTypes { Tower0, Tower1, Tower2, Tower3 }
    public enum ResourceTypes { Wheat, Wood, Stone, Ore }
}
