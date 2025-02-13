using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BuildingWindowBottomPanel;

public class BuildingWindow : MonoBehaviour
{
    public GameObject infoPanel;
    public GameObject bottomPanelNormal;
    public GameObject bottomPanelUtility;
    BuildingWindowBottomPanel currentBottomPanel;


    private void Awake() {
        SetupComponents();
    }

    private void SetupComponents() {
        if (!infoPanel) infoPanel = transform.GetChild(1).gameObject;

        if (!bottomPanelNormal) {
            bottomPanelNormal = transform.GetChild(2).transform.gameObject;
        }

        if (!bottomPanelUtility) {
            bottomPanelUtility = transform.GetChild(3).transform.gameObject;
        }

        bottomPanelNormal.GetComponent<BuildingWindowBottomPanel>().SetupColumns();
        bottomPanelNormal.SetActive(false);

        bottomPanelUtility.GetComponent<BuildingWindowBottomPanel>().SetupColumns();
        bottomPanelUtility.SetActive(false);
    }

    public void DeactivateWindow() {
        if (currentBottomPanel) {
            foreach (BuildingColumn buildingColumn in currentBottomPanel.BuildingColumns) {
                buildingColumn.TurnOffInfoIcon();
            }
        }

        if (currentBottomPanel) {
            currentBottomPanel.gameObject.SetActive(false);
            currentBottomPanel = null;
        }

        infoPanel.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ActivateWindow(BuildingSlot buildingSlot)
    {
        if (buildingSlot.Status == Utility.LocationStatus.Built || buildingSlot.Status == Utility.LocationStatus.Training)
        {
            switch (buildingSlot.Type)
            {
                case Utility.BuildingSlotType.Defense:
                    break;
                case Utility.BuildingSlotType.Attack:
                    currentBottomPanel = OpenPanel(BottomPanelMode.utilityPanel).GetComponent<BuildingWindowBottomPanel>();
                    List<OurUnit> units = buildingSlot.building.GetComponent<Structure>().producingUnits;

                    BuildingColumn[] normalBuildingColumns = currentBottomPanel.NormalBuildingColumns;
                    BuildingColumn[] utilityColumns = currentBottomPanel.UtilityColumns;

                    for (int j = 0; j < units.Count; j++) {
                        OurUnit unit = units[j];
                        BuildingColumn normalBuildingColumn = normalBuildingColumns[j];

                        UpdateUnitCostDisplay(normalBuildingColumns[j], unit);
                        UpdateUnitResourceAndButtonInteractivity(buildingSlot, normalBuildingColumns[j], unit);

                        // Check if the player has enough resources to buy the unit in real time - fix it!
                        //Player.Instance.onResourcesUpdated += () => {
                        //    if (gameObject.activeSelf) UpdateUnitResourceAndButtonInteractivity(buildingSlot, normalBuildingColumns[j], unit);
                        //};
                    }

                    break;
                case Utility.BuildingSlotType.Resource:
                    currentBottomPanel = OpenPanel(BottomPanelMode.normalPanel).GetComponent<BuildingWindowBottomPanel>();

                    int i = 0;
                    List<ItemUpgrade> upgrades = GameManager.Instance.upgrades.FindAll(x => x.type == buildingSlot.building.GetComponent<Structure>().upgradeType);
                    foreach (ItemUpgrade upgrade in upgrades)
                    {
                        foreach (Amount cost in upgrade.cost)
                        {
                            var text = currentBottomPanel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                            text.gameObject.SetActive(true);
                            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);

                        }
                        //Debug.Log(buildingSlot.baseID + " MAP : " + buildingSlot.name + buildingSlot.building.GetComponent<Structure>().level);
                        for (int j = 0; j < currentBottomPanel.transform.GetChild(i).childCount; j++)
                        {
                            if (currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Cost") ||
                                currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap"))
                            {
                                foreach (var x in currentBottomPanel.transform.GetChild(i).GetChild(j).GetComponentsInChildren<Button>())
                                {
                                    var cantAfford = false;
                                    if (currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Cost"))
                                    {
                                        var resourceName = currentBottomPanel.transform.GetChild(i).GetChild(j).name.Replace("Cost_", "");
                                        if (resourceName == "Gold" || resourceName == "Wood")
                                        {
                                            var v1 = (Utility.ResourceTypes)Enum.Parse(typeof(Utility.ResourceTypes), resourceName);
                                            var owned = Player.Instance.resources.Find(x => x.amount.type == v1).amount.value;
                                            var costamount = Array.Find(upgrade.cost, x => x.type == v1);
                                            if (costamount != null) cantAfford = owned < costamount.value;
                                        }
                                    }

                                    x.interactable = upgrade.minimumBuildingTier <= buildingSlot.building.GetComponent<Structure>().level && !cantAfford;
                                }
                            }
                            if (currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap")) currentBottomPanel.transform.GetChild(i).GetChild(j).GetComponent<Button>().interactable = upgrade.minimumBuildingTier > buildingSlot.building.GetComponent<Structure>().level ? false : true;
                        }
                        currentBottomPanel.transform.GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = upgrade.Icon;
                        currentBottomPanel.transform.GetChild(i).GetComponent<BuildingColumn>().currentItemName = upgrade.upgradeName;
                        i++;
                    }
                    break;
            }
        }
        if (buildingSlot.Status == Utility.LocationStatus.Free)
        {
            currentBottomPanel = OpenPanel(BottomPanelMode.normalPanel).GetComponent<BuildingWindowBottomPanel>();

            int i = 0;
            foreach (var obj in GameManager.Instance.buildings.FindAll(x => x.locationType == buildingSlot.Type))
            {
                foreach (Amount cost in obj.cost)
                {
                    var text = currentBottomPanel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                    
                    text.gameObject.SetActive(true);
                    text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                    text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);

                }
                for (int j = 0; j < currentBottomPanel.transform.GetChild(i).childCount; j++)
                {
                    if (currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Cost") ||
                        currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap"))
                    {
                        foreach (var x in currentBottomPanel.transform.GetChild(i).GetChild(j).GetComponentsInChildren<Button>())
                        {
                            var cantAfford1 = false;
                            if (currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Cost"))
                            {
                                var resourceName = currentBottomPanel.transform.GetChild(i).GetChild(j).name.Replace("Cost_", "");
                                if (resourceName == "Gold" || resourceName == "Wood")
                                {
                                    var v1 = (Utility.ResourceTypes)Enum.Parse(typeof(Utility.ResourceTypes), resourceName);
                                    var owned = Player.Instance.resources.Find(x => x.amount.type == v1).amount.value;
                                    var costamount = Array.Find(obj.cost, x => x.type == v1);
                                    if(costamount != null) cantAfford1 = owned < costamount.value;
                                }
                            }

                            x.interactable = !cantAfford1;
                        }
                    }
                    if (currentBottomPanel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap")) currentBottomPanel.transform.GetChild(i).GetChild(j).GetComponent<Button>().interactable = true;
                }

                foreach (var resource in GameManager.Instance.resourceSprites)
                {
                    bool found = false;
                    foreach (Amount cost in obj.cost)
                    {
                        if (cost.type.ToString() == resource.name) found = true;
                    }
                    if (!found) currentBottomPanel.transform.GetChild(i).transform.Find("Cost_" + resource.name).GetChild(0).gameObject.SetActive(false);
                }
                currentBottomPanel.transform.GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = obj.Icon;
                currentBottomPanel.transform.GetChild(i).GetComponent<BuildingColumn>().currentItemName = obj.buildingName;
                i++;
            }
        }
    }

    private GameObject OpenPanel(BottomPanelMode bottomPanelMode) {

        GameObject panel;
        switch (bottomPanelMode) {

            case BottomPanelMode.utilityPanel:
                panel = bottomPanelUtility;
                break;
            case BottomPanelMode.normalPanel:
            default:
                panel = bottomPanelNormal;
                break;

        }

        panel.SetActive(true);
        gameObject.SetActive(true);
        return panel;
    }

    private static void UpdateUnitResourceAndButtonInteractivity(BuildingSlot buildingSlot, BuildingColumn buildingColumn, OurUnit unit) {
        for (int j = 0; j < buildingColumn.transform.childCount; j++) {
            if (buildingColumn.transform.GetChild(j).name.Contains("Cost") ||
                buildingColumn.transform.GetChild(j).name.Contains("Button_Wrap")) {
                foreach (Button button in buildingColumn.transform.GetChild(j).GetComponentsInChildren<Button>()) {
                    var cantAfford = false;
                    if (buildingColumn.transform.GetChild(j).name.Contains("Cost")) {
                        var resourceName = buildingColumn.transform.GetChild(j).name.Replace("Cost_", "");
                        if (resourceName == "Gold" || resourceName == "Wood") {
                            var v1 = (Utility.ResourceTypes)Enum.Parse(typeof(Utility.ResourceTypes), resourceName);
                            var owned = Player.Instance.resources.Find(x => x.amount.type == v1).amount.value;
                            var costamount = Array.Find(unit.cost, x => x.type == v1);
                            if (costamount != null) cantAfford = owned < costamount.value;
                        }
                    }

                    button.interactable = unit.minimumBuildingTier <= buildingSlot.building.GetComponent<Structure>().level && !cantAfford;
                }
            }
            if (buildingColumn.transform.GetChild(j).name.Contains("Button_Wrap")) buildingColumn.transform.GetChild(j).GetComponent<Button>().interactable = unit.minimumBuildingTier <= buildingSlot.building.GetComponent<Structure>().level;
        }
        buildingColumn.transform.transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = unit.Icon;
        buildingColumn.transform.GetComponent<BuildingColumn>().currentItemName = unit.unitName;
    }

    private void UpdateUnitCostDisplay(BuildingColumn buildingColumn, OurUnit unit) {
        foreach (Amount cost in unit.cost) {
            var text = buildingColumn.transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
            text.gameObject.SetActive(true);
            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
        }
    }
}
