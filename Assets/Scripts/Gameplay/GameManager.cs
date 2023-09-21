using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> bases = new List<GameObject>();
    public List<MapLocation> ALL_Locations = new List<MapLocation>();

    public List<Structure> buildings = new List<Structure>();
    public List<Unit> units = new List<Unit>();
    //public Dictionary<string, Structure> buildings = new Dictionary<string, Structure>();

    public List<Sprite> resourceSprites = new List<Sprite>();
    public int[] startingAmounts = new[] { 500, 1000, 1500, 2000 };
    
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
        for(int i = 0; i < bases.Count; i ++)
        {
            var locations = bases[i].GetComponentsInChildren<MapLocation>();
            foreach(var location in locations)
            {
                location.baseID = i;
            }
            ALL_Locations.AddRange(locations);
        }
        resourceSprites.AddRange(Resources.LoadAll<Sprite>("ResourceSprites"));
        //foreach (GameObject i in Resources.LoadAll<GameObject>("Structures"))
        //{
        //    buildings.Add(i.name, i.GetComponent<Structure>());
        //}
        buildings.AddRange(Resources.LoadAll<Structure>("Structures"));
        units.AddRange(Resources.LoadAll<Unit>("Units"));
        UpdatePlayerResources(Player.Instance, startingAmounts);

        InstantiateBottomMenu();
    }

    private void UpdatePlayerResources(Player player, int[] newAmounts)
    {
        for(int i = 0; i < newAmounts.Length; i++)
        {
            player.resources[i].AmountUpdate(newAmounts[i]);
        }
    }
    public void InstantiateBottomMenu()
    {
        foreach(var x in ALL_Locations.FindAll(y => y.baseID == UIManager.Instance.currentBaseID))
        {
            UIManager.Instance.InstantiateBottomMenu(x);
        }
    }
    public void ItemBuy(string itemName, MapLocation location)
    {
        Debug.Log("Item bought with name:" + itemName);
        if(buildings.Find(x => x.buildingName == itemName) != null)
        {
            var itemPrefab = buildings.Find(x => x.buildingName == itemName);
            SpawnBuilding(itemName, location, itemPrefab);
        }
        else
        {
            if (units.Find(x => x.unitName == itemName) != null)
            {
                var itemPrefab = units.Find(x => x.unitName == itemName);
                SpawnUnit(itemName, location, itemPrefab);
            }
        }
    }
    public void SpawnUnit(string unitName, MapLocation location, Unit unitPrefab)
    {
        if (unitPrefab)
        {
            if (Player.Instance.Buy(unitPrefab.cost))
            {
                location.SpawnUnit(unitPrefab.gameObject);
                UIManager.Instance.OnCloseBuildingWindow();
            }
            else
            {
                UIManager.Instance.DialogWindow("Player cannot afford this UNIT");
            }
        }
        else UIManager.Instance.DialogWindow("UNIT Prefab not found");
    }
    public void SpawnBuilding(string building, MapLocation location, Structure buildingPrefab)
    {
        //Debug.Log(building);
        //var buildingPrefab = buildings.Find(x => x.buildingName == building);
        if (buildingPrefab)
        {
            if (Player.Instance.Buy(buildingPrefab.cost))
            {
                location.SpawnBuilding(buildingPrefab.gameObject);
                UIManager.Instance.OnCloseBuildingWindow();
            }
            else
            {
                UIManager.Instance.DialogWindow("Player cannot afford this building");
            }
        }
        else UIManager.Instance.DialogWindow("Building Prefab not found");
    }
}
