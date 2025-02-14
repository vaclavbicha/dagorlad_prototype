using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ResourceManager : MonoBehaviour {
    public static ResourceManager Instance;

    public List<Sprite> resourceSprites = new();

    [SerializeField]
    public Resource SupplyResource;

    public int currentSupply;

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
        new Amount(Utility.ResourceTypes.Wood, 500)
    };

    public List<Resource> resources;

    public Amount[] BuildingDestructionCost;

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
        timeResource.AmountUpdateWithText(Mathf.FloorToInt(Time.time - sceneStartTime));
    }

    public int GetGameTime() {
        return timeResource.GetValue();
    }

    private void SetStartingTime() {
        timeResource.SetValue(0);
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
            }
        }
    }

    public bool HasEnoughSupplies(int value) {
        return SupplyResource.GetValue() >= value;
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
        bool canAfford = false;

        foreach (Amount amount in prices) {
            switch (amount.type) {
                // currentValue
                //case Utility.ResourceTypes.Supply:
                //    canAfford = SupplyResource.GetValue() >= amount.value;
                //    break;
                case Utility.ResourceTypes.Gold:
                    canAfford = GoldResource.GetValue() >= amount.value;
                    break;
                case Utility.ResourceTypes.Wood:
                    canAfford = WoodResource.GetValue() >= amount.value;
                    break;
            }
        }

        return canAfford;
    }

    public void Buy(Amount[] prices) {
        foreach (Amount amount in prices) {
            switch (amount.type) {
                // currentValue
                //case Utility.ResourceTypes.Supply:
                //    canAfford = SupplyResource.GetValue() >= amount.value;
                //    break;
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
                // currentValue
                //case Utility.ResourceTypes.Supply:
                //    canAfford = SupplyResource.GetValue() >= amount.value;
                //    break;
                case Utility.ResourceTypes.Gold:
                    GoldResource.AddValue(amount.value);
                    break;
                case Utility.ResourceTypes.Wood:
                    WoodResource.AddValue(amount.value);
                    break;
            }
        }
    }

    public void AddUpgrade(ItemUpgrade upgrade) {
        ownedUpgrades.Add(upgrade);
    }

    public void RemoveUpgrade(ItemUpgrade upgrade) {
        ownedUpgrades.Remove(upgrade);
    }
}
        //var priceaux = new List<Amount>();
        //foreach (var x in price) {
        //    if (x.type != Utility.ResourceTypes.Time) priceaux.Add(x);
        //}
        //price = priceaux.ToArray();
        //foreach (var resource in price) {
        //    if (resource.type != Utility.ResourceTypes.Supply) {
        //        var ownedSupplies = SupplyResource.GetValue();
        //        if (ownedSupplies < resource.value) return false;
        //    } else {
        //        if ((resources.Find(x => x.amount.type == y.type).amount.value - currentSupply) < y.value) return false;
        //    }
        //}
        //foreach (var y in price) {
        //    if (y.type != Utility.ResourceTypes.Supply) {
        //        resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(-y.value);
        //    } else {
        //        currentSupply += y.value;
        //        resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(0);
        //    }
        //}
        //return true;

