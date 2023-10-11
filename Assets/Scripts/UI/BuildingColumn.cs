using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingColumn : MonoBehaviour
{
    public UpdateIconText[] array;
    public string currentItemName = "default";
    Image infoImage = null;
    public void Start()
    {
        infoImage = transform.parent.GetComponentInParent<BuildingWindow>().infoPanel.GetComponent<Image>();
    }
    public void OnBuy()
    {
        UIManager.Instance.OnItemBuy(currentItemName);
    }
    public void OnInfoToggle(Toggle change)
    {
        if (infoImage)
        {
            if (change.isOn)
            {
                infoImage.gameObject.SetActive(true);
                infoImage.sprite = GameManager.Instance.infoSprites.Find(sprite => sprite.name == currentItemName);
                UIManager.Instance.lastSelectedInfoToggle = change;
            }
            else
            {
                infoImage.gameObject.SetActive(false);
                UIManager.Instance.lastSelectedInfoToggle = null;
            }
        }
        else
        {
            Debug.Log("CANNOT FIND INFO IMAGE");
        }
    }
}
