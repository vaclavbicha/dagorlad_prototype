using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public enum LocationType {  Defense, Attack, Resource }
    public enum LocationStatus { NONE, Free, Selected, Building, Built }
    public enum DefenseTypes { Tower0, Tower1, Tower2, Tower3 }

    public List<MapLocation> Locations = new List<MapLocation>();
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}
