using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : MonoBehaviour
{
    public string owner;
    public List<MapLocation> locations = new List<MapLocation>();
    public GameObject baseFlag;

    public float flagEnter;
    public float stayPeriodToUnlock;
    public bool isLocked = true;
    // Start is called before the first frame update
    void Start()
    {
        baseFlag.GetComponent<OnTrigger>().AddEvent("Stay", "Player", (sender, collider) => {
            if(owner == "" && isLocked == true)
            {
                isLocked = false;
                flagEnter = Time.time;
            }
        if (Time.time - flagEnter > stayPeriodToUnlock && isLocked == false)
            {
                owner = "Player";
                UIManager.Instance.UnlockBase(gameObject.name);
                sender.GetComponent<Animator>().SetTrigger("RISE");
                sender.transform.GetChild(0).gameObject.SetActive(true);
                isLocked = true;
            }
        });
        baseFlag.GetComponent<OnTrigger>().AddEvent("Exit", "Player", (sender, collider) => {
            if (owner == "") flagEnter = Time.time;
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
