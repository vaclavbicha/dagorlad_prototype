using UnityEngine;

public class BuildingSlotsRow : MonoBehaviour
{
    [SerializeField]
    Utility.BuildingSlotType type;

    void Start()
    {
        SetBuildingSlotsType();
    }

    void SetBuildingSlotsType() {
        for (int i = 0; i < transform.childCount; i++) {
            BuildingSlot slot = transform.GetChild(i).GetComponent<BuildingSlot>();
            slot.Type = type;
        }
    }
}
