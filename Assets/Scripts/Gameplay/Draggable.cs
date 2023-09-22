using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    Vector3 mousePositionOffset;
    MoveTo moveTo;

    private void Start()
    {
        moveTo = GetComponent<MoveTo>();
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