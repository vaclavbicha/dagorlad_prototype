using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

using UnityEngine.UI;

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
    UnityEngine.Color imageColor;

    public delegate void OnClicked(GameObject spellButton);
    public event OnClicked onClicked;

    public delegate void OnUnSelected(GameObject spellButton);
    public event OnUnSelected onUnSelected;

    //private CanvasGroup canvasGroup;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        //canvasGroup = GameObject.Find("Canvas").GetComponent<CanvasGroup>();
    }

    private void Start() {
        imageColor = GetComponent<Image>().color;
    }

    private void FixedUpdate() {
        if (isDragged) return;
        if (!isSelected) return;

        if (Input.touchCount > 0 && !UIManager.Instance.IsMouseOverOverlayCanvas()) {
            Touch touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Ended) { return; }

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
        var currIndex = RIGHT ? UIManager.Instance.rightINDEX : UIManager.Instance.leftINDEX;
        if (currIndex < level) {
            Camera.main.GetComponent<CameraMovement>().IsLocked = false;
            UIManager.Instance.DialogWindow("NOT ENOUGH MANA!");
            return;
        }

        if (spellInstance == null) {
            spellInstance = Instantiate(spellPrefab, position, Quaternion.identity).GetComponent<DraggableMovement>();
            if (spellInstance.GetComponent<Animator>()) spellInstance.GetComponent<Animator>().SetTrigger("EFFECT");
            spellInstance.SetDestination(position);
        }

        if (!UIManager.Instance.IsMouseOverOverlayCanvas()) {
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
        if (UIManager.Instance.IsMouseOverOverlayCanvas()) {
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
        // Won't buy without enough mana
        var currIndex = RIGHT ? UIManager.Instance.rightINDEX : UIManager.Instance.leftINDEX;
        if (currIndex < level) {
            Camera.main.GetComponent<CameraMovement>().IsLocked = false;
            UIManager.Instance.DialogWindow("NOT ENOUGH MANA!");
            return;
        }

        var position = Camera.main.ScreenToWorldPoint(eventData.position);

        if (!UIManager.Instance.IsMouseOverOverlayCanvas()) return;

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
        imageColor.a = 0.5f;
        GetComponent<Image>().color = imageColor;
    }

    public void UnSelectSpell() {
        isSelected = false;
        Camera.main.GetComponent<CameraMovement>().IsLocked = false;
        imageColor.a = 1;
        GetComponent<Image>().color = imageColor;

        // Cancel spell casting
        spellInstance = null;

        onUnSelected?.Invoke(gameObject);
    }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     Debug.Log("OnPointerUp");
    //     isSelected = false;
    // }

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
