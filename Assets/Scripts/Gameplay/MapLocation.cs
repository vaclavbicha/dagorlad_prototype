using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    SpriteRenderer sprite;
    Color color;

    [SerializeField]
    public Timer timer;
    [SerializeField]
    public Slider loadingBar;

    public ItemManager itemManager;

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
        color.a = selectionStatus == Utility.LocationSelectionStatus.Selected ? 1f : 0.3f;
        color = status == Utility.LocationStatus.Building ? Color.green : color;
        var screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        isVisisble = !(screenPosition.x <= 10 || screenPosition.x >= Screen.width || screenPosition.y <= 10 || screenPosition.y >= Screen.height);

        //color.a = isVisisble ? 1f : 1f;

        sprite.color = color;

    }
    public void SpawnUnit(GameObject unitPrefab)
    {
        if (building != null && selectionStatus == Utility.LocationSelectionStatus.Selected)
        {
            trainingUnit = Instantiate(unitPrefab, new Vector3(transform.position.x, transform.position.y, 10), Quaternion.identity);
            trainingUnit.tag = "Player";
            trainingUnit.name += trainingUnit.GetInstanceID().ToString();
            trainingUnit.GetComponent<MoveTo>().TransformDestination = building.GetComponent<Structure>().Rally_Point.transform;
            trainingUnit.GetComponent<StatsManager>().owner = Player.Instance.PlayerName;
            trainingUnit.GetComponent<OurUnit>().status = Utility.UnitStatus.GoingToFlag;
            trainingUnit.GetComponent<OurUnit>().Rally_Point = building.GetComponent<Structure>().Rally_Point.transform;
            trainingUnit.GetComponent<OurUnit>().home = building.GetComponent<Structure>();
            trainingUnit.SetActive(false);
            status = Utility.LocationStatus.Training;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Training", building.GetComponent<Structure>().buildTime.value, true, 0.25f);

                itemManager = UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id && x.type == type);
                loadingBar = Instantiate(building.GetComponent<Structure>().loadingBarPrefab, itemManager.transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
                timer.On_PingAction += UpdateSlider;
                timer.On_Duration_End += isDoneTraining;

            }
            else
            {
                UIManager.Instance.DialogWindow("This location all ready is traning troops");
            }
        }
        else
        {
            UIManager.Instance.DialogWindow("This location all ready has a building on it");
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
            var aux = buildingStructure.buildingSprite;
            buildingStructure.buildingSprite = building.GetComponent<SpriteRenderer>().sprite;
            building.GetComponent<SpriteRenderer>().sprite = aux;

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
    public void UpdateSlider(Timer _timer)
    {
        loadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
    }
    public void isDoneTraining(Timer _timer)
    {
        trainingUnit.SetActive(true);
        status = Utility.LocationStatus.Built;

        Destroy(timer);
        if (loadingBar != null) Destroy(loadingBar.gameObject);

        building.GetComponent<Structure>().Rally_Point.GetComponent<Draggable>().NewUnitSpawned(trainingUnit.GetComponent<OurUnit>());
    }
    public void isDoneBuilding(Timer _timer)
    {
        //building.SetActive(true);
        var aux = building.GetComponent<SpriteRenderer>().sprite;
        building.GetComponent<SpriteRenderer>().sprite = building.GetComponent<Structure>().buildingSprite;
        building.GetComponent<Structure>().buildingSprite = aux;

        status = Utility.LocationStatus.Built;

        DestroyImmediate(timer);
        if(loadingBar != null) Destroy(loadingBar.gameObject);

        UpdateItemManager(true, building.GetComponent<Structure>());
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
            switch (buttonIcon.locationType)
            {
                case Utility.LocationType.Defense:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon.Icon;
                    itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
                    break;
                case Utility.LocationType.Attack:
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = UIManager.Instance.currentBaseID == 3 ? buttonIcon?.Flag3 : buttonIcon?.Flag1;
                    itemManager.mid.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    itemManager.mid.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = buttonIcon?.Icon;
                    itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
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
                    itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon.Icon;
                    itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
                    break;
            }
        }
    }
    public void DestroyBuilding()
    {
        DestroyImmediate(building);
        status = Utility.LocationStatus.Free;
        GameManager.Instance.InstantiateBottomMenu();

    }
}
