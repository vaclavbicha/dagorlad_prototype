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

    public ToggleGroup toggleGroup;

    public BuildingWindow window;
    public MapLocation currentSelected;

    public GameObject dialogWindow;

    public RectTransform bottomPanelContent;

    public List<GameObject> bottomPanelButtons;

    float lastClicked;

    private void Start()
    {
        if (toggleGroup != null && toggleGroup.transform.childCount != 0)
        {
            var Toggles = toggleGroup.GetComponentsInChildren<Toggle>();
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
    public void OnCloseBuildingWindow()
    {
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
}
