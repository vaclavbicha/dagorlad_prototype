using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    List<SliderMenu> sliderMenus = new();
    [SerializeField]
    List<GameObject> spellButtons = new();
    GameObject currentlySelectedSpellButton;

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

    void UnselectSpellButtons() {
        foreach (GameObject child in spellButtons) {
            if (child == currentlySelectedSpellButton) {
                currentlySelectedSpellButton.GetComponent<DragSpell>().UnSelectSpell();
                currentlySelectedSpellButton = null;
            }
        }
    }
}
