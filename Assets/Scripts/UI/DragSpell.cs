using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSpell : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    public GameObject spellPrefab;
    public GameObject deathPrefab;
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
            if (spellInstance == null)
            {
                spellInstance = Instantiate(spellPrefab, new Vector3(position.x, position.y, 0), Quaternion.identity).GetComponent<MoveTo>();
                spellInstance.GetComponent<Animator>().SetTrigger("EFFECT");
            }
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
        spellInstance.GetComponent<Spell>().On_SpellEnd += (spell) => { StartCoroutine(SpawnDeath(spell.transform.position)); Destroy(spell); };//Destroy(spell, spell.GetComponent<Spell>().duration); };
        spellInstance = null;
        Camera.main.GetComponent<MoveTo>().Lock = false;

        Debug.Log(transform.parent.parent.parent.parent.parent.name);
        if (transform.parent.parent.parent.parent.parent.name.Contains("Left"))
        {

            UIManager.Instance.leftLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("IN");

        }
        else
        {
            UIManager.Instance.rightLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("IN");
        }
        //UIManager.Instance.leftLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().ResetTrigger("IN");
        //UIManager.Instance.rightLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().ResetTrigger("IN");
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
    IEnumerator SpawnDeath(Vector3 pos)
    {
        yield return new WaitForSeconds(0);
        if (deathPrefab)
        {
            var x = Instantiate(deathPrefab, pos, Quaternion.identity);
            Destroy(x, 2f);
        }
    }
}
