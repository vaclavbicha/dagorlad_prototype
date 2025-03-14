using System;
using System.Collections;
using System.Collections.Generic;
using DanielLochner.Assets;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BuildingWindow;

public class UIManager : MonoBehaviour {
    public static UIManager Instance;

    public int currentBaseID = 1;

    public ToggleGroup toggleGroupBases;
    public GameObject baseBorder;
    [SerializeField]
    public Color32[] borderColors;

    public ToggleGroup toggleGroupStructureTypes;

    public BuildingWindow buildingWindow;
    public BuildingSlot currentSelected;

    public GameObject dialogWindow;
    public GameObject dialogWindowYesNo;
    public GameObject MiniMap;
    public Camera MiniMapCamera;

    Camera mainCamera;

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

    LayerMask buildingSlotLayer;

    // these two values are used to prevent selecting the building slot in the camera or if building slot is under the UI
    bool isMovingOnTheBuilding;
    bool isTouchStarted;

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        mainCamera = Camera.main;

        SetupUIComponents();

        foreach (var x in baseBorder.GetComponentsInChildren<Image>()) {
            x.color = borderColors[currentBaseID];
        }
        if (toggleGroupBases != null && toggleGroupBases.transform.childCount != 0) {
            var Toggles = toggleGroupBases.GetComponentsInChildren<Toggle>();
            //Toggles[0].SetIsOnWithoutNotify(true);
            Toggles[0].isOn = true;
            for (int i = 0; i < Toggles.Length; i++) {
                int panelNumber = i + 1;
                Toggles[i].onValueChanged.AddListener(delegate (bool isOn) {
                    if (isOn) {
                        if (Time.time - lastClicked <= 1f && currentBaseID == panelNumber) mainCamera.GetComponent<CameraMovement>().SetDestination(GameManager.Instance.bases.Find(x => x.name.Contains(panelNumber.ToString())).transform.position);
                        lastClicked = Time.time;
                        currentBaseID = panelNumber;
                        GameManager.Instance.InstantiateBottomMenu();
                        foreach (var x in baseBorder.GetComponentsInChildren<Image>()) {
                            x.color = borderColors[currentBaseID - 1];
                            Debug.Log("AAAA");
                        }
                    }
                });
                if (i > 0) Toggles[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    private void SetupUIComponents() {
        buildingWindow = FindObjectOfType<BuildingWindow>(true);
        // load the building window
        if (buildingWindow) {
            buildingWindow.gameObject.SetActive(true);
            buildingWindow.SetupWindow();
            buildingWindow.DeactivateWindow();
        }

        rightLoadingBar.interactable = false;
        leftLoadingBar.interactable = false;

        leftTimer = gameObject.AddComponent<Timer>();
        leftTimer.AddTimer("Building", GameManager.Instance.secondsToFullLeftPanel, true, 0.25f);

        leftTimer.On_PingAction += UpdateSliderLeft;

        rightTimer = gameObject.AddComponent<Timer>();
        rightTimer.AddTimer("Building", GameManager.Instance.secondsToFullRightPanel, true, 0.25f);

        rightTimer.On_PingAction += UpdateSliderRight;

        buildingSlotLayer = 1 << LayerMask.NameToLayer("BuildingSlot");
    }

    public void UnlockBase(string name) {
        var Toggles = toggleGroupBases.GetComponentsInChildren<Toggle>();
        for (int i = 0; i < Toggles.Length; i++) {
            if (Toggles[i].name == name) Toggles[i].transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void SetTimer(bool Right, int level) {
        if (Right) {
            var percent = 1f - ((rightINDEX - level) / 4f);
            rightTimer.timeStarted = Time.time - GameManager.Instance.secondsToFullRightPanel * ((rightINDEX - level) / 4f);
            rightTimer.timeFinish = Time.time + GameManager.Instance.secondsToFullRightPanel * percent;
            UpdateSliderRight(rightTimer);
            rightTimer.On_PingAction += UpdateSliderRight;
        } else {
            var percent = 1f - ((leftINDEX - level) / 4f);
            leftTimer.timeStarted = Time.time - GameManager.Instance.secondsToFullLeftPanel * ((leftINDEX - level) / 4f);
            leftTimer.timeFinish = Time.time + GameManager.Instance.secondsToFullLeftPanel * percent;
            UpdateSliderLeft(leftTimer);
            leftTimer.On_PingAction += UpdateSliderLeft;
        }
    }
    
    public void Update() {
        if (!MiniMap.activeSelf) HandleBuildingSlotClicked();

        if (!lookForNextClick) return;

        if (selectedRallyPoint && Input.GetMouseButton(0) && !IsMouseOverOverlayCanvas()) {
            PutRallyPointDown(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    public void PutRallyPointDown(Vector3 destination) {
        mainCamera.GetComponent<CameraMovement>().IsLocked = false;
        mainCamera.GetComponent<CameraMovement>().DisableEdgeScrolling();
        mainCamera.GetComponent<Animator>().SetTrigger("SmallShake");

        selectedRallyPoint.transform.position = destination;
        selectedRallyPoint.GetComponent<DraggableMovement>().SetDestination(destination);
        selectedRallyPoint.GetComponent<RallyPoint>().PlayPutRallyPointDownAnimation();

        lookForNextClick = false;

        selectedRallyPoint.GetComponent<RallyPoint>().ManageTargets();

        selectedRallyPoint = null;
        if (selectedRallyPointButton) {
            selectedRallyPointButton.color = Color.white;
            selectedRallyPointButton = null;
        }
    }

    public void OnCloseBuildingWindow() {
        if (lastSelectedInfoToggle) {
            lastSelectedInfoToggle.isOn = false;
            lastSelectedInfoToggle = null;
        }
        buildingWindow.DeactivateWindow();
        DeselectLocation();
    }

    public void OnSelectLocation(int location_id, Utility.BuildingSlotType location_type) {
        if (selectedRallyPoint) PutRallyPointDown(selectedRallyPoint.position);
        DeselectLocation();
        Debug.Log(location_id);
        currentSelected = GameManager.Instance.ALL_Locations.Find(x => x.id == location_id && x.Type == location_type && x.baseID == currentBaseID);
        if (!currentSelected) return;

        currentSelected.SelectionStatus = Utility.LocationSelectionStatus.Selected;
        buildingWindow.ActivateWindow(currentSelected);
    }

    public void DeselectLocation() {
        if (currentSelected) {
            currentSelected.SelectionStatus = Utility.LocationSelectionStatus.Unselected;
        }
        currentSelected = null;
    }

    public void OnItemBuy(string itemName) {
        //GameManager.Instance.SpawnBuilding(buildingName, currentSelected);
        GameManager.Instance.ItemBuy(itemName, currentSelected);
    }

    public void OnBuildingDestroy() {
        if (!currentSelected) {
            DialogWindow("Selected Location was not found!");
            return;
        }

        currentSelected.DestroyBuilding();
        buildingWindow.DeactivateWindow();
    }

    public void OnOpenUpgradeBuildingWindow() {
        if (!currentSelected) {
            DialogWindow("Selected Location was not found!");
            return;
        }

        OpenUpgradeBuildingWindow();
        buildingWindow.DeactivateWindow();
    }

    public void OpenUpgradeBuildingWindow() {
        var cost = currentSelected.building.GetComponent<Structure>().ReturnCostOfLevel();
        if (cost == null) {
            DialogWindow("Building is at the MAX level");
            return;
        }

        // good code - temporary disabled
        //DialogWindowYesNo("Do you want to upgrade this building ?", cost,
        //    () => {
        //        Debug.Log("PLAYER SAID YES");
        //        if (Player.Instance.Buy(cost)) {
        //            foreach (var x in cost) {
        //                if (x.type == Utility.ResourceTypes.Time) currentSelected.UpgradeStructure(x.value);
        //            }
        //            //location.building.GetComponent<Structure>().UpgradeStructure();
        //            Debug.Log("PLAYER CAN AFFORD");
        //            GameManager.Instance.InstantiateBottomMenu();
        //        } else DialogWindow("Player cannot afford");
        //    },
        //    () => { Debug.Log("PLAYER SAID NO"); });

        // temporary

        // FIX
        //if (Player.Instance.Buy(cost)) {
        //    foreach (var x in cost) {
        //        if (x.type == Utility.ResourceTypes.Time) currentSelected.UpgradeStructure(x.value);
        //    }
        //        //location.building.GetComponent<Structure>().UpgradeStructure();
        //        Debug.Log("PLAYER CAN AFFORD");
        //        GameManager.Instance.InstantiateBottomMenu();
        //    } else DialogWindow("Player cannot afford");
    }

    public void LookToPlaceRallyPoint(Transform point) {
        if (selectedRallyPointButton) selectedRallyPointButton.color = Color.white;
        if (selectedRallyPoint) {
            foreach (var x in selectedRallyPoint.GetComponentsInChildren<Animator>()) {
                x.SetBool("HOLD", false);
            }
            selectedRallyPoint.GetComponent<RallyPoint>().PlayPutRallyPointDownAnimation();
        }
        lookForNextClick = true;
        selectedRallyPoint = point;
        selectedRallyPointButton = null;
    }
    public void LookToPlaceRallyPoint(Transform point, Image image) {
        lookForNextClick = true;
        selectedRallyPoint = point;
        selectedRallyPointButton = image;
        image.color = new Color(255, 255, 255, 0.5f);
    }

    public void DisableLookingForNextClick() {
        lookForNextClick = false;
    }

    public void TurnOnOffMiniMap(bool state) {
        MiniMap.SetActive(state);
        MiniMapCamera.gameObject.SetActive(state);
    }
    public void OnBaseSwitch(int id) {
        //map

        //if (Time.Time - lastClicked <= 1f && currentBaseID == id) mainCamera.GetComponent<CameraMovement>().SetDestination(GameManager.Instance.bases.Find(x => x.name.Contains(id.ToString())).transform.position);
        //lastClicked = Time.Time;
        //currentBaseID = id;
        //GameManager.Instance.InstantiateBottomMenu();

    }
    public void MapGoTo(int i) {
        Debug.Log("GO TO " + i);
        mainCamera.GetComponent<CameraMovement>().SetDestination(GameManager.Instance.bases.Find(x => x.name.Contains(i.ToString())).transform.position);
        EventSystem.current.currentSelectedGameObject.transform.parent.gameObject.SetActive(false);
    }
    public void InstantiateBottomMenu(BuildingSlot location) {
        var button = Instantiate(bottomPanelButtons.Find(x => x.GetComponent<ItemManager>().type == location.Type), bottomPanelContent.Find(location.Type.ToString()));
        button.GetComponent<ItemManager>().locationID = location.id;
        button.name = "Button_" + location.Type.ToString() + "_" + location.id.ToString();

        if (location.building != null) {
            location.itemManager = button.GetComponent<ItemManager>();
            location.UpdateItemManager(true, location.building.GetComponent<Structure>());
        }
    }
    public void DialogWindow(string message) {
        dialogWindow.SetActive(true);
        dialogWindow.GetComponentInChildren<TextMeshProUGUI>().text = message;
        StartCoroutine(CloseDialogWindow(2f));
    }
    IEnumerator CloseDialogWindow(float t) {
        yield return new WaitForSeconds(t);
        dialogWindow.SetActive(false);
    }
    public void DialogWindowYesNo(string message, Amount[] costs, UnityAction yesEvent, UnityAction noEvent) {
        if (costs == null) dialogWindowYesNo.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0).gameObject.SetActive(false);

        var column = dialogWindowYesNo.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(1).GetChild(0);
        column.gameObject.SetActive(true);
        var j = 0;
        foreach (var cost in costs) {
            var text = column.transform.Find("Cost_" + j.ToString()).GetChild(0);
            text.gameObject.SetActive(true);
            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText());
            j++;
        }
        while (j <= 2) {
            column.transform.Find("Cost_" + j).GetChild(0).gameObject.SetActive(false);
            j++;
        }

        dialogWindowYesNo.SetActive(true);
        dialogWindowYesNo.GetComponentInChildren<TextMeshProUGUI>().text = message;
        var buttons = dialogWindowYesNo.GetComponentsInChildren<Button>();
        foreach (var x in buttons) {
            if (x.gameObject.name.Contains("YES")) {
                x.onClick.RemoveAllListeners();
                x.onClick.AddListener(yesEvent);
            }
            if (x.gameObject.name.Contains("NO")) {
                x.onClick.RemoveAllListeners();
                x.onClick.AddListener(noEvent);
            }
        }
    }
    public void UpdateSliderLeft(Timer _timer) {
        leftLoadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
        if (leftLoadingBar.value <= 0.25f) leftINDEX = 0;
        if (leftLoadingBar.value >= 0.25f && leftLoadingBar.value < 0.5f) leftINDEX = 1;
        if (leftLoadingBar.value >= 0.5f && leftLoadingBar.value < 0.75f) leftINDEX = 2;
        if (leftLoadingBar.value >= 0.75f && leftLoadingBar.value < 1f) leftINDEX = 3;
        if (leftLoadingBar.value == 1f) leftINDEX = 4;
    }
    public void UpdateSliderRight(Timer _timer) {
        rightLoadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
        if (rightLoadingBar.value <= 0.25f) rightINDEX = 0;
        if (rightLoadingBar.value >= 0.25f && rightLoadingBar.value < 0.5f) rightINDEX = 1;
        if (rightLoadingBar.value >= 0.5f && rightLoadingBar.value < 0.75f) rightINDEX = 2;
        if (rightLoadingBar.value >= 0.75f && rightLoadingBar.value < 1f) rightINDEX = 3;
        if (rightLoadingBar.value == 1f) rightINDEX = 4;
    }

    public void HandleBuildingSlotClicked() {
        // && !SpellManager.Instance.IsAnySpellSelected() doesn't work
        if (IsMouseOverOverlayCanvas()) return;

        Vector2 rayOrigin = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.zero, Mathf.Infinity, buildingSlotLayer);

        if (!hit.collider) return;

        BuildingSlot clickedBuildingSlot = hit.collider.gameObject.GetComponent<BuildingSlot>();

        // If the player is moving the camera, don't select the building slot
        if (Input.touchCount <= 0) {
            if (isMovingOnTheBuilding) isMovingOnTheBuilding = false;
            return;
        }

        TouchPhase touchPhase = Input.GetTouch(0).phase;

        switch (touchPhase) {
            case TouchPhase.Began:
                isTouchStarted = true;
                break;
            case TouchPhase.Moved:
                isMovingOnTheBuilding = true;
                break;
            case TouchPhase.Ended:
                if (isMovingOnTheBuilding) return;
                if (!isTouchStarted) return;

                // If the building window is active and the player taps on the building slot, close it
                // FIX IT
                //if (buildingWindow.gameObject.activeSelf) {
                //    lookForNextClick = false;
                //    buildingWindow.gameObject.SetActive(false);
                //    return;
                //} else {
                //    lookForNextClick = true;
                //}

                SelectBuildingSlot(clickedBuildingSlot);
                isTouchStarted = false;
                break;
        }
    }

    private void SelectBuildingSlot(BuildingSlot clickedBuildingSlot) {
        clickedBuildingSlot.PlayStoneSound();
        OnSelectLocation(clickedBuildingSlot.id, clickedBuildingSlot.Type);
    }

    public bool IsMouseOverOverlayCanvas() {
        PointerEventData pointerEventData = new(EventSystem.current) {
            position = Input.mousePosition
        };

        List<RaycastResult> raycastResults = new();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var ev in raycastResults) {
            if (ev.gameObject.layer == 5) return true; //layer 5 is the UI layer
        }
        return false;
    }
}
