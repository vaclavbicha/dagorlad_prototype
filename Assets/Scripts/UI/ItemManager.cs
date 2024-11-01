using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour {
    public int locationID;
    public Utility.BuildingSlotType type;

    public GameObject top;
    public GameObject mid;
    public GameObject bottom;
    public GameObject delete;

    public Transform RallyPoint;
    public Structure building;

    bool pressed = false;
    float endTimer;
    readonly float pressTime = 0.7f;

    enum ClickType { NoClick, SingleClick, DoubleClick };

    ClickType clickType = ClickType.NoClick;

    private readonly float doubleTapThreshold = 0.15f;
    int tapCount;

    readonly float passedTimeSinceLaskClick;
    bool isClicked;
    bool isDoubleClicked;

    Camera mainCamera;


    public void Start()
    {
        top = transform.GetChild(1).GetChild(0).gameObject;
        mainCamera = Camera.main;
    }
    public void Update() {
        if (endTimer <= Time.time) {
            if (pressed && building != null) {
                delete.SetActive(true);
            }
        }

        HandleClickAndDoubleClick();
    }

    private void HandleClickAndDoubleClick() {
        switch (clickType) {
            case ClickType.SingleClick:
                if (UIManager.Instance.selectedRallyPoint == null) {
                    PickRallyPointUp();
                } else {
                    PutRallyPointBackDown();
                }
                break;
            case ClickType.DoubleClick:
                if (UIManager.Instance.selectedRallyPoint != null) PutRallyPointBackDown();
                MoveToRallyPointPosition();
                break;
        }

        clickType = ClickType.NoClick;
    }

    public void SelectLocation(string arg)
    {
        UIManager.Instance.OnSelectLocation(locationID, type);
        //UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>().
    }

    IEnumerator SingleOrDoubleTap() {
        yield return new WaitForSeconds(doubleTapThreshold);

        if (tapCount == 1) {
            clickType = ClickType.SingleClick;
        } else if (tapCount == 2) {
            clickType = ClickType.DoubleClick;
        }

        tapCount = 0;
    }

    public void OnFlagClick()
    {
        tapCount++;
        StartCoroutine(SingleOrDoubleTap());
    }

    private void PickRallyPointUp() {
        if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == null) return;

        RallyPoint.GetComponent<Draggable>().PlayPickRallyPointUpAnimation();
        building.isAttackPoint = false;
        var img = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
        UIManager.Instance.LookToPlaceRallyPoint(RallyPoint, img);
    }

    private void PutRallyPointBackDown() {
        UIManager.Instance.DisableLookingForNextClick();
        UIManager.Instance.PutRallyPointDown(RallyPoint.position);
    }

    private void MoveToRallyPointPosition() {
        mainCamera.GetComponent<CameraMovement>().SetDestination(RallyPoint.position);
    }

    public void OnHold()
    {
        pressed = true;
        endTimer = Time.time + pressTime;
    }
    public void OnRelease()
    {
        pressed = false;
    }
    public void OnBuildingDestroy()
    {
        building.buildingSlot.DestroyBuilding();
    }
    public void OnBuildingUpgradeWindow()
    {
        //var location = GameManager.Instance.ALL_Locations.Find(x => x.id == locationID && x.type == type && x.baseID == UIManager.Instance.currentBaseID);
        if (building.buildingSlot)
        {
            var cost = building.ReturnCostOfLevel();
            if (cost != null)
            {
                UIManager.Instance.DialogWindowYesNo("Do you want to upgrade this building ?", cost,
                    () =>
                    {
                        Debug.Log("PLAYER SAID YES");
                        if (Player.Instance.Buy(cost))
                        {
                            foreach(var x in cost)
                            {
                                if (x.type == Utility.ResourceTypes.Time) building.buildingSlot.UpgradeStructure(x.value);
                            }
                            //location.building.GetComponent<Structure>().UpgradeStructure();
                            Debug.Log("PLAYER CAN AFFORD");
                            GameManager.Instance.InstantiateBottomMenu();
                        }
                        else UIManager.Instance.DialogWindow("Player cannot afford");
                    },
                    () => { Debug.Log("PLAYER SAID NO"); });
            }else UIManager.Instance.DialogWindow("Building is MAX level");
            delete.SetActive(false);
        }
        else UIManager.Instance.DialogWindow("Selected Location was not found!");
    }
}
