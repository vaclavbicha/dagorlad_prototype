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

    public Transform RallyPoint;
    public Structure building;

    public void Start()
    {
        top = transform.GetChild(1).GetChild(0).gameObject;
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
}
