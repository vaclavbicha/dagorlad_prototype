using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public void ActivateWindow(int location_id, Utility.LocationType location_type, MapLocation mapLocation)
    {
        if (mapLocation.status == Utility.LocationStatus.Built)
        {
            switch (mapLocation.type)
            {
                case Utility.LocationType.Defense:
                    break;
                case Utility.LocationType.Attack:
                    gameObject.SetActive(true);
                    int i = 0;
                    int j = 0;
                    foreach (var obj in GameManager.Instance.units.FindAll(x => x.type == mapLocation.building.GetComponent<Structure>().unitType))
                    {
                        j = 0;
                        foreach (var cost in obj.cost)
                        {
                            var text = transform.GetChild(0).GetChild(i).transform.Find("Cost_" + j.ToString()).GetChild(0);
                            text.gameObject.SetActive(true);
                            text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                            text.GetComponent<UpdateIconText>().UpdateDisplay(cost.value, gameObject);
                            j++;
                        }
                        while (j <= 3)
                        {
                            transform.GetChild(0).GetChild(i).transform.Find("Cost_" + j).GetChild(0).gameObject.SetActive(false);
                            j++;
                        }
                        transform.GetChild(0).GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetChild(0).GetComponent<Image>().sprite = obj.Icon;
                        transform.GetChild(0).GetChild(i).GetComponent<BuildingColumn>().currentBuildingName = obj.unitName;
                        i++;
                    }
                    break;
                case Utility.LocationType.Resource:
                    break;
            }
        }
        else
        {
            gameObject.SetActive(true);
            int i = 0;
            int j = 0;
            foreach (var obj in GameManager.Instance.buildings.FindAll(x => x.locationType == location_type))
            {
                j = 0;
            //Debug.Log("#" + transform.GetChild(0).GetChild(i).transform.name);
            foreach (var cost in obj.cost)
                {
                //Debug.Log("#" + transform.GetChild(0).GetChild(i).transform.Find("Cost_" + j.ToString()));
                var text = transform.GetChild(0).GetChild(i).transform.Find("Cost_" + j.ToString()).GetChild(0);
                    text.gameObject.SetActive(true);
                    text.GetComponent<UpdateIconText>().Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                    text.GetComponent<UpdateIconText>().UpdateDisplay(cost.value, gameObject);
                    j++;
                }
                while (j <= 3)
                {
                    transform.GetChild(0).GetChild(i).transform.Find("Cost_" + j.ToString()).GetChild(0).gameObject.SetActive(false);
                    j++;
                }
                transform.GetChild(0).GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetChild(0).GetComponent<Image>().sprite = obj.Icon;
                transform.GetChild(0).GetChild(i).GetComponent<BuildingColumn>().currentBuildingName = obj.buildingName;
                i++;
            }
        }
    }
}
