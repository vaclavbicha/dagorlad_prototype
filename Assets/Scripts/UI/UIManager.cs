using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public int currentBaseID = 1;

    public BuildingWindow window;
    public MapLocation currentSelected;

    public GameObject dialogWindow;

    public RectTransform bottomPanelContent;

    public List<GameObject> bottomPanelButtons;

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
    public void OnCloseBuildingWindow()
    {
        window.gameObject.SetActive(false);
        DeselectLocation();
    }

    public void OnSelectLocation(int location_id, Utility.LocationType location_type)
    {
        DeselectLocation();
        Debug.Log(location_id);
        currentSelected = GameManager.Instance.ALL_Locations.Find(x => x.isVisisble && x.id == location_id && x.type == location_type);
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

    public void InstantiateBottomMenu(MapLocation location)
    {
        var button = Instantiate(bottomPanelButtons.Find(x => x.GetComponent<ItemManager>().type == location.type), bottomPanelContent.Find(location.type.ToString()));
        button.GetComponent<ItemManager>().locationID = location.id;
        button.name = "Button_" + location.type.ToString() +  "_" + location.id.ToString();
    }
    public void DialogWindow(string message)
    {
        dialogWindow.SetActive(true);
        dialogWindow.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }
}
