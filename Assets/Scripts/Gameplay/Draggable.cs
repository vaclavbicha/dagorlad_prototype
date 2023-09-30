using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour
{
    Vector3 mousePositionOffset;
    MoveTo moveTo;
    public OnTrigger onTrigger;
    public Structure home;

    public List<GameObject> EnemiesInRange = new List<GameObject>();

    private void Start()
    {
        moveTo = GetComponent<MoveTo>();
        GetComponentInChildren<OnTrigger>().AddEvent("Enter", "Enemy", (sender, collider) => {
            if (EnemiesInRange.Find(x => x.name == collider.name) == null)
            {
                EnemiesInRange.Add(collider.gameObject);
                home.AttackEnemiesInRange();
            }
        });
        GetComponentInChildren<OnTrigger>().AddEvent("Exit", "Enemy", (sender, collider) => {
            if (EnemiesInRange.Find(x => x.name == collider.name) != null)
            {
                EnemiesInRange.Remove(collider.gameObject);
                home.StopAttackEnemiesInRange(collider.gameObject);
            }
        });
    }
    //public void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        Debug.Log("AAAAAAAAAAA");
    //        //OnMouseClickDown();
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
        moveTo.SetDestination(GetMouseWorldPosition() + mousePositionOffset);
    }
    public void OFF()
    {
        Camera.main.GetComponent<MoveTo>().Lock = false;
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