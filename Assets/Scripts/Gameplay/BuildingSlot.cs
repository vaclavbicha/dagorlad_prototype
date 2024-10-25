using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildingSlot : MonoBehaviour {
    private Camera mainCamera;
    private AudioSource audioSource;

    public int id;
    public int baseID;
    [SerializeField]
    bool isVisisble;
    public Player owner;
    public Utility.LocationType Type { get; set; }

    public Utility.LocationStatus status;
    public Utility.LocationStatus Status { 
        get { return status; }
        set {
            status = value;

            switch (status) {
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
    public Utility.LocationSelectionStatus SelectionStatus { 
        get { return selectionStatus; } 
        set {
            selectionStatus = value;

            switch (selectionStatus) {
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

    public Timer timer;
    public Slider loadingBar;

    public ItemManager itemManager;

    // Production Queue
    public List<GameObject> productionList = new();

    void Start() {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        sprite = GetComponent<SpriteRenderer>();
        Status = Utility.LocationStatus.Free;
        sprite.color = Color.white;
    }

    //void Update() {
    //    if(Input.GetMouseButtonDown(0) && IsMouseOverBuildingSlot()) {
    //        Debug.Log("Fdfd");
    //    }
        //var screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        //isVisisble = !(screenPosition.x <= 10 || screenPosition.x >= Screen.width || screenPosition.y <= 10 || screenPosition.y >= Screen.height);

        //color.a = isVisisble ? 0f : 1f;
    //}

    public void OnSelectChangeColor() {
        color = Color.black;
        color.a = 1f;
        sprite.color = color;
    }

    public void OnDefaultStatusChangeColor() {
        sprite.color = Color.white;
    }

    public void OnBuildingChangeColor() {
        color = Color.green;
        sprite.color = color;
    }

    public void OnUnselectChangeColor() {
        color = Color.white;
        color.a = 0.8f;
        sprite.color = color;
    }

    public void SelectLocation() {
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
        foreach (var x in upgradePrefab.cost) {
            if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
        }

        if (timer != null) UIManager.Instance.DialogWindow("This location all ready is building upgrade");

        timer = gameObject.AddComponent<Timer>();
        timer.AddTimer("BuildingUpgrade", buildTime, true, 0.25f);

        itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == Type);
        loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
        timer.On_PingAction += UpdateSlider;
        timer.On_Duration_End += IsDoneUpgrading;
        loadingBar.GetComponentInChildren<Button>().onClick.AddListener(CancelLoading);
    }
    public void SpawnUnitQueue()
    {
            trainingUnit = Instantiate(productionList[0], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            trainingUnit.tag = "Player";
            trainingUnit.name += trainingUnit.GetInstanceID().ToString();
            trainingUnit.GetComponent<UnitMovement>().TransformDestination = building.GetComponent<Structure>().Rally_Point.transform;
            trainingUnit.GetComponent<StatsManager>().owner = Player.Instance.PlayerName;
            trainingUnit.GetComponent<OurUnit>().status = Utility.UnitStatus.GoingToFlag;
            trainingUnit.GetComponent<OurUnit>().Rally_Point = building.GetComponent<Structure>().Rally_Point.transform;
            trainingUnit.GetComponent<OurUnit>().home = building.GetComponent<Structure>();
            var upgradez = Player.Instance.ownedUpgrades.FindAll(y => y.effect.type == Utility.UpgradeEffectTypes.Troops);
            if (upgradez.Count > 0)
            {
                foreach (var up in upgradez)
                {
                    trainingUnit.GetComponent<StatsManager>().UpgradeStat(up.effect.stat, up.effect.isPercent);
                }
            }
            trainingUnit.SetActive(false);
            Status = Utility.LocationStatus.Training;

            var buildTime = 0f;
            foreach (var x in trainingUnit.GetComponent<OurUnit>().cost)
            {
                if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
            }
            if (timer != null) //UIManager.Instance.DialogWindow("This location all ready is traning troops");

            timer = gameObject.AddComponent<Timer>();
            timer.AddTimer("Training", buildTime, true, 0.25f);

            itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == Type);
            loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
            loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
            timer.On_PingAction += UpdateSlider;
            timer.On_Duration_End += IsDoneTraining;
            loadingBar.GetComponentInChildren<Button>().onClick.AddListener(CancelLoading);
    }
    public void UpdateSliderMultiplier()
    {
        loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
    }
    public void SpawnUnit()
    {
        if (building != null && SelectionStatus == Utility.LocationSelectionStatus.Selected && productionList.Count == 1)
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
            var upgradez = Player.Instance.ownedUpgrades.FindAll(y => y.effect.type == Utility.UpgradeEffectTypes.Troops);
            if(upgradez.Count > 0)
            {
                foreach(var up in upgradez)
                {
                    trainingUnit.GetComponent<StatsManager>().UpgradeStat(up.effect.stat, up.effect.isPercent);
                }
            }
            trainingUnit.SetActive(false);
            Status = Utility.LocationStatus.Training;

            var buildTime = 0f;
            foreach (var x in trainingUnit.GetComponent<OurUnit>().cost)
            {
                if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
            }
            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Training", buildTime, true, 0.25f);

                itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == Type);
                loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
                loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
                timer.On_PingAction += UpdateSlider;
                timer.On_Duration_End += IsDoneTraining;
                loadingBar.GetComponentInChildren<Button>().onClick.AddListener(CancelLoading);

            }
            else
            {
                //UIManager.Instance.DialogWindow("This location all ready is traning troops");
            }
        }
        else
        {
            //UIManager.Instance.DialogWindow("This location all ready has a building on it");
        }
    }
    public void SpawnBuilding(GameObject buildingPrefab)
    {
        if (building != null || SelectionStatus != Utility.LocationSelectionStatus.Selected) UIManager.Instance.DialogWindow("This location all ready has a building on it");

        building = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
        building.tag = "Structure";
        building.name += building.GetInstanceID().ToString();
        var buildingStructure = building.GetComponent<Structure>();
        buildingStructure.buildingSlot = this;

        if (buildingStructure.locationType == Utility.LocationType.Attack) {
            buildingStructure.Rally_Point = Instantiate(GameManager.Instance.flagPrefab, transform.position + new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
            buildingStructure.Rally_Point.GetComponent<DraggableMovement>().SetDestination(transform.position + new Vector3(0.5f, 0.5f, 0f));
            building.GetComponent<StatsManager>().owner = Player.Instance.PlayerName;

            buildingStructure.Rally_Point.GetComponent<Draggable>().home = buildingStructure.gameObject;
            buildingStructure.Rally_Point.GetComponent<Draggable>().owner = Player.Instance.PlayerName;
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
        foreach (var x in building.GetComponent<Structure>().cost) {
            if (x.type == Utility.ResourceTypes.Time) buildTime = x.value;
        }
        StartCoroutine(UpdateBuildingSprite(buildTime / 2));

        Status = Utility.LocationStatus.Building;

        if (timer != null) UIManager.Instance.DialogWindow("This location all ready has a timer");

        timer = gameObject.AddComponent<Timer>();
        timer.AddTimer("Building", buildTime, true, 0.25f);

        itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == Type);
        loadingBar = Instantiate(buildingPrefab.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
        timer.On_PingAction += UpdateSlider;
        timer.On_Duration_End += IsDoneBuilding;
        loadingBar.GetComponentInChildren<Button>().onClick.AddListener(CancelLoading);
    }
    public void UpgradeStructure(float time)
    {
        Status = Utility.LocationStatus.Building;

        if (timer != null) UIManager.Instance.DialogWindow("This location all ready has a timer");

        timer = gameObject.AddComponent<Timer>();
        timer.AddTimer("Upgrade", time, true, 0.25f);

        itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == Type);
        loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
        timer.On_PingAction += UpdateSlider;
        timer.On_Duration_End += IsDoneUpgradingBuilding;
        loadingBar.GetComponentInChildren<Button>().onClick.AddListener(CancelLoading);
    }
    public void IsDoneUpgradingBuilding(Timer _timer)
    {
        //Camera.main.GetComponent<Animator>().SetTrigger("SmallShake");
        Status = Utility.LocationStatus.Built;

        DestroyImmediate(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);

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

        DestroyImmediate(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);

        building.GetComponent<Structure>().Rally_Point.GetComponent<Draggable>().NewUnitSpawned(trainingUnit.GetComponent<OurUnit>());

        productionList.RemoveAt(0);

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

        DestroyImmediate(timer);
        if(loadingBar != null) Destroy(loadingBar.gameObject);

        UpdateItemManager(true, building.GetComponent<Structure>());

        if(building.GetComponent<Structure>().production.type == Utility.ResourceTypes.Supply)
        {
            Player.Instance.resources.Find(x => x.amount.type == Utility.ResourceTypes.Supply).AmountUpdateWithText(building.GetComponent<Structure>().production.value);
        }
        else if(Type == Utility.LocationType.Resource)
        {
            Player.Instance.resources.Find(x => x.amount.type == building.GetComponent<Structure>().production.type).currentProduction += building.GetComponent<Structure>().production.value;
        }
    }
    public void IsDoneUpgrading(Timer _timer)
    {
        Status = Utility.LocationStatus.Built;
        var reff = upgradeItem.GetComponent<ItemUpgrade>();
        Player.Instance.ownedUpgrades.Add(reff);
        if (reff.effect.type == Utility.UpgradeEffectTypes.Resource)
        {
            if (reff.effect.resourceAmount.type == Utility.ResourceTypes.Supply)
            {
                Player.Instance.resources.Find(x => x.amount.type == reff.effect.resourceAmount.type).AmountUpdateWithText(reff.effect.resourceAmount.value);
            }
            else
            {
                Player.Instance.resources.Find(x => x.amount.type == reff.effect.resourceAmount.type).currentProduction += reff.effect.resourceAmount.value;
            }
        }
        if(reff.effect.type == Utility.UpgradeEffectTypes.Troops)
        {
            var statsManagers = FindObjectsOfType<StatsManager>();
            foreach(var z in statsManagers)
            {
                if(z.gameObject.layer != 6)
                {
                    z.UpgradeStat(reff.effect.stat, reff.effect.isPercent);
                }
            }
        }

        Destroy(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);
    }
    public void CancelLoading()
    {
        Debug.Log("Cancel loading bar");
        DestroyImmediate(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);

        switch (Status)
        {
            case Utility.LocationStatus.Building:
                if(building.GetComponent<Structure>().level < 0)
                {
                    Player.Instance.Refund(building.GetComponent<Structure>().cost);
                    if (Type == Utility.LocationType.Attack) Destroy(building.GetComponent<Structure>().Rally_Point);
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
                if(Type == Utility.LocationType.Attack)
                {
                    foreach (var unit in productionList)
                    {
                        Player.Instance.Refund(unit.GetComponent<OurUnit>().cost);
                    }
                    Destroy(trainingUnit);
                    productionList.RemoveAll(x => x);
                }
                if(Type == Utility.LocationType.Resource)
                {
                    Player.Instance.Refund(upgradeItem.GetComponent<ItemUpgrade>().cost);
                    Destroy(upgradeItem);
                }
                break;
        }

        NavMeshManager.Instance.UpdateNavMesh();
    }
    IEnumerator UpdateBuildingSprite(float t)
    {
        yield return new WaitForSeconds(t);
        if(building) building.GetComponent<Structure>().UpgradeStructure();
    }
    public void UpdateItemManager(bool filling, Structure buttonIcon)
    {
        if(timer != null)
        {
            itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == Type);
            loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
            timer.On_PingAction += UpdateSlider;
            loadingBar.GetComponentInChildren<Button>().onClick.AddListener(CancelLoading);
        }
        if (Status != Utility.LocationStatus.Building && baseID == UIManager.Instance.currentBaseID)
        {
            itemManager.building = building.GetComponent<Structure>();

            itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
            Color fillingColor = new(0, 0, 0);
            switch (itemManager.building.level)
            {
                case 0:
                    ColorUtility.TryParseHtmlString("#646D6F", out fillingColor);
                    break;
                case 1:
                    ColorUtility.TryParseHtmlString("#7ECFEC", out fillingColor);
                    break;
                case 2:
                    ColorUtility.TryParseHtmlString("#ECB136", out fillingColor);
                    break;
            }
            itemManager.mid.transform.GetChild(0).GetComponent<Image>().color = fillingColor;

            switch (buttonIcon.locationType)
            {
                case Utility.LocationType.Defense:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon != null ? buttonIcon.scrollIcon : null;
                    break;
                case Utility.LocationType.Attack:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = UIManager.Instance.currentBaseID == 3 ? buttonIcon != null ? buttonIcon.ButtonIcon3 : null : buttonIcon != null ? buttonIcon.ButtonIcon1 : null;
                    itemManager.mid.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    itemManager.mid.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    itemManager.mid.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = buttonIcon != null ? buttonIcon.scrollIcon : null;
                    //itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
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
                case Utility.LocationType.Resource:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon?.scrollIcon;
                    //itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
                    break;
            }
        }
        if (Status == Utility.LocationStatus.Building && baseID == UIManager.Instance.currentBaseID && buttonIcon.locationType == Utility.LocationType.Attack &&
            building.GetComponent<Structure>().level >= 0)
        {
            Debug.Log("ASDASDASDASFSAA !!!!!!");
            itemManager.building = building.GetComponent<Structure>();

            itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
            Color fillingColor = new(0, 0, 0);
            switch (itemManager.building.level)
            {
                case 0:
                    ColorUtility.TryParseHtmlString("#646D6F", out fillingColor);
                    break;
                case 1:
                    ColorUtility.TryParseHtmlString("#7ECFEC", out fillingColor);
                    break;
                case 2:
                    ColorUtility.TryParseHtmlString("#ECB136", out fillingColor);
                    break;
            }
            itemManager.mid.transform.GetChild(0).GetComponent<Image>().color = fillingColor;
            itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = UIManager.Instance.currentBaseID == 3 ? buttonIcon?.ButtonIcon3 : buttonIcon?.ButtonIcon1;
            itemManager.mid.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            itemManager.mid.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
            itemManager.RallyPoint = building.GetComponent<Structure>().Rally_Point.transform;
        }
    }
    public void DestroyBuilding()
    {

        if (building.GetComponent<Structure>().production.type == Utility.ResourceTypes.Supply)
        {
            Player.Instance.resources.Find(x => x.amount.type == Utility.ResourceTypes.Supply).AmountUpdateWithText(-building.GetComponent<Structure>().production.value);
        }
        else if (Type == Utility.LocationType.Resource)
        {
            Player.Instance.resources.Find(x => x.amount.type == building.GetComponent<Structure>().production.type).currentProduction -= building.GetComponent<Structure>().production.value;
        }

        DestroyImmediate(building);
        Status = Utility.LocationStatus.Free;
        GameManager.Instance.InstantiateBottomMenu();
    }

    public void PlaySound() {
        audioSource.Play();
    }
}
