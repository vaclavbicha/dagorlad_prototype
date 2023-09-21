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

    public void Start()
    {
        top = transform.GetChild(1).GetChild(0).gameObject;
    }
    public void SelectLocation(string arg)
    {
        UIManager.Instance.OnSelectLocation(locationID, type);
        //UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>().
    }
}
