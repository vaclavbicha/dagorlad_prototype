using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    Camera camera;
    MoveTo cameraMove;

    public SpriteRenderer mapRenderer;
    float mapMinX, mapMaxX, mapMinY, mapMaxY;

    Vector3 Origin;
    Vector3 Difference;

    public bool drag = false;

    public bool isOverFlag = false;
    public Draggable currentRallyPoint = null;
    public bool dragFlag = false;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        cameraMove = camera.GetComponent<MoveTo>();

        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //if(Input.GetMouseButtonDown(0)) SearchRallyPoint();
        if (Input.GetMouseButton(0) && !isMouseOverOverlayCanvas())
        {
            if (!drag && !dragFlag) isOverFlag = SearchRallyPoint();
            //if(!drag && !currentRallyPoint) SearchRallyPoint();
            //if (cameraMove.Lock && currentRallyPoint) currentRallyPoint.HOLD();
            if (isOverFlag)
            {
                if (!dragFlag)
                {
                    dragFlag = true;
                }
            }
            else
            {
                Difference = camera.ScreenToWorldPoint(Input.mousePosition) - camera.transform.position;
                if (!drag)
                {
                    drag = true;
                    Origin = camera.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }
        else
        {
            drag = false;

            dragFlag = false;
            isOverFlag = false;
            if (currentRallyPoint) currentRallyPoint.OFF();
            currentRallyPoint = null;
        }
        if (dragFlag)
        {
            currentRallyPoint.HOLD();
        }
        if (drag)
        {
            cameraMove.SetDestination(ClampCamera(Origin - Difference));
        }
    }
    public bool isMouseOverOverlayCanvas()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach(var ev in raycastResults)
        {
            //if (ev.gameObject.layer == 9) ev.gameObject.GetComponent<Draggable>().ONNNN();
            if (ev.gameObject.layer == 5) return true; //layer 5 is the UI layer
        }
        return false;
    }
    private bool SearchRallyPoint()
    {
        //Debug.Log("MOUSE BUTTON DOWN" + camera.ScreenToWorldPoint(Input.mousePosition));
        RaycastHit2D[] hit = Physics2D.RaycastAll(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        foreach(var x in hit)
        {
            if(x.collider.gameObject.layer == 9 && x.collider.tag == "Rally_Point")
            {
                //Debug.Log(x.transform.name);
                currentRallyPoint = x.transform.GetComponent<Draggable>();
                currentRallyPoint.ON();
                return true;
            } 
        }
        currentRallyPoint = null;
        return false;
    }
    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = camera.orthographicSize;
        float camWidth = camera.orthographicSize * camera.aspect;

        float minX = mapMinX + camWidth;
        float maxX = mapMaxX - camWidth;
        float minY = mapMinY + camHeight;
        float maxY = mapMaxY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
    public void SetPositionWithClamp(Vector3 pos)
    {
        cameraMove.SetDestination(ClampCamera(pos));
    }
}
