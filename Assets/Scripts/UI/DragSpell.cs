using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;
using static DragSpell;

public class DragSpell : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private RectTransform rectTransform;
    public GameObject spellPrefab;
    public GameObject deathPrefab;
    DraggableMovement spellInstance;
    public bool RIGHT;
    public int level;
    bool isSelected;
    bool isDragged;

    public delegate void OnClicked(GameObject spellButton);
    public event OnClicked onClicked;

    //private CanvasGroup canvasGroup;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //canvasGroup = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
    }

    private void FixedUpdate() {
        if (isDragged) return;
        if (!isSelected) return;

        if (Input.touchCount > 0 && !isMouseOverOverlayCanvas()) {
            Touch touch = Input.GetTouch(0);

            Vector2 touch_position = touch.position;
            Vector3 screen_position = Camera.main.ScreenToWorldPoint(new Vector3(touch_position.x, touch_position.y, 0));
            spellInstance = Instantiate(spellPrefab, new Vector3(screen_position.x, screen_position.y, 0), Quaternion.identity).GetComponent<DraggableMovement>();
            if (spellInstance.GetComponent<Animator>()) spellInstance.GetComponent<Animator>().SetTrigger("EFFECT");
            spellInstance.GetComponent<SpriteRenderer>().enabled = true;
            spellInstance.SetDestination(screen_position);
            CastSpell();

            UnSelectSpell();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isSelected) {
            UnSelectSpell();
        }
        isDragged = true;
        Debug.Log("OnBeginDrag");
        //canvasGroup
    }
    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("OnDrag");
        Camera.main.GetComponent<CameraMovement>().IsLocked = true;
        //rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        //RallyPoint.GetComponent<DraggableMovement>().SetDestination(Camera.main.ScreenToWorldPoint(eventData.position));
        var position = Camera.main.ScreenToWorldPoint(eventData.position);

        // Won't buy without enough mana
        //var currIndex = RIGHT ? UIManager.Instance.rightINDEX : UIManager.Instance.leftINDEX;
        //if(currIndex < level) {
        //    Camera.main.GetComponent<CameraMovement>().IsLocked = false;
        //    UIManager.Instance.DialogWindow("NOT ENOUGH MANA!");
        //    return;
        //}

        if (spellInstance == null) {
            spellInstance = Instantiate(spellPrefab, position, Quaternion.identity).GetComponent<DraggableMovement>();
            if (spellInstance.GetComponent<Animator>()) spellInstance.GetComponent<Animator>().SetTrigger("EFFECT");
            spellInstance.SetDestination(position);
        }

        if (!isMouseOverOverlayCanvas()) {
            spellInstance.GetComponent<SpriteRenderer>().enabled = true;
            spellInstance.SetDestination(position);
        } else {
            // Cancel spell casting
            spellInstance.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    public void OnEndDrag(PointerEventData eventData) {
        Debug.Log("OnEndDrag");
        isDragged = false;
        if (spellInstance == null) return;

        // Cancel spell casting
        if (isMouseOverOverlayCanvas()) {
            spellInstance = null;
            return;
        }

        CastSpell();
    }

    private void CastSpell() {
        spellInstance.GetComponent<Spell>().SpellStart();
        spellInstance.GetComponent<Spell>().On_SpellEnd += (spell) => { StartCoroutine(SpawnDeath(spell.transform.position)); Destroy(spell); };//Destroy(spell, spell.GetComponent<Spell>().duration); };
        spellInstance = null;
        Camera.main.GetComponent<CameraMovement>().IsLocked = false;
        UIManager.Instance.SetTimer(RIGHT, level);

        Debug.Log(transform.parent.parent.parent.parent.parent.name);
        if (transform.parent.parent.parent.parent.parent.name.Contains("Left")) {

            UIManager.Instance.leftLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("IN");

        } else {
            UIManager.Instance.rightLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("IN");
        }
        //UIManager.Instance.leftLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().ResetTrigger("IN");
        //UIManager.Instance.rightLoadingBar.transform.parent.parent.parent.GetComponent<Animator>().ResetTrigger("IN");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
        // Won't buy without enough mana
        // var currIndex = RIGHT ? UIManager.Instance.rightINDEX : UIManager.Instance.leftINDEX;
        //     if(currIndex < level) {
        //         Camera.main.GetComponent<CameraMovement>().IsLocked = false;
        //         UIManager.Instance.DialogWindow("NOT ENOUGH MANA!");
        //         return;
        //     }

        var position = Camera.main.ScreenToWorldPoint(eventData.position);

        if (!isMouseOverOverlayCanvas()) return;

        if (!isSelected) {
            SelectSpell();
            onClicked?.Invoke(gameObject);
        } else {
            UnSelectSpell();
        }
    }

    public void SelectSpell() {
        isSelected = true;
        Camera.main.GetComponent<CameraMovement>().IsLocked = true;
        GetComponent<Image>().color = Color.red;
    }

    public void UnSelectSpell() {
        isSelected = false;
        Camera.main.GetComponent<CameraMovement>().IsLocked = false;
        GetComponent<Image>().color = Color.white;

        // Cancel spell casting
        spellInstance = null;
    }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     Debug.Log("OnPointerUp");
    //     isSelected = false;
    // }

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
