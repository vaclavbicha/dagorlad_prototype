using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    public string PlayerName;

    public List<MapLocation> locations = new List<MapLocation>();

    public List<Resource> resources = new List<Resource>();

    public List<ItemUpgrade> ownedUpgrades = new List<ItemUpgrade>();

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
}
