using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public int currentBaseID = 1;

    public ToggleGroup toggleGroupBases;
    public GameObject baseBorder;
    [SerializeField]
    public Color32[] borderColors;

    public ToggleGroup toggleGroupStructureTypes;

    public BuildingWindow window;
    public MapLocation currentSelected;

    public GameObject dialogWindow;
    public GameObject dialogWindowYesNo;
    public GameObject MiniMap;
    public Camera MiniMapCamera;

    public RectTransform bottomPanelContent;

    public List<GameObject> bottomPanelButtons;

    float lastClicked;

    [System.NonSerialized]
    public bool lookForNextClick = false;
    public Transform selectedRallyPoint = null;
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

    public GameObject winWindow;
    private void Start()
    {
        foreach (var x in baseBorder.GetComponentsInChildren<Image>())
        {
            x.color = borderColors[currentBaseID];
        }
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
                        foreach(var x in baseBorder.GetComponentsInChildren<Image>())
                        {
                            x.color = borderColors[currentBaseID-1];
                            Debug.Log("AAAA");
                        }
                    }
                });
                if(i > 0) Toggles[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
        leftTimer = gameObject.AddComponent<Timer>();
        leftTimer.AddTimer("Building", GameManager.Instance.secondsToFullLeftPanel, true, 0.25f);

        leftTimer.On_PingAction += UpdateSliderLeft;

        rightTimer = gameObject.AddComponent<Timer>();
        rightTimer.AddTimer("Building", GameManager.Instance.secondsToFullRightPanel, true, 0.25f);

        rightTimer.On_PingAction += UpdateSliderRight;
    }
    public void UnlockBase(string name)
    {
        var Toggles = toggleGroupBases.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < Toggles.Length; i++)
        {
            if (Toggles[i].name == name) Toggles[i].transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void SetTimer(bool Right, int level)
    {
        if (Right)
        {
            var percent = 1f - ((rightINDEX - level) / 4f);
            rightTimer.timeStarted = Time.time - GameManager.Instance.secondsToFullRightPanel * ((rightINDEX - level) / 4f);
            rightTimer.timeFinish = Time.time + GameManager.Instance.secondsToFullRightPanel * percent;
            UpdateSliderRight(rightTimer);
            rightTimer.On_PingAction += UpdateSliderRight;
        }
        else
        {
            var percent = 1f - ((leftINDEX - level) / 4f);
            leftTimer.timeStarted = Time.time - GameManager.Instance.secondsToFullLeftPanel * ((leftINDEX - level) / 4f);
            leftTimer.timeFinish = Time.time + GameManager.Instance.secondsToFullLeftPanel * percent;
            UpdateSliderLeft(leftTimer);
            leftTimer.On_PingAction += UpdateSliderLeft;
        }
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
                //selectedRallyPoint.GetComponentInChildren<Animator>().SetTrigger("OFF");
                foreach (var x in selectedRallyPoint.GetComponentsInChildren<Animator>())
                {
                    x.SetBool("HOLD", false);
                }
                lookForNextClick = false;
                selectedRallyPoint = null;
                if (selectedRallyPointButton)
                {
                    selectedRallyPointButton.color = Color.white;
                    selectedRallyPointButton = null;
                }
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
    public void LookToPlaceRallyPoint(Transform point)
    {
        if (selectedRallyPointButton) selectedRallyPointButton.color = Color.white;
        if(selectedRallyPoint)
        {
            foreach (var x in selectedRallyPoint.GetComponentsInChildren<Animator>())
            {
                x.SetBool("HOLD", false);
            }
        }
        lookForNextClick = true;
        selectedRallyPoint = point;
        selectedRallyPointButton = null;
    }
    public void LookToPlaceRallyPoint(Transform point, Image image)
    {
        lookForNextClick = true;
        selectedRallyPoint = point;
        selectedRallyPointButton = image;
        image.color = new Color(255,255,255,0.5f);
    }
    public void TurnOnOffMiniMap(bool state)
    {
        MiniMap.SetActive(state);
        MiniMapCamera.gameObject.SetActive(state);
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
        StartCoroutine(CloseDialogWindow(2f));
    }
    IEnumerator CloseDialogWindow(float t)
    {
        yield return new WaitForSeconds(t);
        dialogWindow.SetActive(false);
    }
    public void DialogWindowYesNo(string message, Amount[] _cost, UnityAction yesEvent, UnityAction noEvent)
    {
        if(_cost != null)
        {
            var column = dialogWindowYesNo.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0);
            column.gameObject.SetActive(true);
            var j = 0;
            foreach (var cost in _cost)
            {
                var text = column.transform.Find("Cost_" + j.ToString()).GetChild(0);
                text.gameObject.SetActive(true);
                text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
                j++;
            }
            while (j <= 2)
            {
                column.transform.Find("Cost_" + j).GetChild(0).gameObject.SetActive(false);
                j++;
            }
        }
        else
        {
            dialogWindowYesNo.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

        dialogWindowYesNo.SetActive(true);
        dialogWindowYesNo.GetComponentInChildren<TextMeshProUGUI>().text = message;
        var buttons = dialogWindowYesNo.GetComponentsInChildren<Button>();
        foreach(var x in buttons)
        {
            if (x.gameObject.name.Contains("YES"))
            {
                x.onClick.RemoveAllListeners();
                x.onClick.AddListenerOnce(yesEvent);
            }
            if (x.gameObject.name.Contains("NO"))
            {
                x.onClick.RemoveAllListeners();
                x.onClick.AddListenerOnce(noEvent);
            }
        }
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
        if (rightLoadingBar.value <= 0.25f) rightINDEX = 0;
        if (rightLoadingBar.value >= 0.25f && rightLoadingBar.value < 0.5f) rightINDEX = 1;
        if (rightLoadingBar.value >= 0.5f && rightLoadingBar.value < 0.75f) rightINDEX = 2;
        if (rightLoadingBar.value >= 0.75f && rightLoadingBar.value < 1f) rightINDEX = 3;
        if (rightLoadingBar.value == 1f) rightINDEX = 4;
    }
}
