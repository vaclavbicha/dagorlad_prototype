using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public BuildingWindow window;
    public MapLocation currentSelected;

    public GameObject dialogWindow;
    public GameObject topPanel;

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

    public void OnSelectLocation(string arg)
    {
        Debug.Log("UI MANAGER " + arg);
        var toks = arg.Split(",");
        var location_id = int.Parse(toks[0]);
        var location_type = toks[1];

        DeselectLocation();
        currentSelected = GameManager.Instance.ALL_Locations.Find(x => x.isVisisble && x.id == location_id && x.type == (Utility.LocationType)System.Enum.Parse(typeof(Utility.LocationType), location_type));
        if (currentSelected)
        {
            currentSelected.selectionStatus = Utility.LocationSelectionStatus.Selected;
            window.ActivateWindow(location_id, location_type);
        }
        else DialogWindow("Selected Location not visible on screen");
    }

    public void DeselectLocation()
    {
        if (currentSelected) currentSelected.selectionStatus = Utility.LocationSelectionStatus.Unselected;
        currentSelected = null;
    }

    public void OnBuyBuilding(string buildingName)
    {
        GameManager.Instance.SpawnBuilding(buildingName, currentSelected);
    }

    public void DialogWindow(string message)
    {
        dialogWindow.SetActive(true);
        dialogWindow.GetComponentInChildren<TextMeshProUGUI>().text = message;
    }
}
