using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    Camera camera;
    MoveTo camera_move;

    public SpriteRenderer mapRenderer;
    float mapMinX, mapMaxX, mapMinY, mapMaxY;

    Vector3 Origin;
    Vector3 Difference;

    bool drag = false;

    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        camera_move = camera.GetComponent<MoveTo>();

        mapMinX = mapRenderer.transform.position.x - mapRenderer.bounds.size.x / 2f;
        mapMaxX = mapRenderer.transform.position.x + mapRenderer.bounds.size.x / 2f;

        mapMinY = mapRenderer.transform.position.y - mapRenderer.bounds.size.y / 2f;
        mapMaxY = mapRenderer.transform.position.y + mapRenderer.bounds.size.y / 2f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0) && !isMouseOverOverlayCanvas())
        {
            Difference = camera.ScreenToWorldPoint(Input.mousePosition) - camera.transform.position;
            if (!drag)
            {
                drag = true;
                Origin = camera.ScreenToWorldPoint(Input.mousePosition);
            }
        }
        else
        {
            drag = false;
        }

        if (drag)
        {
            camera_move.SetDestination(ClampCamera(Origin - Difference));
        }
    }
    private bool isMouseOverOverlayCanvas()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach(var ev in raycastResults)
        {
            if (ev.gameObject.layer == 5) return true; //layer 5 is the UI layer
        }
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
}
