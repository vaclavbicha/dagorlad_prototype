using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public GameObject infoPanel;
    GameObject bottomPanelNormal;
    GameObject bottomPanelUtility;

    public enum BottomPanelMode {
        normalPanel,
        utilityPanel
    }

    private void Awake() {
        SetupComponents();
    }

    private void SetupComponents() {
        if (!infoPanel) infoPanel = transform.GetChild(1).gameObject;

        if (!bottomPanelNormal) {
            bottomPanelNormal = transform.GetChild(2).transform.gameObject;
            bottomPanelNormal.SetActive(false);
        }

        if (!bottomPanelUtility) {
            bottomPanelUtility = transform.GetChild(3).gameObject;
            bottomPanelUtility.SetActive(false);
        }
    }

    public void ActivateWindow(int location_id, Utility.BuildingSlotType location_type, BuildingSlot mapLocation)
    {
        GameObject panel = null;

        if (mapLocation.Status == Utility.LocationStatus.Built || mapLocation.Status == Utility.LocationStatus.Training)
        {
            switch (mapLocation.Type)
            {
                case Utility.BuildingSlotType.Defense:
                    break;
                case Utility.BuildingSlotType.Attack:
                    panel = bottomPanelUtility;
                    panel.SetActive(true);

                    gameObject.SetActive(true);

                    int i = 0;

                    List<OurUnit> units = GameManager.Instance.units.FindAll(x => x.type == mapLocation.building.GetComponent<Structure>().unitType);
                    foreach (OurUnit unit in units) {

                        Color fillingColor = GetFillingColor(unit.minimumBuildingTier);

                        foreach (Amount cost in unit.cost) {
                            var text = panel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                            text.gameObject.SetActive(true);
                            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
                            panel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0).GetComponent<Image>().color = fillingColor;

                        }
                        foreach (var resource in GameManager.Instance.resourceSprites) {
                            bool found = false;
                            foreach (var cost in unit.cost) {
                                if (cost.type.ToString() == resource.name) found = true;
                            }
                            if (!found) panel.transform.GetChild(i).transform.Find("Cost_" + resource.name).GetChild(0).gameObject.SetActive(false);
                        }

                        for (int j = 0; j < panel.transform.GetChild(i).childCount; j++) {
                            if (panel.transform.GetChild(i).GetChild(j).name.Contains("Cost") ||
                                panel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap")) {
                                foreach (Button button in panel.transform.GetChild(i).GetChild(j).GetComponentsInChildren<Button>()) {
                                    var cantAfford = false;
                                    if (panel.transform.GetChild(i).GetChild(j).name.Contains("Cost")) {
                                        var resourceName = panel.transform.GetChild(i).GetChild(j).name.Replace("Cost_", "");
                                        if (resourceName == "Gold" || resourceName == "Wood") {
                                            var v1 = (Utility.ResourceTypes)Enum.Parse(typeof(Utility.ResourceTypes), resourceName);
                                            var owned = Player.Instance.resources.Find(x => x.amount.type == v1).amount.value;
                                            var costamount = Array.Find(unit.cost, x => x.type == v1);
                                            if (costamount != null) cantAfford = owned < costamount.value;
                                        }
                                    }

                                    button.interactable = unit.minimumBuildingTier <= mapLocation.building.GetComponent<Structure>().level && !cantAfford;
                                }
                            }
                            if (panel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap")) panel.transform.GetChild(i).GetChild(j).GetComponent<Button>().interactable = unit.minimumBuildingTier > mapLocation.building.GetComponent<Structure>().level ? false : true;
                        }
                        panel.transform.GetChild(i).transform.Find("Button_Wrap").GetComponent<Image>().color = fillingColor;
                        panel.transform.GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = unit.Icon;
                        panel.transform.GetChild(i).GetComponent<BuildingColumn>().currentItemName = unit.unitName;
                        i++;
                    }
                    break;
                case Utility.BuildingSlotType.Resource:
                    panel = bottomPanelNormal;
                    panel.SetActive(true);

                    gameObject.SetActive(true);

                    i = 0;
                    List<ItemUpgrade> upgrades = GameManager.Instance.upgrades.FindAll(x => x.type == mapLocation.building.GetComponent<Structure>().upgradeType);
                    foreach (ItemUpgrade upgrade in upgrades)
                    {
                        Color fillingColor = GetFillingColor(upgrade.minimumBuildingTier);
                        
                        foreach (Amount cost in upgrade.cost)
                        {
                            var text = panel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                            text.gameObject.SetActive(true);
                            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
                            panel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0).GetComponent<Image>().color = fillingColor;

                        }
                        foreach (var resource in GameManager.Instance.resourceSprites)
                        {
                            bool found = false;
                            foreach (var cost in upgrade.cost)
                            {
                                if (cost.type.ToString() == resource.name) found = true;
                            }
                            if (!found) panel.transform.GetChild(i).transform.Find("Cost_" + resource.name).GetChild(0).gameObject.SetActive(false);
                        }
                        //Debug.Log(mapLocation.baseID + " MAP : " + mapLocation.name + mapLocation.building.GetComponent<Structure>().level);
                        for (int j = 0; j < panel.transform.GetChild(i).childCount; j++)
                        {
                            if (panel.transform.GetChild(i).GetChild(j).name.Contains("Cost") ||
                                panel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap"))
                            {
                                foreach (var x in panel.transform.GetChild(i).GetChild(j).GetComponentsInChildren<Button>())
                                {
                                    var cantAfford = false;
                                    if (panel.transform.GetChild(i).GetChild(j).name.Contains("Cost"))
                                    {
                                        var resourceName = panel.transform.GetChild(i).GetChild(j).name.Replace("Cost_", "");
                                        if (resourceName == "Gold" || resourceName == "Wood")
                                        {
                                            var v1 = (Utility.ResourceTypes)Enum.Parse(typeof(Utility.ResourceTypes), resourceName);
                                            var owned = Player.Instance.resources.Find(x => x.amount.type == v1).amount.value;
                                            var costamount = Array.Find(upgrade.cost, x => x.type == v1);
                                            if (costamount != null) cantAfford = owned < costamount.value;
                                        }
                                    }

                                    x.interactable = upgrade.minimumBuildingTier <= mapLocation.building.GetComponent<Structure>().level && !cantAfford;
                                }
                            }
                            if (panel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap")) panel.transform.GetChild(i).GetChild(j).GetComponent<Button>().interactable = upgrade.minimumBuildingTier > mapLocation.building.GetComponent<Structure>().level ? false : true;
                        }
                        panel.transform.GetChild(i).transform.Find("Button_Wrap").GetComponent<Image>().color = fillingColor;
                        panel.transform.GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = upgrade.Icon;
                        panel.transform.GetChild(i).GetComponent<BuildingColumn>().currentItemName = upgrade.upgradeName;
                        i++;
                    }
                    break;
            }
        }
        if (mapLocation.Status == Utility.LocationStatus.Free)
        {
            panel = bottomPanelNormal;
            panel.SetActive(true);

            gameObject.SetActive(true);
            int i = 0;
            foreach (var obj in GameManager.Instance.buildings.FindAll(x => x.locationType == location_type))
            {
                Color fillingColor;
                ColorUtility.TryParseHtmlString("#6D7779", out fillingColor);
                foreach (Amount cost in obj.cost)
                {
                    var text = panel.transform.GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                    text.GetComponent<Image>().color = fillingColor;
                    text.gameObject.SetActive(true);
                    text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                    text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);

                }
                for (int j = 0; j < panel.transform.GetChild(i).childCount; j++)
                {
                    if (panel.transform.GetChild(i).GetChild(j).name.Contains("Cost") ||
                        panel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap"))
                    {
                        foreach (var x in panel.transform.GetChild(i).GetChild(j).GetComponentsInChildren<Button>())
                        {
                            var cantAfford1 = false;
                            if (panel.transform.GetChild(i).GetChild(j).name.Contains("Cost"))
                            {
                                var resourceName = panel.transform.GetChild(i).GetChild(j).name.Replace("Cost_", "");
                                if (resourceName == "Gold" || resourceName == "Wood")
                                {
                                    var v1 = (Utility.ResourceTypes)Enum.Parse(typeof(Utility.ResourceTypes), resourceName);
                                    var owned = Player.Instance.resources.Find(x => x.amount.type == v1).amount.value;
                                    var costamount = Array.Find(obj.cost, x => x.type == v1);
                                    if(costamount != null) cantAfford1 = owned < costamount.value;
                                }
                            }

                            x.interactable = cantAfford1 ? false : true;
                        }
                    }
                    if (panel.transform.GetChild(i).GetChild(j).name.Contains("Button_Wrap")) panel.transform.GetChild(i).GetChild(j).GetComponent<Button>().interactable = true;
                }
                panel.transform.GetChild(i).transform.Find("Button_Wrap").GetComponent<Image>().color = fillingColor;

                foreach (var resource in GameManager.Instance.resourceSprites)
                {
                    bool found = false;
                    foreach (Amount cost in obj.cost)
                    {
                        if (cost.type.ToString() == resource.name) found = true;
                    }
                    if (!found) panel.transform.GetChild(i).transform.Find("Cost_" + resource.name).GetChild(0).gameObject.SetActive(false);
                }
                panel.transform.GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = obj.Icon;
                panel.transform.GetChild(i).GetComponent<BuildingColumn>().currentItemName = obj.buildingName;
                i++;
            }
        }
    }

    public void DeactivateWindow() {
        bottomPanelNormal.SetActive(false);
        bottomPanelUtility.SetActive(false);
        gameObject.SetActive(false);
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
