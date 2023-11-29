using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    public string owner;
    Vector3 mousePositionOffset;
    MoveTo moveTo;
    public OnTrigger onTrigger;
    public Structure home;

    public List<OurUnit> currentArmy = new List<OurUnit>();
    public List<GameObject> EnemiesInRange = new List<GameObject>();

    private void Start()
    {
        moveTo = GetComponent<MoveTo>();
        GetComponentInChildren<OnTrigger>().AddEvent("Enter", owner == "Player" ? "Enemy" : "Player", (sender, collider) => {
            if (EnemiesInRange.Find(x => x.name == collider.name) == null)
            {
                EnemiesInRange.Add(collider.gameObject);
                ManageTargets();
                //home.AttackEnemiesInRange();
            }
        });
        GetComponentInChildren<OnTrigger>().AddEvent("Exit", owner == "Player" ? "Enemy" : "Player", (sender, collider) => {
            if (EnemiesInRange.Find(x => x.name == collider.name) != null)
            {
                foreach (var x in currentArmy)
                {
                    x.RemoveAttacker(collider.transform);
                }
                EnemiesInRange.Remove(collider.gameObject);
                ManageTargets();
                //home.StopAttackEnemiesInRange(collider.gameObject);
            }
        });
    }
    private void Update()
    {
        if(home == null && currentArmy.Count == 0)
        {
            Destroy(gameObject);
        }
    }
    public void NewUnitSpawned(OurUnit unit)
    {
        currentArmy.Add(unit);
        ManageTargets();
    }
    public void ManageTargets()
    {
        Debug.Log("Enemies : " + EnemiesInRange.Count + "  Troops : " + currentArmy.Count);
        //if(EnemiesInRange.Count < currentArmy.Count)
        //{
        //    Debug.Log("More Troops than Enemies");
        //}
        //else
        //{
        //    Debug.Log("More Enemies than Troops");
        //}
        var i = 0;
        foreach (var x in currentArmy)
        {
            if (EnemiesInRange.Count > 0)
            {
                if (i >= EnemiesInRange.Count)
                {
                    i = 0;
                }
                if (x.status != Utility.UnitStatus.Dead && EnemiesInRange[i].GetComponent<OurUnit>().AvailableAttackerPosition(null) != null) x.Attack(EnemiesInRange[i]);
                else Debug.LogError("COULDNT ATTACKKK");
                i++;
            }
            else
            {
                x.StopAttack();
            }
        }
    }
    //public void AttackEnemiesInRange()
    //{
    //    //Sorting + Target selection
    //    foreach (var x in currentArmy)
    //    {
    //        //x.Attack(Rally_Point.GetComponent<Draggable>().EnemiesInRange[0]);
    //    }
    //}
    //public void StopAttackEnemiesInRange(GameObject enemy)
    //{
    //    //Disengage
    //    var enemyStats = enemy.GetComponent<StatsManager>();
    //    foreach (var x in currentArmy)
    //    {
    //        if (x.currentTarget == enemyStats)
    //        {
    //            x.StopAttack();
    //        }
    //    }
    //}
    public void ON()
    {
        Debug.Log("AAAAABB");
        Camera.main.GetComponent<MoveTo>().Lock = true;
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
        
    }
    public void HOLD()
    {
        GetComponentInChildren<Animator>().SetBool("HOLD", true);
        moveTo.SetDestination(GetMouseWorldPosition() + mousePositionOffset);
    }
    public void OFF()
    {
        Camera.main.GetComponent<MoveTo>().Lock = false;
        GetComponentInChildren<Animator>().SetBool("HOLD", false);
        Camera.main.GetComponent<Animator>().SetTrigger("SmallShake");
    }
    private void OnMouseClickDown()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var ev in raycastResults)
        {
            if (ev.gameObject == gameObject)
            {
                Camera.main.GetComponent<MoveTo>().Lock = true;
                mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
            }
        }
    }
    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    //private void OnMouseDown()
    //{
    //    Camera.main.GetComponent<MoveTo>().Lock = true;
    //    mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    //}
    //private void OnMouseDrag()
    //{
    //    moveTo.SetDestination(GetMouseWorldPosition() + mousePositionOffset);
    //}
    //private void OnMouseUp()
    //{
    //    Camera.main.GetComponent<MoveTo>().Lock = false;
    //}
}