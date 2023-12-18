using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public int locationID;
    public Utility.LocationType type;

    public GameObject top;
    public GameObject mid;
    public GameObject bottom;
    public GameObject delete;

    public Transform RallyPoint;
    public Structure building;

    bool pressed = false;
    float endTimer;
    float pressTime = 0.7f;


    public void Start()
    {
        top = transform.GetChild(1).GetChild(0).gameObject;
    }
    public void Update()
    {
        if(endTimer <= Time.time)
        {
            if (pressed && building != null)
            {
                delete.SetActive(true);
            }
        }
    }
    public void SelectLocation(string arg)
    {
        UIManager.Instance.OnSelectLocation(locationID, type);
        //UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>().
    }
    public void OnFlagClick(bool isRed)
    {
        if(UIManager.Instance.selectedRallyPoint == null)
        {
            var img = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
            foreach (var x in RallyPoint.GetComponentsInChildren<Animator>())
            {
                x.SetBool("HOLD", true);
            }
            building.isAttackPoint = isRed;
            UIManager.Instance.LookToPlaceRallyPoint(RallyPoint, img);
        }
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
        building.mapLocation.DestroyBuilding();
    }
    public void OnBuildingUpgradeWindow()
    {
        //var location = GameManager.Instance.ALL_Locations.Find(x => x.id == locationID && x.type == type && x.baseID == UIManager.Instance.currentBaseID);
        if (building.mapLocation)
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
                                if (x.type == Utility.ResourceTypes.Time) building.mapLocation.UpgradeStructure(x.value);
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
