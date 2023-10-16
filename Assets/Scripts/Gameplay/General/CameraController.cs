using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public MapLocation clickedMapLocation = null;
    public bool dragFlag = false;

    public float holdTimerClickedMapLocation = 0f;
    float lastclickedTimer;

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
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isMouseOverOverlayCanvas() && !UIManager.Instance.lookForNextClick)
        {
            clickedMapLocation = OnMapLocationClick();
            if (clickedMapLocation != null) lastclickedTimer = Time.time + holdTimerClickedMapLocation;
        }
        if (Input.GetMouseButtonUp(0) && !isMouseOverOverlayCanvas() && clickedMapLocation != null && lastclickedTimer < Time.time)
        {
            if(clickedMapLocation == OnMapLocationClick())
            {
                Debug.Log("DASDADASDA");
                SelectLocation(clickedMapLocation);
            }
        }
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
        if (drag && !UIManager.Instance.window.gameObject.activeInHierarchy)
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
            Debug.Log(ev.gameObject.name);
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
    private MapLocation OnMapLocationClick()
    {
        //Debug.Log("MOUSE BUTTON DOWN" + camera.ScreenToWorldPoint(Input.mousePosition));
        RaycastHit2D[] hit = Physics2D.RaycastAll(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        foreach (var x in hit)
        {
            if (x.collider.gameObject.layer == 11 && x.collider.tag == "MapLocation")
            {
                //Debug.Log(x.transform.name);
                //var clickedMapLocation = x.transform.GetComponent<MapLocation>();
                //SelectLocation(clickedMapLocation);
                return x.transform.GetComponent<MapLocation>();
            }
            if (x.collider.gameObject.layer == 6 && x.collider.tag == "Structure")
            {
                //var clickedMapLocation = x.transform.GetComponent<Structure>();
                //SelectLocation(clickedMapLocation.mapLocation);
                return x.transform.GetComponent<Structure>().mapLocation;
            }
        }
        //clickedMapLocation = null;
        return null;
    }
    private void SelectLocation(MapLocation location)
    {
        foreach (var z in UIManager.Instance.toggleGroupBases.GetComponentsInChildren<Toggle>())
        {
            if (z.name.Contains(location.baseID.ToString()))
            {
                z.isOn = true;
            }
        }
        foreach (var z in UIManager.Instance.toggleGroupStructureTypes.GetComponentsInChildren<Toggle>())
        {
            if (z.name == location.type.ToString())
            {
                z.isOn = true;
            }
        }
        UIManager.Instance.OnSelectLocation(location.id, location.type);
        clickedMapLocation = null;
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
