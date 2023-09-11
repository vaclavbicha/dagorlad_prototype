using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> Bases = new List<GameObject>();
    public List<MapLocation> ALL_Locations = new List<MapLocation>();

    public List<GameObject> Buildings = new List<GameObject>();
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
    private void Start()
    {
        for(int i = 0; i < Bases.Count; i ++)
        {
            var locations = Bases[i].GetComponentsInChildren<MapLocation>();
            foreach(var location in locations)
            {
                location.baseID = i;
            }
            ALL_Locations.AddRange(locations);
        }
    }

    public void SpawnBuilding(string building, MapLocation location)
    {
        var buildingPrefab = Buildings.Find(x => x.name == building);
        if (buildingPrefab) location.SpawnBuilding(buildingPrefab);
        else UIManager.Instance.DialogWindow("Building Prefab not found");
    }
}
