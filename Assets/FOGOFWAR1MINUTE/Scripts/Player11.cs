using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player11 : MonoBehaviour
{
    public FogOfWar fogOfWar;
    public Transform secondaryFogOfWar;
    [Range(0,5)]
    public float sightDistance;
    public float checkInterval;

    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(CheckFogOfWar(checkInterval));
        secondaryFogOfWar.localScale = new Vector2(sightDistance, sightDistance) * 10f;
        //fogOfWar.AddUnit(gameObject, sightDistance);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            transform.position += transform.up * Time.deltaTime*5;
        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.up * Time.deltaTime*5;
        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.right * Time.deltaTime*5;
        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * Time.deltaTime*5;

        if(Time.time >= startTime + checkInterval)
        {
            startTime = Time.time;
            //fogOfWar.MakeHole(transform.position, sightDistance);
        }
    }

    private IEnumerator CheckFogOfWar(float checkInterval) {
        while (true) {
            fogOfWar.MakeHole(transform.position, sightDistance);
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
