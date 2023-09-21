using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveTo))]
public class Unit : MonoBehaviour
{
    public string unitName;
    public Sprite Icon;
    public Utility.UnitTypes type;
    public Amount[] cost;
    public MoveTo moveTo;

    public Transform rangeOrigin;
    public float range;

    // Start is called before the first frame update
    void Start()
    {
        moveTo = GetComponent<MoveTo>();
        moveTo.On_FinalDestinationReach += RandomBetween;
        moveTo.SetDestination(new Vector3(Random.Range(rangeOrigin.position.x - range, rangeOrigin.position.x + range), Random.Range(rangeOrigin.position.y - range, rangeOrigin.position.y + range), 0));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void RandomBetween(GameObject sender)
    {
        //Debug.Log("SET DESTINATION");
        StartCoroutine(waiter(Random.Range(0,range*10)));
    }
    IEnumerator waiter(float t)
    {
        yield return new WaitForSeconds(t);
        moveTo.SetDestination(new Vector3(Random.Range(rangeOrigin.position.x - range, rangeOrigin.position.x + range), Random.Range(rangeOrigin.position.y - range, rangeOrigin.position.y + range), 0));
    }
}
