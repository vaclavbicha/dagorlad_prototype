using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingWindow : MonoBehaviour
{
    public void ActivateWindow(int location_id, string location_type)
    {
        gameObject.SetActive(true);
        int i = 0;
        int j = 0;
        foreach(var obj in GameManager.Instance.buildings.FindAll(x => x.locationType == (Utility.LocationType)System.Enum.Parse(typeof(Utility.LocationType), location_type)))
        {
            j = 0;
            foreach(var cost in obj.cost)
            {
                var text = transform.GetChild(0).GetChild(i).transform.Find("Cost_" + j).GetComponentInChildren<UpdateIconText>();
                text.gameObject.SetActive(true);
                text.Icon.sprite = GameManager.Instance.resourceSprites.Find(sprite => sprite.name == cost.type.ToString());
                text.UpdateDisplay(cost.value, gameObject);
                j++;
            }
            while(j <= 3)
            {
                transform.GetChild(0).GetChild(i).transform.Find("Cost_" + j).GetChild(0).gameObject.SetActive(false);
                j++;
            }
            transform.GetChild(0).GetChild(i).transform.Find("Button_Wrap").GetChild(0).GetChild(0).GetComponent<Image>().sprite = obj.Icon;
            i++;
        }
    }
}
