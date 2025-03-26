using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UI;

public class BuildingSlot : MonoBehaviour
{
    private AudioSource audioSource;
    private Tilemap basesTilemap;
    private TileBase baseTile;

    private Vector3Int baseTileIndex;

    public int id;
    public int baseID;
    [SerializeField]
    bool isVisisble;
    public Player owner;
    public Utility.BuildingSlotType Type { get; set; }

    public Utility.LocationStatus status;
    public Utility.LocationStatus Status
    {
        get { return status; }
        set
        {
            status = value;

            switch (status)
            {
                default:
                    OnDefaultStatusChangeColor();
                    break;
                case Utility.LocationStatus.Building:
                    OnBuildingChangeColor();
                    break;
            }
        }
    }

    private Utility.LocationSelectionStatus selectionStatus;
    public Utility.LocationSelectionStatus SelectionStatus
    {
        get { return selectionStatus; }
        set
        {
            selectionStatus = value;

            switch (selectionStatus)
            {
                default:
                    OnUnselectChangeColor();
                    break;
                case Utility.LocationSelectionStatus.Selected:
                    OnSelectChangeColor();
                    break;
            }
        }
    }
    public GameObject building = null;
    public GameObject trainingUnit = null;
    public GameObject upgradeItem = null;

    SpriteRenderer sprite;
    Color color = Color.white;

    [SerializeField]
    private GameObject loadingBarPrefab;

    public Timer timer;
    public Slider loadingBar;

    public ItemManager itemManager;

    // Production Queue
    public List<GameObject> productionList = new();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        basesTilemap = FindObjectOfType<BasesTilemap>().GetComponent<Tilemap>();
        sprite = GetComponent<SpriteRenderer>();
        Status = Utility.LocationStatus.Free;
        sprite.color = Color.white;

