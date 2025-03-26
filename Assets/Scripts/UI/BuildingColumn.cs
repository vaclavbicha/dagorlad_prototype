using UnityEngine;
using UnityEngine.UI;

public class BuildingColumn : MonoBehaviour {
    public string currentItemName = "default";

    [SerializeField]
    public BuildingColumnMode buildingColumnMode;

    public enum BuildingColumnMode {
        normal,
        utility
    }

    [SerializeField]
    Image infoImage;

    [SerializeField]
    Transform infoToggle;

    [SerializeField]
    Toggle infoButtonToggle;

    [SerializeField]
    Transform SpriteWrap;

    [SerializeField]
    CostRow SupplyCostRow;

    [SerializeField]
    CostRow GoldCostRow;

    [SerializeField]
    CostRow WoodCostRow;

    [SerializeField]
    CostRow TimeCostRow;

    UpdateIconText[] GetComponentsInChildren() {
        return GetComponentsInChildren<UpdateIconText>();
    }


    public void SetupColumn()
    {
        //infoImage = transform.parent.GetComponentInParent<BuildingWindow>().infoPanel.GetComponent<Image>();
        //infoToggle = transform.Find("Info_Wrap").Find("Toggle_Button").transform;
        //infoButtonToggle = infoToggle.GetComponent<Toggle>();

        //if (buildingColumnMode == BuildingColumnMode.normal) {
        //    SupplyCostRow = transform.Find("Cost_Supply").GetComponent<CostRow>();
        //    GoldCostRow = transform.Find("Cost_Gold").GetComponent<CostRow>();
        //    WoodCostRow = transform.Find("Cost_Wood").GetComponent<CostRow>();
        //    TimeCostRow = transform.Find("Cost_Time").GetComponent<CostRow>();
        //}
    }

    public void Deactivate() {
        switch (buildingColumnMode) {
            case BuildingColumnMode.normal:
                SupplyCostRow.Deactivate();
                GoldCostRow.Deactivate();
                WoodCostRow.Deactivate();
                TimeCostRow.Deactivate();
                break;

            case BuildingColumnMode.utility:

                break;
        }

        gameObject.SetActive(false);
    }

    public void SetActive(Structure building) {
        Amount[] costs = building.cost;
        Sprite buildingIcon = building.Icon;
        string buildingName = building.buildingName;

        SetCosts(costs);
        SetIcon(buildingIcon);
        SetName(buildingName);

        gameObject.SetActive(true);
    }

    private void SetCosts(Amount[] costs) {
        foreach (Amount cost in costs) {
            switch (cost.type) {
                case Utility.ResourceTypes.Supply:
                    SupplyCostRow.SetActive(cost.value);
                    break;
                case Utility.ResourceTypes.Gold:
                    GoldCostRow.SetActive(cost.value);
                    Debug.Log(cost.value);
                    break;
                case Utility.ResourceTypes.Wood:
                    WoodCostRow.SetActive(cost.value);
                    break;
                case Utility.ResourceTypes.Time:
                    TimeCostRow.SetActive(cost.value);
                    break;
            }
        }
    }

    private void SetIcon(Sprite sprite) {
        SpriteWrap.GetComponent<Image>().sprite = sprite;
    }

    private void SetName(string name) {
        currentItemName = name;
    }

    public void OnBuy()
    {
        UIManager.Instance.OnItemBuy(currentItemName);
    }
    public void OnInfoToggle(Toggle change)
    {
        if (!infoImage) Debug.Log("CANNOT FIND INFO IMAGE");

        if (change.isOn) {
            infoImage.gameObject.SetActive(true);
            infoImage.sprite = GameManager.Instance.infoSprites.Find(sprite => sprite.name == currentItemName);
            UIManager.Instance.lastSelectedInfoToggle = change;
        } else {
            infoImage.gameObject.SetActive(false);
            UIManager.Instance.lastSelectedInfoToggle = null;
        }
    }

    public void TurnOffInfoIcon() {
        infoButtonToggle.isOn = false;
    }
}
