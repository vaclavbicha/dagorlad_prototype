using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    //public Timer productionTimer;

    //public int resourceCicleTime;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    public void UpdateInRealTime() {
        timeResource.AmountUpdateWithText(Mathf.FloorToInt(Time.time - sceneStartTime));
    }

    public int GetGameTime() {
        return timeResource.GetValue();
    }

    private void Start()
    {
        sceneStartTime = Time.time;
        // temporary
        resources = new List<Resource> { SupplyResource, GoldResource, WoodResource };
            SetStartingTime();
            SetStartingResourcesAmount();
    }

    private void Update() {
        UpdateInRealTime();
    }

    private void SetStartingTime() {
        timeResource.SetValue(0);
    }

    private void SetStartingResourcesAmount()
    {
        Debug.Log(startingAmounts.Length);
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

    //public List<MapLocation> locations = new List<MapLocation>();

    //public List<Resource> resources = new();
    //


    //public delegate void OnResourcesUpdated();
    //public OnResourcesUpdated onResourcesUpdated;

    //private void Start() {
    //    foreach (Resource resource in resources) {
    //        resource.On_AmountUpdate += OnResourceUpdated;
    //    }
    //}

    //void OnResourceUpdated(int _value, GameObject _sender) {
    //    Debug.Log("OnResourceUpdated");
    //    onResourcesUpdated?.Invoke();
    //}
    //IEnumerator NewTimer() {
    //    yield return new WaitForSeconds(0);
    //    productionTimer = gameObject.AddComponent<Timer>();
    //    productionTimer.AddTimer("Produce", resourceCicleTime, true);
    //    productionTimer.On_Duration_End += DistributeResources;
    //}
    //public bool Refund(Amount[] price) {
    //    var priceaux = new List<Amount>();
    //    foreach (var x in price) {
    //        if (x.type != Utility.ResourceTypes.Time) priceaux.Add(x);
    //    }
    //    price = priceaux.ToArray();
    //    foreach (var y in price) {
    //        if (y.type != Utility.ResourceTypes.Supply) {
    //            resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(y.value);
    //        } else {
    //            currentSupply -= y.value;
    //            resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(0);
    //        }
    //    }
    //    return true;
    //}
    //public bool Buy(Amount[] price) {
    //    var priceaux = new List<Amount>();
    //    foreach (var x in price) {
    //        if (x.type != Utility.ResourceTypes.Time) priceaux.Add(x);
    //    }
    //    price = priceaux.ToArray();
    //    foreach (var y in price) {
    //        if (y.type != Utility.ResourceTypes.Supply) {
    //            var owned = resources.Find(x => x.amount.type == y.type).amount.value;
    //            if (owned < y.value) return false;
    //        } else {
    //            if ((resources.Find(x => x.amount.type == y.type).amount.value - currentSupply) < y.value) return false;
    //        }
    //    }
    //    foreach (var y in price) {
    //        if (y.type != Utility.ResourceTypes.Supply) {
    //            resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(-y.value);
    //        } else {
    //            currentSupply += y.value;
    //            resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(0);
    //        }
    //    }
    //    return true;
    //}

    //[System.Obsolete]
    //public void DistributeResources(Timer timer) {
    //    foreach (var x in GameManager.Instance.ALL_Locations.FindAll(y => y.Type == Utility.BuildingSlotType.Resource && y.owner == this && (y.Status == Utility.LocationStatus.Built || y.Status == Utility.LocationStatus.Training))) {
    //        if (x.building.GetComponent<Structure>().production.type != Utility.ResourceTypes.Supply) {
    //            var aux = ownedUpgrades.FindAll(z => z.effect.resourceAmount.type == x.building.GetComponent<Structure>().production.type && z.gameObject.active == true);
    //            var totalValueGained = aux.Count == 0 ? x.building.GetComponent<Structure>().production.value : x.building.GetComponent<Structure>().production.value + aux.Count * aux[0].effect.resourceAmount.value;
    //            var money = x.building.GetComponent<Structure>().production.value + aux.Count;
    //            switch (x.building.GetComponent<Structure>().production.type) {
    //                case Utility.ResourceTypes.Gold:
    //                    GoldResource.AddValue(money);
    //                    break;
    //                case Utility.ResourceTypes.Wood:
    //                    WoodResource.AddValue(money);
    //                    break;
    //            }
    //        }
    //    }
    //    Destroy(productionTimer);
    //    StartCoroutine(NewTimer());
    //}
}