        baseTileIndex = GetBaseTileLocation();
        baseTile = basesTilemap.GetTile(baseTileIndex);
    }

    //void Update() {
    //var screenPosition = mainCamera.WorldToScreenPoint(transform.position);
    //isVisisble = !(screenPosition.x <= 10 || screenPosition.x >= Screen.width || screenPosition.y <= 10 || screenPosition.y >= Screen.height);

    //color.a = isVisisble ? 0f : 1f;
    //}

    public void OnSelectChangeColor()
    {
        color = Color.black;
        color.a = 1f;
        sprite.color = color;
    }

    public void OnDefaultStatusChangeColor()
    {
        sprite.color = Color.white;
    }

    public void OnBuildingChangeColor()
    {
        color = Color.green;
        sprite.color = color;
    }

    public void OnUnselectChangeColor()
    {
        color = Color.white;
        color.a = 0.8f;
        sprite.color = color;
    }

    public void SelectLocation()
    {
        UIManager.Instance.OnSelectLocation(id, Type);
    }

    public void SpawnUpgrade(ItemUpgrade upgradePrefab)
    {
        if (building == null || SelectionStatus != Utility.LocationSelectionStatus.Selected) UIManager.Instance.DialogWindow("Weird error");

        upgradeItem = Instantiate(upgradePrefab.gameObject, new Vector3(-5, -10, 0), Quaternion.identity);

        upgradeItem.tag = "Player";
        upgradeItem.name += upgradeItem.GetInstanceID().ToString();

        Status = Utility.LocationStatus.Training;

        var buildTime = 0f;
        foreach (var x in upgradePrefab.cost)
        {
            if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
        }

        if (timer != null) UIManager.Instance.DialogWindow("This location is already building upgrade");

        timer = gameObject.AddComponent<Timer>();
        timer.AddTimer("BuildingUpgrade", buildTime, true, 0.25f);

        CreateLoadingBar();

        timer.On_PingAction += UpdateSlider;
        timer.On_Duration_End += IsDoneUpgrading;
    }

    public void SpawnUnitQueue()
    {
        Debug.Log("Started spawning ...");
        trainingUnit = Instantiate(productionList[0], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        trainingUnit.tag = "Player";
        trainingUnit.name += trainingUnit.GetInstanceID().ToString();
        trainingUnit.GetComponent<UnitMovement>().TransformDestination = building.GetComponent<Structure>().Rally_Point.transform;
        trainingUnit.GetComponent<StatsManager>().owner = Player.Instance.PlayerName;
        trainingUnit.GetComponent<OurUnit>().status = Utility.UnitStatus.GoingToFlag;
        trainingUnit.GetComponent<OurUnit>().Rally_Point = building.GetComponent<Structure>().Rally_Point.transform;
        trainingUnit.GetComponent<OurUnit>().home = building.GetComponent<Structure>();

        UpgradeStats();

        trainingUnit.SetActive(false);
        Status = Utility.LocationStatus.Training;

        var buildTime = 0f;
        foreach (var x in trainingUnit.GetComponent<OurUnit>().cost)
        {
            if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
        }
        if (timer == null) {//UIManager.Instance.DialogWindow("This location is already traning troops");

            timer = gameObject.AddComponent<Timer>();
            timer.AddTimer("Training", buildTime, true, 0.25f);

            CreateLoadingBar();
            loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
            timer.On_PingAction += UpdateSlider;
            timer.On_Duration_End += IsDoneTraining;
        }
    }
    public void UpdateSliderMultiplier()
    {
        if (loadingBar) loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
    }
    public void SpawnUnit() {
        if (building == null || SelectionStatus != Utility.LocationSelectionStatus.Selected) {
            Debug.Log("error");
            return;
        }

        Debug.Log("Started spawning ...");
        trainingUnit = Instantiate(productionList[0], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
        trainingUnit.tag = "Player";
        trainingUnit.name += trainingUnit.GetInstanceID().ToString();
        trainingUnit.GetComponent<UnitMovement>().TransformDestination = building.GetComponent<Structure>().Rally_Point.transform;
        trainingUnit.GetComponent<StatsManager>().owner = Player.Instance.PlayerName;
        trainingUnit.GetComponent<OurUnit>().status = Utility.UnitStatus.GoingToFlag;
        trainingUnit.GetComponent<OurUnit>().Rally_Point = building.GetComponent<Structure>().Rally_Point.transform;
        trainingUnit.GetComponent<OurUnit>().home = building.GetComponent<Structure>();
        
        UpgradeStats();

        trainingUnit.SetActive(false);
        Status = Utility.LocationStatus.Training;

        var buildTime = 0f;
        foreach (var x in trainingUnit.GetComponent<OurUnit>().cost) {
            if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
        }
        if (!timer) {
            timer = gameObject.AddComponent<Timer>();
            timer.AddTimer("Training", buildTime, true, 0.25f);

            CreateLoadingBar();
            loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
            timer.On_PingAction += UpdateSlider;
            timer.On_Duration_End += IsDoneTraining;
        }
    }

    private void UpgradeStats() {
        var upgrades = ResourceManager.Instance.ownedUpgrades.FindAll(y => y.effect.type == Utility.UpgradeEffectTypes.Troops);
        if (upgrades.Count > 0) {
            foreach (ItemUpgrade upgrade in upgrades) {
                trainingUnit.GetComponent<StatsManager>().UpgradeStat(upgrade.effect.stat, upgrade.effect.isPercent);
            }
        }
    }

    public void SpawnBuilding(GameObject buildingPrefab)
    {
        DisableBaseTile();
        if (building != null || SelectionStatus != Utility.LocationSelectionStatus.Selected) UIManager.Instance.DialogWindow("This location all ready has a building on it");

        building = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        building.tag = "Structure";
        building.name += building.GetInstanceID().ToString();
        var buildingStructure = building.GetComponent<Structure>();
        buildingStructure.buildingSlot = this;

        if (buildingStructure.locationType == Utility.BuildingSlotType.Attack)
        {
            buildingStructure.Rally_Point = Instantiate(GameManager.Instance.flagPrefab, transform.position + new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
            buildingStructure.Rally_Point.GetComponent<DraggableMovement>().SetDestination(transform.position + new Vector3(0.5f, 0.5f, 0f));
            building.GetComponent<StatsManager>().owner = Player.Instance.PlayerName;

            buildingStructure.Rally_Point.GetComponent<RallyPoint>().home = buildingStructure.gameObject;
            buildingStructure.Rally_Point.GetComponent<RallyPoint>().owner = Player.Instance.PlayerName;
            buildingStructure.Rally_Point.GetComponent<Animator>().enabled = false;
            buildingStructure.Rally_Point.GetComponent<SpriteRenderer>().sprite = UIManager.Instance.currentBaseID == 3 ? buildingStructure.Flag3 : buildingStructure.Flag1;
            buildingStructure.Rally_Point.GetComponent<Animator>().enabled = true;
            buildingStructure.Rally_Point.GetComponent<Animator>().runtimeAnimatorController = UIManager.Instance.currentBaseID == 3 ? buildingStructure.FlagController3 : buildingStructure.FlagController1;
        }
        //building.SetActive(false);
        //var aux = buildingStructure.buildingSprite;
        //buildingStructure.buildingSprite = building.GetComponent<SpriteRenderer>().sprite;
        //building.GetComponent<SpriteRenderer>().sprite = aux;
        building.GetComponent<Structure>().ChangeSprite();
        var buildTime = 0f;
        foreach (var x in building.GetComponent<Structure>().cost)
        {
            if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
        }
        StartCoroutine(UpdateBuildingSprite(buildTime / 2));

        Status = Utility.LocationStatus.Building;

        if (timer != null) UIManager.Instance.DialogWindow("This location all ready has a timer");

        timer = gameObject.AddComponent<Timer>();
        timer.AddTimer("Building", buildTime, true, 0.25f);

        CreateLoadingBar();

        timer.On_PingAction += UpdateSlider;
        timer.On_Duration_End += IsDoneBuilding;
    }
    public void UpgradeStructure(float time)
    {
        Status = Utility.LocationStatus.Building;

        if (timer != null) UIManager.Instance.DialogWindow("This location all ready has a timer");

        timer = gameObject.AddComponent<Timer>();
        timer.AddTimer("Upgrade", time, true, 0.25f);

        CreateLoadingBar();

        timer.On_PingAction += UpdateSlider;
        timer.On_Duration_End += IsDoneUpgradingBuilding;
    }

    public void IsDoneUpgradingBuilding(Timer _timer) {
        //Camera.main.GetComponent<Animator>().SetTrigger("SmallShake");
        Status = Utility.LocationStatus.Built;

        ClearTimerAndLoadingBar();

        building.GetComponent<Structure>().UpgradeStructure();

        UpdateItemManager(true, building.GetComponent<Structure>());
    }

    public void UpdateSlider(Timer _timer)
    {
        loadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
    }

    public void IsDoneTraining(Timer _timer)
    {
        trainingUnit.SetActive(true);
        Status = Utility.LocationStatus.Built;

        ClearTimerAndLoadingBar();

        building.GetComponent<Structure>().Rally_Point.GetComponent<RallyPoint>().NewUnitSpawned(trainingUnit.GetComponent<OurUnit>());

        productionList.RemoveAt(0);

        ResourceManager.Instance.BuyUnit(trainingUnit.GetComponent<OurUnit>().cost[0]);

        if (productionList.Count != 0)
        {
            SpawnUnitQueue();
        }
    }
    public void IsDoneBuilding(Timer _timer)
    {
        //Camera.main.GetComponent<Animator>().SetTrigger("SmallShake");
        building.GetComponent<Structure>().UpgradeStructure();
        //building.SetActive(true);
        //var aux = building.GetComponent<SpriteRenderer>().sprite;
        //building.GetComponent<SpriteRenderer>().sprite = building.GetComponent<Structure>().buildingSprite;
        //building.GetComponent<Structure>().buildingSprite = aux;

        Status = Utility.LocationStatus.Built;

        ClearTimerAndLoadingBar();

        UpdateItemManager(true, building.GetComponent<Structure>());
        
        if (Type == Utility.BuildingSlotType.Resource)
        {
            int newProductionValue = building.GetComponent<Structure>().production.value;
            
            switch (building.GetComponent<Structure>().production.type) {
                case Utility.ResourceTypes.Supply:
                    ResourceManager.Instance.SupplyResource.AddMaxValue(newProductionValue);
                    break;
                case Utility.ResourceTypes.Gold:
                    ResourceManager.Instance.GoldResource.AddCurrentProduction(newProductionValue);
                    break;
                case Utility.ResourceTypes.Wood:
                    ResourceManager.Instance.WoodResource.AddCurrentProduction(newProductionValue);
                    break;
            }
        }
    }
    public void IsDoneUpgrading(Timer _timer)
    {
        Status = Utility.LocationStatus.Built;
        ItemUpgrade upgrade = upgradeItem.GetComponent<ItemUpgrade>();
        ResourceManager.Instance.AddUpgrade(upgrade);

        if (upgrade.effect.type == Utility.UpgradeEffectTypes.Resource)
        {
            int additionalProductionValue = upgrade.effect.resourceAmount.value;

            switch (upgrade.effect.resourceAmount.type) {
                case Utility.ResourceTypes.Supply:
                    ResourceManager.Instance.SupplyResource.AddMaxValue(additionalProductionValue);
                    break;
                case Utility.ResourceTypes.Gold: 
                    ResourceManager.Instance.GoldResource.AddCurrentProduction(additionalProductionValue);
                    break;
                case Utility.ResourceTypes.Wood:
                    ResourceManager.Instance.WoodResource.AddCurrentProduction(additionalProductionValue);
                    break;
            }
        }
        if (upgrade.effect.type == Utility.UpgradeEffectTypes.Troops)
        {
            var statsManagers = FindObjectsOfType<StatsManager>();
            foreach (var z in statsManagers)
            {
                if (z.gameObject.layer != 6)
                {
                    z.UpgradeStat(upgrade.effect.stat, upgrade.effect.isPercent);
                }
            }
        }

        ClearTimerAndLoadingBar();
    }
    public void CancelLoading()
    {
        EnableBaseTile();
        Debug.Log("Cancel loading bar");
        ClearTimerAndLoadingBar();

        switch (Status)
        {
            case Utility.LocationStatus.Building:
                if (building.GetComponent<Structure>().level < 0)
                {
                    ResourceManager.Instance.Refund(building.GetComponent<Structure>().cost);
                    if (Type == Utility.BuildingSlotType.Attack) Destroy(building.GetComponent<Structure>().Rally_Point);
                    Destroy(building);
                    Status = Utility.LocationStatus.Free;
                    SelectionStatus = Utility.LocationSelectionStatus.Unselected;
                }
                else
                {
                    Status = Utility.LocationStatus.Built;
                    SelectionStatus = Utility.LocationSelectionStatus.Unselected;
                    UpdateItemManager(true, building.GetComponent<Structure>());
                }
                break;

            case Utility.LocationStatus.Training:
                if (Type == Utility.BuildingSlotType.Attack)
                {
                    ResourceManager.Instance.RefundUnitCost(trainingUnit.GetComponent<OurUnit>().cost);
                    Destroy(trainingUnit);
                    productionList.RemoveAll(x => x);
                }
                if (Type == Utility.BuildingSlotType.Resource)
                {
                    ResourceManager.Instance.Refund(upgradeItem.GetComponent<ItemUpgrade>().cost);
                    Destroy(upgradeItem);
                }
                break;
        }

        NavMeshManager.Instance.UpdateNavMesh();
    }
    IEnumerator UpdateBuildingSprite(float t)
    {
        yield return new WaitForSeconds(t);
        if (building) building.GetComponent<Structure>().UpgradeStructure();
    }
    public void UpdateItemManager(bool filling, Structure buttonIcon)
    {
        if (timer != null)
        {
            CreateLoadingBar();
            timer.On_PingAction += UpdateSlider;
        }
        if (Status != Utility.LocationStatus.Building && baseID == UIManager.Instance.currentBaseID) {
            itemManager.building = building.GetComponent<Structure>();

            switch (buttonIcon.locationType) {
                case Utility.BuildingSlotType.Defense:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon != null ? buttonIcon.scrollIcon : null;
                    break;
                case Utility.BuildingSlotType.Attack:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = UIManager.Instance.currentBaseID == 3 ? buttonIcon != null ? buttonIcon.ButtonIcon3 : null : buttonIcon != null ? buttonIcon.ButtonIcon1 : null;
                    itemManager.mid.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    itemManager.mid.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    itemManager.mid.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = buttonIcon != null ? buttonIcon.scrollIcon : null;
                    //EX DRAGDROP.cs
                    //var x = itemManager.bottom.GetComponentInChildren<DragDrop>();
                    //var y = itemManager.bottom.GetComponentInChildren<Toggle>();
                    //x.GetComponent<Image>().enabled = true;
                    //y.GetComponent<Image>().enabled = true;
                    //y.onValueChanged.RemoveAllListeners();
                    //y.onValueChanged.AddListener((isON) => { building.GetComponent<Structure>().isAttackPoint = isON; });
                    //x.RallyPoint = building.GetComponent<Structure>().Rally_Point.transform;

                    //OLD FLAGS
                    //foreach(var x in itemManager.bottom.GetComponentsInChildren<Button>())
                    //{
                    //    x.GetComponent<Image>().enabled = true;
                    //}
                    itemManager.RallyPoint = building.GetComponent<Structure>().Rally_Point.transform;
                    break;
                case Utility.BuildingSlotType.Resource:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon?.scrollIcon;
                    break;
            }
        }
        if (Status == Utility.LocationStatus.Building && baseID == UIManager.Instance.currentBaseID && buttonIcon.locationType == Utility.BuildingSlotType.Attack &&
            building.GetComponent<Structure>().level >= 0)
        {
            Debug.Log("ASDASDASDASFSAA !!!!!!");
            itemManager.building = building.GetComponent<Structure>();

            itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = UIManager.Instance.currentBaseID == 3 ? buttonIcon?.ButtonIcon3 : buttonIcon?.ButtonIcon1;
            itemManager.mid.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            itemManager.mid.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            itemManager.RallyPoint = building.GetComponent<Structure>().Rally_Point.transform;
        }
    }

    public void DestroyBuilding()
    {
        if (timer) ClearTimerAndLoadingBar();

        // Deduct demolish price
        ResourceManager.Instance.PayDemolishPrice();

        // Stop production
        if (Type == Utility.BuildingSlotType.Resource) {
            int lostProductionValue = building.GetComponent<Structure>().production.value;

            switch (building.GetComponent<Structure>().production.type) {
                case Utility.ResourceTypes.Supply:
                    ResourceManager.Instance.SupplyResource.DeductCurrentValue(lostProductionValue);
                    break;
                case Utility.ResourceTypes.Gold:
                    ResourceManager.Instance.GoldResource.DeductCurrentProduction(lostProductionValue);
                    break;
                case Utility.ResourceTypes.Wood:
                    ResourceManager.Instance.WoodResource.DeductCurrentProduction(lostProductionValue);
                    break;
            }
        } else if (Type == Utility.BuildingSlotType.Attack) {
            Destroy(building.GetComponent<Structure>().Rally_Point);
        }

        DestroyImmediate(building);
        Status = Utility.LocationStatus.Free;
        GameManager.Instance.InstantiateBottomMenu();
        EnableBaseTile();
    }

    private void CreateLoadingBar() {

        itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == Type);
        loadingBar = Instantiate(loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
        loadingBar.interactable = false;
        loadingBar.GetComponentInChildren<Button>().onClick.AddListener(CancelLoading);
    }

    private void ClearTimerAndLoadingBar() {
        DestroyImmediate(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);
    }

    public void PlayStoneSound()
    {
        audioSource.Play();
    }

    private Vector3Int GetBaseTileLocation()
    {
        Vector3Int cellIndex = basesTilemap.WorldToCell(transform.position);
        return cellIndex;
    }

    private void DisableBaseTile()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
        basesTilemap.SetTile(baseTileIndex, null);
    }

    private void EnableBaseTile()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
        basesTilemap.SetTile(baseTileIndex, baseTile);
    }
}
