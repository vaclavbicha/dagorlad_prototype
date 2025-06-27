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

    [SerializeField]
    Button buttonWrap;

    [SerializeField]
    Button buttonIcon;

    private int supplyCost;
    private int goldCost;
    private int woodCost;
    private int timeCost;

    public int SupplyCost {
        get => supplyCost;
        set {
            supplyCost = value;
            SupplyCostRow.SetActive(value);
        }
    }

    public int GoldCost {
        get => goldCost;
        set {
            goldCost = value;
            GoldCostRow.SetActive(value);
        }
    }
    
    public int WoodCost {
        get => woodCost;
        set {
            woodCost = value;
            WoodCostRow.SetActive(value);
        }
    }

    public int TimeCost {
        get => timeCost;
        set {
            timeCost = value;
            TimeCostRow.SetActive(value);
        }
    }

    public void Deactivate() {
        switch (buildingColumnMode) {
            case BuildingColumnMode.normal:
                ResourceManager.Instance.onResourcesUpdated -= UpdateAffordability;

                SupplyCostRow.Deactivate();
                GoldCostRow.Deactivate();
                WoodCostRow.Deactivate();
                TimeCostRow.Deactivate();
                break;

            case BuildingColumnMode.utility:
                ResourceManager.Instance.onResourcesUpdated -= UpdateAffordability;

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

        ResourceManager.Instance.onResourcesUpdated += UpdateAffordability;

        gameObject.SetActive(true);
    }

    public void SetUnitColumn(OurUnit unit) {
        Amount[] costs = unit.cost;
        Sprite unitIcon = unit.Icon;
        string unitName = unit.unitName;

        SetCosts(costs);
        SetIcon(unitIcon);
        SetName(unitName);

        ResourceManager.Instance.onResourcesUpdated += UpdateAffordability;

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
                    SupplyCost = cost.value;
                    break;
                case Utility.ResourceTypes.Gold:
                    GoldCost = cost.value;
                    break;
                case Utility.ResourceTypes.Wood:
                    WoodCost = cost.value;
                    break;
                case Utility.ResourceTypes.Time:
                    TimeCost = cost.value;
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

    public void DisableColumn() {
        buttonWrap.interactable = false;
        buttonIcon.interactable = false;
    }

    public void EnableColumn() {
        buttonWrap.interactable = true;
        buttonIcon.interactable = true;
    }

    public void UpdateAffordability() {
        bool canAfford = true;

        if (ResourceManager.Instance.HasEnoughSupplies(supplyCost)) {
            SupplyCostRow.SetAbleToBuy();
        } else {
            SupplyCostRow.SetUnableToBuy();
            canAfford = false;
        }

        if (ResourceManager.Instance.HasEnoughGold(goldCost)) {
            GoldCostRow.SetAbleToBuy(); 
        } else {
            GoldCostRow.SetUnableToBuy();
            canAfford = false;
        }

        if (ResourceManager.Instance.HasEnoughWood(woodCost)) {
            WoodCostRow.SetAbleToBuy();
        } else {
            WoodCostRow.SetUnableToBuy();
            canAfford = false;
        }

        if (canAfford) {
            EnableColumn();
        } else {
            DisableColumn();
        }
    }
}
