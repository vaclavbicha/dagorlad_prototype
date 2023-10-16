using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public int currentBaseID = 1;

    public ToggleGroup toggleGroupBases;
    public ToggleGroup toggleGroupStructureTypes;

    public BuildingWindow window;
    public MapLocation currentSelected;

    public GameObject dialogWindow;

    public RectTransform bottomPanelContent;

    public List<GameObject> bottomPanelButtons;

    float lastClicked;

    [System.NonSerialized]
    public bool lookForNextClick = false;
    Transform selectedRallyPoint = null;
    public Image selectedRallyPointButton = null;

    [SerializeField]
    public Timer leftTimer;
    public Slider leftLoadingBar;
    public int leftINDEX;

    [SerializeField]
    public Timer rightTimer;
    public Slider rightLoadingBar;
    public int rightINDEX;

    [System.NonSerialized]
    public Toggle lastSelectedInfoToggle = null;

    private void Start()
    {
        if (toggleGroupBases != null && toggleGroupBases.transform.childCount != 0)
        {
            var Toggles = toggleGroupBases.GetComponentsInChildren<Toggle>();
            //Toggles[0].SetIsOnWithoutNotify(true);
            Toggles[0].isOn = true;
            for (int i = 0; i < Toggles.Length; i++)
            {
                int panelNumber = i+1;
                Toggles[i].onValueChanged.AddListenerOnce(delegate (bool isOn)
                {
                    if (isOn)
                    {
                        if (Time.time - lastClicked <= 1f && currentBaseID == panelNumber) Camera.main.GetComponent<MoveTo>().SetDestination(GameManager.Instance.bases.Find(x => x.name.Contains(panelNumber.ToString())).transform.position);
                        lastClicked = Time.time;
                        currentBaseID = panelNumber;
                        GameManager.Instance.InstantiateBottomMenu();
                    }
                });
            }
        }
        leftTimer = gameObject.AddComponent<Timer>();
        leftTimer.AddTimer("Building", GameManager.Instance.secondsToFullLeftPanel, true, 0.25f);

        leftTimer.On_PingAction += UpdateSliderLeft;

        rightTimer = gameObject.AddComponent<Timer>();
        rightTimer.AddTimer("Building", GameManager.Instance.secondsToFullRightPanel, true, 0.25f);

        rightTimer.On_PingAction += UpdateSliderRight;
    }
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
    public void Update()
    {
        if (lookForNextClick)
        {
            if (Input.GetMouseButton(0) && !isMouseOverOverlayCanvas())
            {
                selectedRallyPoint.GetComponent<MoveTo>().SetDestination(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                lookForNextClick = false;
                selectedRallyPoint = null;
                selectedRallyPointButton.color = Color.white;
            }
        }
    }
    public bool isMouseOverOverlayCanvas()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var ev in raycastResults)
        {
            //if (ev.gameObject.layer == 9) ev.gameObject.GetComponent<Draggable>().ONNNN();
            if (ev.gameObject.layer == 5) return true; //layer 5 is the UI layer
        }
        return false;
    }
    public void OnCloseBuildingWindow()
    {
        if (lastSelectedInfoToggle)
        {
            lastSelectedInfoToggle.isOn = false;
            lastSelectedInfoToggle = null;
        }
        window.gameObject.SetActive(false);
        DeselectLocation();
    }

    public void OnSelectLocation(int location_id, Utility.LocationType location_type)
    {
        DeselectLocation();
        Debug.Log(location_id);
        currentSelected = GameManager.Instance.ALL_Locations.Find(x => x.id == location_id && x.type == location_type && x.baseID == currentBaseID);
        if (currentSelected)
        {
            currentSelected.selectionStatus = Utility.LocationSelectionStatus.Selected;
            window.ActivateWindow(location_id, location_type, currentSelected);
        }
        else DialogWindow("Selected Location not visible on screen");
    }

    public void DeselectLocation()
    {
        if (currentSelected) currentSelected.selectionStatus = Utility.LocationSelectionStatus.Unselected;
        currentSelected = null;
    }

    public void OnItemBuy(string itemName)
    {
        //GameManager.Instance.SpawnBuilding(buildingName, currentSelected);
        GameManager.Instance.ItemBuy(itemName, currentSelected);
    }
    public void LookToPlaceRallyPoint(Transform point, Image image)
    {
        lookForNextClick = true;
        selectedRallyPoint = point;
        selectedRallyPointButton = image;
        image.color = Color.blue;
    }
    public void OnBaseSwitch(int id)
    {
        //map

        //if (Time.time - lastClicked <= 1f && currentBaseID == id) Camera.main.GetComponent<MoveTo>().SetDestination(GameManager.Instance.bases.Find(x => x.name.Contains(id.ToString())).transform.position);
        //lastClicked = Time.time;
        //currentBaseID = id;
        //GameManager.Instance.InstantiateBottomMenu();

    }
    public void MapGoTo(int i)
    {
        Camera.main.GetComponent<MoveTo>().SetDestination(GameManager.Instance.bases.Find(x => x.name.Contains(i.ToString())).transform.position);
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }
    public void InstantiateBottomMenu(MapLocation location)
    {
        var button = Instantiate(bottomPanelButtons.Find(x => x.GetComponent<ItemManager>().type == location.type), bottomPanelContent.Find(location.type.ToString()));
        button.GetComponent<ItemManager>().locationID = location.id;
        button.name = "Button_" + location.type.ToString() +  "_" + location.id.ToString();

        if (location.building != null)
        {
            location.itemManager = button.GetComponent<ItemManager>();
            location.UpdateItemManager(true, location.building.GetComponent<Structure>());
        }
    }
    public void DialogWindow(string message)
    {
        dialogWindow.SetActive(true);
        dialogWindow.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }

    public void UpdateSliderLeft(Timer _timer)
    {
        leftLoadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
        if (leftLoadingBar.value <= 0.25f) leftINDEX = 0;
        if (leftLoadingBar.value >= 0.25f && leftLoadingBar.value < 0.5f) leftINDEX = 1;
        if (leftLoadingBar.value >= 0.5f && leftLoadingBar.value < 0.75f) leftINDEX = 2;
        if (leftLoadingBar.value >= 0.75f && leftLoadingBar.value < 1f) leftINDEX = 3;
        if (leftLoadingBar.value == 1f) leftINDEX = 4;
    }
    public void UpdateSliderRight(Timer _timer)
    {
        rightLoadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
        if (rightLoadingBar.value <= 0.25f) leftINDEX = 0;
        if (rightLoadingBar.value >= 0.25f && rightLoadingBar.value < 0.5f) leftINDEX = 1;
        if (rightLoadingBar.value >= 0.5f && rightLoadingBar.value < 0.75f) leftINDEX = 2;
        if (rightLoadingBar.value >= 0.75f && rightLoadingBar.value < 1f) leftINDEX = 3;
        if (rightLoadingBar.value == 1f) leftINDEX = 4;
    }
}
