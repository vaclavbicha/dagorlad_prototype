using System.Collections.Generic;
using UnityEngine;

public class SliderMenu : MonoBehaviour
{
    List<GameObject> spellButtons = new List<GameObject>();
    GameObject currentlySelectedSpellButton;

    void Start()
    {
        LoadChildren();
    }

    void LoadChildren() {
        for (int i = 0; i < transform.childCount; i++) {
            GameObject child = transform.GetChild(i).GetChild(0).gameObject;
            spellButtons.Add(child);
            child.GetComponent<DragSpell>().onClicked += ManageSpellButtonSelection;
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
