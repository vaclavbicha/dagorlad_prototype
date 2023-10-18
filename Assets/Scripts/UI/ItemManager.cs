using System.Collections;
using System.Collections.Generic;
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
    float pressTime = 2f;


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
        var img = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Image>();
        building.isAttackPoint = isRed;
        UIManager.Instance.LookToPlaceRallyPoint(RallyPoint, img);
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
}
