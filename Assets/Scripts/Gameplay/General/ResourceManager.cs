using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;

    public List<Sprite> resourceSprites = new();

    [SerializeField]
    public SupplyResource SupplyResource;

    [SerializeField]
    public Resource GoldResource;

    [SerializeField]
    public Resource WoodResource;

    [SerializeField]
    TimeResource timeResource;

    float sceneStartTime;


    [SerializeField]
    public Amount[] startingAmounts = new[] {
        new Amount(Utility.ResourceTypes.Supply, 500),
        new Amount(Utility.ResourceTypes.Gold, 500),
        new Amount(Utility.ResourceTypes.Wood, 500),
        new Amount(Utility.ResourceTypes.Time, 0)   
    };

    public List<Resource> resources;

    [SerializeField]
    Amount[] BuildingDestructionCost;

    public List<ItemUpgrade> ownedUpgrades = new();

    public delegate void OnResourcesUpdated();
    public OnResourcesUpdated onResourcesUpdated;

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    private void Start() {
        sceneStartTime = Time.time;
        SetStartingTime();
        SetStartingResourcesAmount();

        SubscribeToResourceUpdates();
    }

    private void OnDestroy() {
        UnsubscribeToResourceUpdates();
    }

    private void Update() {
        UpdateInRealTime();
    }

    public void UpdateInRealTime() {
        timeResource.SetValue(Mathf.FloorToInt(Time.time - sceneStartTime));
    }

    public int GetGameTime() {
        return timeResource.GetValue();
    }


    private void SetStartingTime() {
        timeResource.SetValue(0);
    }

    public Amount[] GetBuildingDemolishCost() {
        return BuildingDestructionCost;
    }

    private void SetStartingResourcesAmount() {
        foreach (Amount amount in startingAmounts) {
            switch (amount.type) {
                case Utility.ResourceTypes.Supply:
                    SupplyResource.SetStartingValue(amount.value);
                    break;
                case Utility.ResourceTypes.Gold:
                    GoldResource.SetStartingValue(amount.value);
                    break;
                case Utility.ResourceTypes.Wood:
                    WoodResource.SetStartingValue(amount.value);
                    break;
                case Utility.ResourceTypes.Time:
                    timeResource.SetValue(amount.value);
                    break;
            }
        }
    }

    public bool HasEnoughSupplies(int value) {
        int maxAmount = SupplyResource.GetMaxValue();
        int currentAmount = SupplyResource.GetCurrentValue();
        return currentAmount + value <= maxAmount;
    }

    public bool HasEnoughGold(int value) {
        return GoldResource.GetValue() >= value;
    }

    public bool HasEnoughWood(int value) {
        return WoodResource.GetValue() >= value;
    }

    void SubscribeToResourceUpdates() {
        SupplyResource.On_AmountUpdate += OnResourceUpdated;
        GoldResource.On_AmountUpdate += OnResourceUpdated;
        WoodResource.On_AmountUpdate += OnResourceUpdated;
    }

    public void UnsubscribeToResourceUpdates() {
        SupplyResource.On_AmountUpdate -= OnResourceUpdated;
        GoldResource.On_AmountUpdate -= OnResourceUpdated;
        WoodResource.On_AmountUpdate -= OnResourceUpdated;
    }

    void OnResourceUpdated(int _value) {
        onResourcesUpdated?.Invoke();
    }

    public bool CanAfford(Amount[] prices) {
        bool canAfford = true;

        foreach (Amount amount in prices) {
            switch (amount.type) {
                case Utility.ResourceTypes.Supply:
                    if (!HasEnoughSupplies(amount.value)) return false;
                    break;
            case Utility.ResourceTypes.Gold:
                    if (!HasEnoughGold(amount.value)) return false;
                    break;
                case Utility.ResourceTypes.Wood:
                    if (!HasEnoughWood(amount.value)) return false;
                    break;
            }
        }

        return canAfford;
    }

    public void Buy(Amount[] prices) {
        foreach (Amount amount in prices) {
            switch (amount.type) {
                case Utility.ResourceTypes.Gold:
                    GoldResource.DeductValue(amount.value);
                    break;
                case Utility.ResourceTypes.Wood:
                    WoodResource.DeductValue(amount.value);
                    break;
            }
        }
    }

    public void Refund(Amount[] prices) {
        foreach (Amount amount in prices) {
            switch (amount.type) {
                case Utility.ResourceTypes.Gold:
                    GoldResource.AddValue(amount.value);
                    break;
                case Utility.ResourceTypes.Wood:
                    WoodResource.AddValue(amount.value);
                    break;
            }
        }
    }

    public void BuyUnit(Amount price) {
        SupplyResource.AddCurrentValue(price.value);
    }

    public void PayDemolishPrice() {
        Buy(BuildingDestructionCost);
    }

    public void RefundUnitCost(Amount[] prices) {
        foreach (Amount amount in prices) {
            switch (amount.type) {
                case Utility.ResourceTypes.Gold:
                    GoldResource.AddValue(amount.value);
                    break;
                case Utility.ResourceTypes.Wood:
                    WoodResource.AddValue(amount.value);
                    break;
            }
        }
    }

    public void RefundUnitSupply(Amount price) {
        SupplyResource.DeductCurrentValue(price.value);
    }

    public void AddUpgrade(ItemUpgrade upgrade) {
        ownedUpgrades.Add(upgrade);
    }

    public void RemoveUpgrade(ItemUpgrade upgrade) {
        ownedUpgrades.Remove(upgrade);
    }
}