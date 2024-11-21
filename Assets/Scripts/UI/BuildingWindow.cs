using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BuildingWindowBottomPanel;

public class BuildingWindow : MonoBehaviour
{
    public GameObject infoPanel;
    GameObject bottomPanelNormal;
    GameObject bottomPanelUtility;
    BuildingWindowBottomPanel currentBottomPanel;

    private void Awake() {
        SetupComponents();
    }

    private void SetupComponents() {
        if (!infoPanel) infoPanel = transform.GetChild(1).gameObject;

        if (!bottomPanelNormal) {
            bottomPanelNormal = transform.GetChild(2).transform.gameObject;
            bottomPanelNormal.GetComponent<BuildingWindowBottomPanel>().SetupColumns();
            bottomPanelNormal.SetActive(false);
        }

        if (!bottomPanelUtility) {
            bottomPanelUtility = transform.GetChild(3).transform.gameObject;
            bottomPanelUtility.GetComponent<BuildingWindowBottomPanel>().SetupColumns();
            bottomPanelUtility.SetActive(false);
        }
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

                        Color fillingColor = GetFillingColor(unit.minimumBuildingTier);
                        UpdateUnitCostDisplay(normalBuildingColumns[j], unit, fillingColor);
                        UpdateUnitResourceAndButtonInteractivity(buildingSlot, normalBuildingColumns[j], unit, fillingColor);
                    }

                    
                    break;
                case Utility.BuildingSlotType.Resource:
                    currentBottomPanel = OpenPanel(BottomPanelMode.normalPanel).GetComponent<BuildingWindowBottomPanel>();

                    int i = 0;
                    List<ItemUpgrade> upgrades = GameManager.Instance.upgrades.FindAll(x => x.type == buildingSlot.building.GetComponent<Structure>().upgradeType);
                    foreach (ItemUpgrade upgrade in upgrades)
                    {
                        Color fillingColor = GetFillingColor(upgrade.minimumBuildingTier);
                        
                        foreach (Amount cost in upgrade.cost)
                        {
                            var text = currentBottomPanel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                            text.gameObject.SetActive(true);
                            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
                            currentBottomPanel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0).GetComponent<Image>().color = fillingColor;

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
                        currentBottomPanel.transform.GetChild(i).transform.Find("Button_Wrap").GetComponent<Image>().color = fillingColor;
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
                ColorUtility.TryParseHtmlString("#6D7779", out Color fillingColor);
                foreach (Amount cost in obj.cost)
                {
                    var text = currentBottomPanel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                    text.GetComponent<Image>().color = fillingColor;
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
                currentBottomPanel.transform.GetChild(i).transform.Find("Button_Wrap").GetComponent<Image>().color = fillingColor;

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
        GameObject panel = null;

        switch (bottomPanelMode) {
            case BottomPanelMode.normalPanel:
                panel = bottomPanelNormal;
                break;
            case BottomPanelMode.utilityPanel:
                panel = bottomPanelUtility;
                break;
        }

        panel.SetActive(true);
        gameObject.SetActive(true);
        return panel;
    }

    private static void UpdateUnitResourceAndButtonInteractivity(BuildingSlot buildingSlot, BuildingColumn buildingColumn, OurUnit unit, Color fillingColor) {
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
        buildingColumn.transform.transform.Find("Button_Wrap").GetComponent<Image>().color = fillingColor;
        buildingColumn.transform.transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = unit.Icon;
        buildingColumn.transform.GetComponent<BuildingColumn>().currentItemName = unit.unitName;
    }

    private void UpdateUnitCostDisplay(BuildingColumn buildingColumn, OurUnit unit, Color fillingColor) {
        foreach (Amount cost in unit.cost) {
            var text = buildingColumn.transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
            text.gameObject.SetActive(true);
            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
            buildingColumn.transform.Find("Cost_" + cost.type.ToString()).GetChild(0).GetComponent<Image>().color = fillingColor;
        }
    }

    private static Color GetFillingColor(int minimumBuildingTier) {
        Color fillingColor = new(0, 0, 0);

        switch (minimumBuildingTier) {
            case 0:
                ColorUtility.TryParseHtmlString("#6D7779", out fillingColor);
                break;
            case 1:
                ColorUtility.TryParseHtmlString("#7ECFEC", out fillingColor);
                break;
            case 2:
                ColorUtility.TryParseHtmlString("#ECB136", out fillingColor);
                break;
        }

        return fillingColor;
    }
}
