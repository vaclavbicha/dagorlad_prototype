using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum LocationType {  Defense, Attack, Resource }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnBuilding(string arg)
    {

    }
    public struct Base
    {
        public List<Attribute> Attributes;
    }
    public struct Attribute
    {
        public List<Location> Locations;
    }
    public struct Location
    {
        public LocationType locationType;
        public Vector3 position;
    }
}
