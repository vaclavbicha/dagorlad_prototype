using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public List<GameObject> bases = new();
    public List<BuildingSlot> ALL_Locations = new();

    public List<Structure> buildings = new();

    [SerializeField]
    public List<OurUnit> units = new();
    public List<ItemUpgrade> upgrades = new();
    //public Dictionary<string, Structure> buildings = new Dictionary<string, Structure>();

    public List<Sprite> infoSprites = new();
    public List<Sprite> resourceSprites = new();
    public int[] startingAmounts = new[] { 500, 1000, 1500, 2000 };

    public int secondsToFullLeftPanel = 60;
    public int secondsToFullRightPanel = 120;

    public GameObject flagPrefab;

    public List<RallyPoint> enemies = new();

    public Amount[] BuildingDestructionCost;

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
        GetComponent<AudioManager>().Play("ThemeSong");

        for(int i = 0; i < bases.Count; i ++)
        {
            var locations = bases[i].GetComponentsInChildren<BuildingSlot>();
            foreach(var location in locations)
            {
                location.baseID = i + 1;
            }
            ALL_Locations.AddRange(locations);
        }
        infoSprites.AddRange(Resources.LoadAll<Sprite>("InfoSprites"));
        //resourceSprites.AddRange(Resources.LoadAll<Sprite>("ResourceSprites"));
        //foreach (GameObject i in Resources.LoadAll<GameObject>("Structures"))
        //{
        //    buildings.Add(i.name, i.GetComponent<Structure>());
        //}
        buildings.AddRange(Resources.LoadAll<Structure>("Structures"));
        units.AddRange(Resources.LoadAll<OurUnit>("Units"));
        upgrades.AddRange(Resources.LoadAll<ItemUpgrade>("Upgrades"));
        UpdatePlayerResources(Player.Instance, startingAmounts);

        UIManager.Instance.currentBaseID = 1;
        StartCoroutine(InstantiateBottomMenuCoroutine());
    }

    IEnumerator InstantiateBottomMenuCoroutine() {
        yield return new WaitForFixedUpdate();
        InstantiateBottomMenu();
    }

    private void UpdatePlayerResources(Player player, int[] newAmounts)
    {
        for(int i = 0; i < newAmounts.Length; i++)
        {
            player.resources[i].AmountUpdateWithText(newAmounts[i]);
        }
    }
    public void InstantiateBottomMenu()
    {
        for(int i = 0; i < UIManager.Instance.bottomPanelContent.childCount; i ++)
        {
            foreach(var child2 in UIManager.Instance.bottomPanelContent.GetChild(i).GetComponentsInChildren<ItemManager>())
            {
                DestroyImmediate(child2.gameObject);
            }
        }

        foreach (var x in ALL_Locations.FindAll(y => y.baseID == UIManager.Instance.currentBaseID))
        {
            UIManager.Instance.InstantiateBottomMenu(x);
        }
    }
    public void ItemBuy(string itemName, BuildingSlot location)
    {
        Debug.Log("Item bought with name:" + itemName);
        GetComponent<AudioManager>().Play("item_buy");

        if (buildings.Find(x => x.buildingName == itemName) != null)
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
            else
            {
                if (upgrades.Find(x => x.upgradeName == itemName) != null)
                {
                    var itemPrefab = upgrades.Find(x => x.upgradeName == itemName);
                    SpawnUpgrade(itemName, location, itemPrefab);
                }
            }
        }
    }
    public void SpawnUpgrade(string itemName, BuildingSlot location, ItemUpgrade upgradePrefab)
    {
        if (!upgradePrefab) {
            UIManager.Instance.DialogWindow("UNIT Prefab not found");
            return;
        }

        if (!Player.Instance.Buy(upgradePrefab.cost)) {
            UIManager.Instance.DialogWindow("Player cannot afford this UPGRADE");
            return;
        }

        if (Player.Instance.ownedUpgrades.FindAll(x => x.upgradeName == upgradePrefab.upgradeName).Count >= upgradePrefab.effect.maxOwned) {
            UIManager.Instance.DialogWindow("Player ownes the maximum amount of this upgrade");
            return;
        }

        location.SpawnUpgrade(upgradePrefab);
        UIManager.Instance.OnCloseBuildingWindow();
    }
    public void SpawnUnit(string unitName, BuildingSlot location, OurUnit unitPrefab)
    {
        if (!unitPrefab) {
            UIManager.Instance.DialogWindow("UNIT Prefab not found");
            return;
        }

        if (!Player.Instance.Buy(unitPrefab.cost)) {
            UIManager.Instance.DialogWindow("Not enough resources");
            return;
        }

        location.productionList.Add(unitPrefab.gameObject);
        location.SpawnUnit();
        location.UpdateSliderMultiplier();
        UIManager.Instance.OnCloseBuildingWindow();
    }
    public void SpawnBuilding(string building, BuildingSlot location, Structure buildingPrefab)
    {
        //Debug.Log(building);
        //var buildingPrefab = buildings.Find(x => x.buildingName == building);
        if (!buildingPrefab) {
            UIManager.Instance.DialogWindow("Building Prefab not found");
            return;
        }

        if (!Player.Instance.Buy(buildingPrefab.cost)) {
            UIManager.Instance.DialogWindow("Player cannot afford this building");
            return;
        }

        location.SpawnBuilding(buildingPrefab.gameObject);
        NavMeshManager.Instance.UpdateNavMesh();
        UIManager.Instance.OnCloseBuildingWindow();
    }
    public void GoMenu()
    {
        SceneManager.LoadScene(1);
    }
}
