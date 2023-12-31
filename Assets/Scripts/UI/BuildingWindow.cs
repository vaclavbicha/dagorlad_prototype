using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public GameObject infoPanel;
    public void ActivateWindow(int location_id, Utility.LocationType location_type, MapLocation mapLocation)
    {
        if (mapLocation.status == Utility.LocationStatus.Built || mapLocation.status == Utility.LocationStatus.Training)
        {
            int i = 0;
            int j = 0;
            switch (mapLocation.type)
            {
                case Utility.LocationType.Defense:
                    break;
                case Utility.LocationType.Attack:
                    gameObject.SetActive(true);

                    foreach (var obj in GameManager.Instance.units.FindAll(x => x.type == mapLocation.building.GetComponent<Structure>().unitType))
                    {
                        //j = 0;
                        Color fillingColor = new Color(0, 0, 0);
                        switch (obj.minimumBuildingTier)
                        {
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
                        foreach (var cost in obj.cost)
                        {
                            var text = transform.GetChild(2).GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                            text.gameObject.SetActive(true);
                            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                            text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
                            transform.GetChild(2).GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0).GetComponent<Image>().color = fillingColor;
                            //j++;
                        }
                        //while (j <= 3)
                        //{
                        //    transform.GetChild(2).GetChild(i).transform.Find("Cost_" + j).GetChild(0).gameObject.SetActive(false);
                        //    j++;
                        //}
                        foreach (var resource in GameManager.Instance.resourceSprites)
                        {
                            bool found = false;
                            foreach (var cost in obj.cost)
                            {
                                if (cost.type.ToString() == resource.name) found = true;
                            }
                            if (!found) transform.GetChild(2).GetChild(i).transform.Find("Cost_" + resource.name).GetChild(0).gameObject.SetActive(false);
                        }
                        //Debug.Log(mapLocation.baseID + " MAP : " + mapLocation.name + mapLocation.building.GetComponent<Structure>().level);
                        for (int ii = 0; ii < transform.GetChild(2).GetChild(i).childCount; ii++)
                        {
                            if (transform.GetChild(2).GetChild(i).GetChild(ii).name.Contains("Cost") ||
                                transform.GetChild(2).GetChild(i).GetChild(ii).name.Contains("Button_Wrap"))
                            {
                                foreach (var x in transform.GetChild(2).GetChild(i).GetChild(ii).GetComponentsInChildren<Button>())
                                {
                                    x.interactable = obj.minimumBuildingTier > mapLocation.building.GetComponent<Structure>().level ? false : true;
                                }
                            }
                            if (transform.GetChild(2).GetChild(i).GetChild(ii).name.Contains("Button_Wrap")) transform.GetChild(2).GetChild(i).GetChild(ii).GetComponent<Button>().interactable = obj.minimumBuildingTier > mapLocation.building.GetComponent<Structure>().level ? false : true;
                        }
                        transform.GetChild(2).GetChild(i).transform.Find("Button_Wrap").GetComponent<Image>().color = fillingColor;
                        transform.GetChild(2).GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = obj.Icon;
                        transform.GetChild(2).GetChild(i).GetComponent<BuildingColumn>().currentItemName = obj.unitName;
                        i++;
                    }
                    break;
                case Utility.LocationType.Resource:
                    gameObject.SetActive(true);
                    foreach (var obj in GameManager.Instance.upgrades.FindAll(x => x.type == mapLocation.building.GetComponent<Structure>().upgradeType))
                    {
                        j = 0;
                        foreach (var cost in obj.cost)
                        {
                            var text = transform.GetChild(2).GetChild(i).transform.Find("Cost_" + j.ToString()).GetChild(0);
                            text.gameObject.SetActive(true);
                            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                            text.GetComponent<UpdateIconText>().UpdateDisplay(cost.value, gameObject);
                            j++;
                        }
                        while (j <= 3)
                        {
                            transform.GetChild(2).GetChild(i).transform.Find("Cost_" + j).GetChild(0).gameObject.SetActive(false);
                            j++;
                        }
                        transform.GetChild(2).GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = obj.Icon;
                        transform.GetChild(2).GetChild(i).GetComponent<BuildingColumn>().currentItemName = obj.upgradeName;
                        i++;
                    }
                    break;
            }
        }
        if (mapLocation.status == Utility.LocationStatus.Free)
        {
            gameObject.SetActive(true);
            int i = 0;
            int j = 0;
            foreach (var obj in GameManager.Instance.buildings.FindAll(x => x.locationType == location_type))
            {
                //j = 0;
                //Debug.Log("#" + transform.GetChild(2).GetChild(i).transform.name);
                foreach (var cost in obj.cost)
                {
                    //Debug.Log("#" + transform.GetChild(2).GetChild(i).transform.Find("Cost_" + j.ToString()));
                    var text = transform.GetChild(2).GetChild(i).transform.Find("Cost_" + cost.type.ToString()).GetChild(0);
                    text.gameObject.SetActive(true);
                    text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                    text.GetComponent<UpdateIconText>().UpdateText(cost.GetValueText(), gameObject);
                    //j++;
                }
                //while (j <= 3)
                //{
                //    transform.GetChild(2).GetChild(i).transform.Find("Cost_" + j.ToString()).GetChild(0).gameObject.SetActive(false);
                //    j++;
                //}
                foreach (var resource in GameManager.Instance.resourceSprites)
                {
                    bool found = false;
                    foreach (var cost in obj.cost)
                    {
                        if (cost.type.ToString() == resource.name) found = true;
                    }
                    if(!found) transform.GetChild(2).GetChild(i).transform.Find("Cost_" + resource.name).GetChild(0).gameObject.SetActive(false);
                }
                transform.GetChild(2).GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetComponent<Image>().sprite = obj.Icon;
                transform.GetChild(2).GetChild(i).GetComponent<BuildingColumn>().currentItemName = obj.buildingName;
                i++;
            }
        }
    }
}
