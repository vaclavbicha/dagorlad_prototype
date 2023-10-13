using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSpell : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    public GameObject spellPrefab;
    MoveTo spellInstance;
    //private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //canvasGroup = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        //canvasGroup
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //RallyPoint.GetComponent<MoveTo>().SetDestination(Camera.main.ScreenToWorldPoint(eventData.position));
        var position = Camera.main.ScreenToWorldPoint(eventData.position);
        if (!isMouseOverOverlayCanvas())
        {
            if(spellInstance == null) spellInstance = Instantiate(spellPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity).GetComponent<MoveTo>();
            else
            {
                spellInstance.SetDestination(new Vector3(position.x, position.y, 0));
            }
        }
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        spellInstance.GetComponent<Spell>().SpellStart();
        spellInstance.GetComponent<Spell>().On_SpellEnd += (spell) => { Destroy(spell); };//Destroy(spell, spell.GetComponent<Spell>().duration); };
        spellInstance = null;
        Camera.main.GetComponent<MoveTo>().Lock = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        Camera.main.GetComponent<MoveTo>().Lock = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
    }

    public bool isMouseOverOverlayCanvas()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var ev in raycastResults)
        {
            //if (ev.gameObject.layer == 9) ev.gameObject.GetComponent<Draggable>().ONNNN();
            if (ev.gameObject.layer == 5) return true; //layer 5 is the UI layer
        }
        return false;
    }
}
