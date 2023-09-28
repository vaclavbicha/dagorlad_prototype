using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    Vector3 mousePositionOffset;
    MoveTo moveTo;
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
    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }
    private void OnMouseDown()
    {
        Camera.main.GetComponent<MoveTo>().Lock = true;
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
    }
    private void OnMouseDrag()
    {
        moveTo.SetDestination(GetMouseWorldPosition() + mousePositionOffset);
    }
    private void OnMouseUp()
    {
        Camera.main.GetComponent<MoveTo>().Lock = false;
    }
}