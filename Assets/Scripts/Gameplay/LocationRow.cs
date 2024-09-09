using UnityEngine;

public class LocationRow : MonoBehaviour
{
    [SerializeField]
    Utility.LocationType type;

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
