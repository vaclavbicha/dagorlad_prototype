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
    }

    // Update is called once per frame
    void Update()
    {
        color = selectionStatus == Utility.LocationSelectionStatus.Selected ? Color.red : Color.white;
        color = status == Utility.LocationStatus.Building ? Color.green : color;
        var screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        isVisisble = !(screenPosition.x <= 10 || screenPosition.x >= Screen.width || screenPosition.y <= 10 || screenPosition.y >= Screen.height);

        color.a = isVisisble ? 0.2f : 1f;

        sprite.color = color;
    }
    public void SpawnUnit(GameObject unitPrefab)
    {
        if (building != null && selectionStatus == Utility.LocationSelectionStatus.Selected)
        {
            trainingUnit = Instantiate(unitPrefab, transform.position, Quaternion.identity);//building.GetComponent<Structure>().Rally_Point.transform.position, Quaternion.identity);
            //trainingUnit.GetComponent<Unit>().rangeOrigin = building.GetComponent<Structure>().Rally_Point.transform;
            //trainingUnit.GetComponent<Unit>().range = 0.2f;
            trainingUnit.GetComponent<MoveTo>().TransformDestination = building.GetComponent<Structure>().Rally_Point.transform;
            trainingUnit.SetActive(false);
            status = Utility.LocationStatus.Training;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Training", 2f, true, 0.25f);

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
            building.GetComponent<Structure>().Rally_Point = Instantiate(GameManager.Instance.flagPrefab, transform.position + new Vector3(0.5f,0.5f,0f), Quaternion.identity);
            building.GetComponent<Structure>().Rally_Point.GetComponent<MoveTo>().SetDestination(transform.position + new Vector3(0.5f, 0.5f, 0f));
            building.SetActive(false);
            status = Utility.LocationStatus.Building;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Building", 3f, true, 0.25f);

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
        Destroy(loadingBar.gameObject);
    }
    public void isDoneBuilding(Timer _timer)
    {
        building.SetActive(true);
        status = Utility.LocationStatus.Built;

        Destroy(timer);
        Destroy(loadingBar.gameObject);

        UpdateItemManager(true, building.GetComponent<Structure>());
    }
    public void UpdateItemManager(bool filling, Structure buttonIcon)
    {
        switch (buttonIcon.locationType)
        {
            case Utility.LocationType.Defense:
                itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon.Icon;
                itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
                break;
            case Utility.LocationType.Attack:
                itemManager.mid.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = buttonIcon?.Flag;
                itemManager.mid.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                itemManager.mid.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Image>().sprite = buttonIcon?.Icon;
                itemManager.mid.transform.GetChild(0).GetComponent<Image>().enabled = filling;
                var x = itemManager.bottom.GetComponentsInChildren<DragDrop>();
                Debug.Log(x.Length);
                x[0].GetComponent<Image>().enabled = true;
                x[1].GetComponent<Image>().enabled = true;
                x[0].RallyPoint = building.GetComponent<Structure>().Rally_Point.transform;
                break;
            case Utility.LocationType.Resource:
                break;
        }
    }
}
