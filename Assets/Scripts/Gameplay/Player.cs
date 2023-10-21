using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public string PlayerName;

    //public List<MapLocation> locations = new List<MapLocation>();

    public List<Resource> resources = new List<Resource>();

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
            var owned = resources.Find(x => x.amount.type == y.type).amount.value;
            if (owned < y.value) return false;
        }
        foreach (var y in price)
        {
            resources.Find(x => x.amount.type == y.type).AmountUpdate(-y.value);
        }
        return true;
    }
    public void DistributeResources(Timer timer)
    {
        foreach(var x in GameManager.Instance.ALL_Locations.FindAll(y => y.type == Utility.LocationType.Resource && y.owner == this && y.status == Utility.LocationStatus.Built))
        {
            resources.Find(y => y.amount.type == x.building.GetComponent<Structure>().production.type).AmountUpdate(x.building.GetComponent<Structure>().production.value);
        }
        Destroy(productionTimer);
        StartCoroutine(NewTimer());
    }
}
