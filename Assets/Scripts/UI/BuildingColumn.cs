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

    public void Deactivate() {
        switch (buildingColumnMode) {
            case BuildingColumnMode.normal:
                SupplyCostRow.Deactivate();
                GoldCostRow.Deactivate();
                WoodCostRow.Deactivate();
                TimeCostRow.Deactivate();
                break;

            case BuildingColumnMode.utility:
                GoldCostRow.Deactivate();
                WoodCostRow.Deactivate();
                TimeCostRow.Deactivate();
                break;
        }

        gameObject.SetActive(false);
    }

    public void SetBuildingColumn(Structure building) {
        Amount[] costs = building.cost;
        Sprite buildingIcon = building.Icon;
        string buildingName = building.buildingName;

        SetCosts(costs);
        SetIcon(buildingIcon);
        SetName(buildingName);

        gameObject.SetActive(true);
    }

    public void SetUnitColumn(OurUnit unit) {
        Amount[] costs = unit.cost;
        Sprite unitIcon = unit.Icon;
        string unitName = unit.unitName;

        SetCosts(costs);
        SetIcon(unitIcon);
        SetName(unitName);

        //ResourceManager.Instance.onResourcesUpdated += TurnOffInfoIcon;

        gameObject.SetActive(true);
    }

    public void SetDemolishColumn() {
        Amount[] costs = ResourceManager.Instance.GetBuildingDemolishCost();
        SetCosts(costs);

        gameObject.SetActive(true);
    }

    public void SetUpgradeColumn() {
        //Amount[] costs = unit.cost;
        //Sprite unitIcon = unit.Icon;
        //string unitName = unit.unitName;

        //SetCosts(costs);
        //SetIcon(unitIcon);
        //SetName(unitName);

        gameObject.SetActive(true);
    }

    private void SetCosts(Amount[] costs) {
        foreach (Amount cost in costs) {
            Debug.Log(cost.value);
            switch (cost.type) {
                case Utility.ResourceTypes.Supply:
                    SupplyCostRow.SetActive(cost.value);
                    SupplyCostRow.SetUnableToBuy();
                    break;
                case Utility.ResourceTypes.Gold:
                    GoldCostRow.SetActive(cost.value);
                    GoldCostRow.SetUnableToBuy();
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
