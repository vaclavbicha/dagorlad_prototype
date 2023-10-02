using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MiniMapController : MonoBehaviour, IPointerClickHandler
{
    public Camera miniMapCam;


    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 cursor = new Vector2(0, 0);

        //Debug.Log(eventData.pressPosition);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RawImage>().rectTransform,
            eventData.pressPosition, eventData.pressEventCamera, out cursor))
        {

            Texture texture = GetComponent<RawImage>().texture;
            Rect rect = GetComponent<RawImage>().rectTransform.rect;

            float coordX = Mathf.Clamp(0, (((cursor.x - rect.x) * texture.width) / rect.width), texture.width);
            float coordY = Mathf.Clamp(0, (((cursor.y - rect.y) * texture.height) / rect.height), texture.height);

            float calX = coordX / texture.width;
            float calY = coordY / texture.height;


            cursor = new Vector2(calX, calY);

            CastRayToWorld(cursor);
        }


    }

    private void CastRayToWorld(Vector2 vec)
    {
        Vector2 MapRay = miniMapCam.ScreenToWorldPoint(new Vector2(vec.x * miniMapCam.pixelWidth,
            vec.y * miniMapCam.pixelHeight));
        //Debug.Log("RRARARARa" + vec + " ?? " + MapRay);
        GameManager.Instance.GetComponent<CameraController>().SetPositionWithClamp(MapRay);
    }
}