using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapLocation : MonoBehaviour
{
    private Camera mainCamera;

    public int id;
    public int baseID;
    public bool isVisisble;
    public Player owner;
    public Utility.LocationType type;
    public Utility.LocationStatus status;
    public Utility.LocationSelectionStatus selectionStatus;
    public GameObject building = null;
    public GameObject trainingUnit = null;
    public GameObject upgradeItem = null;

    SpriteRenderer sprite;
    Color color = Color.white;

    [SerializeField]
    public Timer timer;
    [SerializeField]
    public Slider loadingBar;

    public ItemManager itemManager;

    // Production Queue
    public List<GameObject> productionList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        sprite = GetComponent<SpriteRenderer>();
        status = Utility.LocationStatus.Free;
    }

    // Update is called once per frame
    void Update()
    {
        color = selectionStatus == Utility.LocationSelectionStatus.Selected ? Color.black : Color.white;
        color.a = selectionStatus == Utility.LocationSelectionStatus.Selected ? 1f : 0.78f;
        color = status == Utility.LocationStatus.Building ? Color.green : color;
        var screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        isVisisble = !(screenPosition.x <= 10 || screenPosition.x >= Screen.width || screenPosition.y <= 10 || screenPosition.y >= Screen.height);

        //color.a = isVisisble ? 1f : 1f;

        sprite.color = color;

    }
    public void SpawnUpgrade(ItemUpgrade upgradePrefab)
    {
        if (building != null && selectionStatus == Utility.LocationSelectionStatus.Selected)
        {
            upgradeItem = Instantiate(upgradePrefab.gameObject, new Vector3(-5, -10, 0), Quaternion.identity);

            upgradeItem.tag = "Player";
            upgradeItem.name += upgradeItem.GetInstanceID().ToString();

            status = Utility.LocationStatus.Training;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("BuildingUpgrade", building.GetComponent<Structure>().buildTime.value, true, 0.25f);

                itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == type);
                loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
                timer.On_PingAction += UpdateSlider;
                timer.On_Duration_End += isDoneUpgrading;

            }
            else
            {
                UIManager.Instance.DialogWindow("This location all ready is building upgrade");
            }
        }
        else
        {
            UIManager.Instance.DialogWindow("wierd error");
        }
    }
    public void SpawnUnitQueue()
    {
            trainingUnit = Instantiate(productionList[0], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            trainingUnit.tag = "Player";
            trainingUnit.name += trainingUnit.GetInstanceID().ToString();
            trainingUnit.GetComponent<MoveTo>().TransformDestination = building.GetComponent<Structure>().Rally_Point.transform;
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
            status = Utility.LocationStatus.Training;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Training", trainingUnit.GetComponent<OurUnit>().buildTime.value, true, 0.25f);

                itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == type);
                loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
                loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
                timer.On_PingAction += UpdateSlider;
                timer.On_Duration_End += isDoneTraining;

            }
            else
            {
                //UIManager.Instance.DialogWindow("This location all ready is traning troops");
            }
    }
    public void UpdateSliderMultiplier()
    {
        loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
    }
    public void SpawnUnit()
    {
        if (building != null && selectionStatus == Utility.LocationSelectionStatus.Selected && productionList.Count == 1)
        {
            Debug.Log("Started spawning ...");
            trainingUnit = Instantiate(productionList[0], new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            trainingUnit.tag = "Player";
            trainingUnit.name += trainingUnit.GetInstanceID().ToString();
            trainingUnit.GetComponent<MoveTo>().TransformDestination = building.GetComponent<Structure>().Rally_Point.transform;
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
            status = Utility.LocationStatus.Training;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Training", trainingUnit.GetComponent<OurUnit>().buildTime.value, true, 0.25f);

                itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == type);
                loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
                loadingBar.GetComponentInChildren<TextMeshProUGUI>().text = "X" + productionList.Count.ToString();
                timer.On_PingAction += UpdateSlider;
                timer.On_Duration_End += isDoneTraining;

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
        if (building == null && selectionStatus == Utility.LocationSelectionStatus.Selected)
        {
            building = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
            building.tag = "Structure";
            building.name += building.GetInstanceID().ToString();
            var buildingStructure = building.GetComponent<Structure>();
            buildingStructure.mapLocation = this;

            if (buildingStructure.locationType == Utility.LocationType.Attack)
            {
                buildingStructure.Rally_Point = Instantiate(GameManager.Instance.flagPrefab, transform.position + new Vector3(0.5f, 0.5f, 0f), Quaternion.identity);
                buildingStructure.Rally_Point.GetComponent<MoveTo>().SetDestination(transform.position + new Vector3(0.5f, 0.5f, 0f));
                building.GetComponent<StatsManager>().owner = Player.Instance.PlayerName;

                buildingStructure.Rally_Point.GetComponent<Draggable>().home = buildingStructure;
                buildingStructure.Rally_Point.GetComponent<Draggable>().owner = Player.Instance.PlayerName;
                buildingStructure.Rally_Point.GetComponent<SpriteRenderer>().sprite = UIManager.Instance.currentBaseID == 3 ? buildingStructure.Flag3 : buildingStructure.Flag1;
            }
            //building.SetActive(false);
            //var aux = buildingStructure.buildingSprite;
            //buildingStructure.buildingSprite = building.GetComponent<SpriteRenderer>().sprite;
            //building.GetComponent<SpriteRenderer>().sprite = aux;
            building.GetComponent<Structure>().ChangeSprite();
            StartCoroutine(UpdateBuildingSprite(building.GetComponent<Structure>().buildTime.value / 2));

            status = Utility.LocationStatus.Building;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Building", building.GetComponent<Structure>().buildTime.value, true, 0.25f);

                itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == type);
                loadingBar = Instantiate(buildingPrefab.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
                timer.On_PingAction += UpdateSlider;
                timer.On_Duration_End += isDoneBuilding;

            }
            else
            {
                UIManager.Instance.DialogWindow("This location all ready has a timer");
            }
        }
        else
        {
            UIManager.Instance.DialogWindow("This location all ready has a building on it");
        }
    }
    public void UpgradeStructure(float time)
    {
        status = Utility.LocationStatus.Building;

        if (timer == null)
        {
            timer = gameObject.AddComponent<Timer>();
            timer.AddTimer("Upgrade", time, true, 0.25f);

            itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == type);
            loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
            timer.On_PingAction += UpdateSlider;
            timer.On_Duration_End += isDoneUpgradingBuilding;

        }
        else
        {
            UIManager.Instance.DialogWindow("This location all ready has a timer");
        }
    }
    public void isDoneUpgradingBuilding(Timer _timer)
    {
        Camera.main.GetComponent<Animator>().SetTrigger("SmallShake");
        status = Utility.LocationStatus.Built;

        DestroyImmediate(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);

        building.GetComponent<Structure>().UpgradeStructure();

        UpdateItemManager(true, building.GetComponent<Structure>());
    }
    public void UpdateSlider(Timer _timer)
    {
        loadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
    }
    public void isDoneTraining(Timer _timer)
    {
        trainingUnit.SetActive(true);
        status = Utility.LocationStatus.Built;

        DestroyImmediate(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);

        building.GetComponent<Structure>().Rally_Point.GetComponent<Draggable>().NewUnitSpawned(trainingUnit.GetComponent<OurUnit>());

        productionList.RemoveAt(0);

        if (productionList.Count != 0)
        {
            SpawnUnitQueue();
        }
    }
    public void isDoneBuilding(Timer _timer)
    {
        Camera.main.GetComponent<Animator>().SetTrigger("SmallShake");
        building.GetComponent<Structure>().UpgradeStructure();
        //building.SetActive(true);
        //var aux = building.GetComponent<SpriteRenderer>().sprite;
        //building.GetComponent<SpriteRenderer>().sprite = building.GetComponent<Structure>().buildingSprite;
        //building.GetComponent<Structure>().buildingSprite = aux;

        status = Utility.LocationStatus.Built;

        DestroyImmediate(timer);
        if(loadingBar != null) Destroy(loadingBar.gameObject);

        UpdateItemManager(true, building.GetComponent<Structure>());

        if(building.GetComponent<Structure>().production.type == Utility.ResourceTypes.Supply)
        {
            Player.Instance.resources.Find(x => x.amount.type == Utility.ResourceTypes.Supply).AmountUpdateWithText(building.GetComponent<Structure>().production.value);
        }
        else if(type == Utility.LocationType.Resource)
        {
            Player.Instance.resources.Find(x => x.amount.type == building.GetComponent<Structure>().production.type).currentProduction += building.GetComponent<Structure>().production.value;
        }
    }
    public void isDoneUpgrading(Timer _timer)
    {
        status = Utility.LocationStatus.Built;
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
    IEnumerator UpdateBuildingSprite(float t)
    {
        yield return new WaitForSeconds(t);
        building.GetComponent<Structure>().UpgradeStructure();
    }
    public void UpdateItemManager(bool filling, Structure buttonIcon)
    {
        if(timer != null)
        {
            itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == type);
            loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
            timer.On_PingAction += UpdateSlider;
        }
        if (status != Utility.LocationStatus.Building && baseID == UIManager.Instance.currentBaseID)
        {
            itemManager.building = building.GetComponent<Structure>();

            itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
            Color fillingColor = new Color(0, 0, 0);
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
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon?.scrollIcon;
                    break;
                case Utility.LocationType.Attack:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = UIManager.Instance.currentBaseID == 3 ? buttonIcon?.Flag3 : buttonIcon?.Flag1;
                    itemManager.mid.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    itemManager.mid.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = buttonIcon?.scrollIcon;
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
    }
    public void DestroyBuilding()
    {

        if (building.GetComponent<Structure>().production.type == Utility.ResourceTypes.Supply)
        {
            Player.Instance.resources.Find(x => x.amount.type == Utility.ResourceTypes.Supply).AmountUpdateWithText(-building.GetComponent<Structure>().production.value);
        }
        else if (type == Utility.LocationType.Resource)
        {
            Player.Instance.resources.Find(x => x.amount.type == building.GetComponent<Structure>().production.type).currentProduction -= building.GetComponent<Structure>().production.value;
        }

        DestroyImmediate(building);
        status = Utility.LocationStatus.Free;
        GameManager.Instance.InstantiateBottomMenu();
    }
}
