using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public static SpellManager Instance;
    List<SliderMenu> sliderMenus = new();
    [SerializeField]
    List<GameObject> spellButtons = new(); 
    [SerializeField]
    GameObject currentlySelectedSpellButton;

    private void Awake() {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }

    void Start() {
        LoadChildren();
    }

    void LoadChildren() {
        sliderMenus = new List<SliderMenu>(FindObjectsOfType<SliderMenu>());  
        
        foreach (var menu in sliderMenus) {
            for (int i = 0; i < menu.transform.childCount; i++) {
                GameObject child = menu.transform.GetChild(i).GetChild(0).gameObject;
                spellButtons.Add(child);
                child.GetComponent<DragSpell>().onClicked += ManageSpellButtonSelection;
                child.GetComponent<DragSpell>().onUnSelected += ClearSelectedSpellButton;
            }
        }        
    }

    void ManageSpellButtonSelection(GameObject spellButton) {
        UnselectSpellButtons();
        SelectSpellButton(spellButton);
    }

    void SelectSpellButton(GameObject spellButton) {
        currentlySelectedSpellButton = spellButton;
        currentlySelectedSpellButton.GetComponent<DragSpell>().SelectSpell();
    }

    void ClearSelectedSpellButton(GameObject spellButton) {
        if (currentlySelectedSpellButton == spellButton) {
            currentlySelectedSpellButton = null;
        }
    }

    void UnselectSpellButtons() {
        foreach (GameObject child in spellButtons) {
            if (child == currentlySelectedSpellButton) {
                currentlySelectedSpellButton.GetComponent<DragSpell>().UnSelectSpell();
            }
        }
    }

    public bool IsAnySpellSelected() {
        Debug.Log(currentlySelectedSpellButton != null);
        return (currentlySelectedSpellButton != null);
    }
}
