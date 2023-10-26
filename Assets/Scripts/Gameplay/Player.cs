using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public string PlayerName;

    //public List<MapLocation> locations = new List<MapLocation>();

    public List<Resource> resources = new List<Resource>();
    public int currentSupply;

    public List<ItemUpgrade> ownedUpgrades = new List<ItemUpgrade>();

    public Timer productionTimer;

    public int resourceCicleTime;

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
        resources.AddRange(GetComponents<Resource>());
        StartCoroutine(NewTimer());
    }
    IEnumerator NewTimer()
    {
        yield return new WaitForSeconds(0);
        productionTimer = gameObject.AddComponent<Timer>();
        productionTimer.AddTimer("Produce", resourceCicleTime, true);
        productionTimer.On_Duration_End += DistributeResources;
    }
    public bool Buy(Amount[] price)
    {
        foreach(var y in price)
        {
            if (y.type != Utility.ResourceTypes.Supply)
            {
                var owned = resources.Find(x => x.amount.type == y.type).amount.value;
                if (owned < y.value) return false;
            }
            else
            {
                if ((resources.Find(x => x.amount.type == y.type).amount.value - currentSupply) < y.value) return false;
            }
        }
        foreach (var y in price)
        {
            if (y.type != Utility.ResourceTypes.Supply)
            {
                resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(-y.value);
            }
            else
            {
                currentSupply += y.value;
                resources.Find(x => x.amount.type == y.type).AmountUpdateWithText(0);
            }
        }
        return true;
    }

    [System.Obsolete]
    public void DistributeResources(Timer timer)
    {
        foreach (var x in GameManager.Instance.ALL_Locations.FindAll(y => y.type == Utility.LocationType.Resource && y.owner == this && y.status == Utility.LocationStatus.Built))
        {
            if (x.building.GetComponent<Structure>().production.type != Utility.ResourceTypes.Supply)
            {
                var aux = ownedUpgrades.FindAll(z => z.effect.resourceAmount.type == x.building.GetComponent<Structure>().production.type && z.gameObject.active == true);
                var totalValueGained = aux.Count == 0 ? x.building.GetComponent<Structure>().production.value : x.building.GetComponent<Structure>().production.value + aux.Count * aux[0].effect.resourceAmount.value;
                resources.Find(y => y.amount.type == x.building.GetComponent<Structure>().production.type).AmountUpdateWithText(totalValueGained);
            }
        }
        Destroy(productionTimer);
        StartCoroutine(NewTimer());
    }
}
