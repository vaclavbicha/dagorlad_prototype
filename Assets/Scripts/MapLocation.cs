using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLocation : MonoBehaviour
{
    private Camera mainCamera;

    public int id;
    public int baseID;
    public bool isVisisble;
    public Player owner;
    public Utility.LocationType type;
    public Utility.LocationStatus status;
    public Utility.LocationSelectionStatus selectionStatus;
    public GameObject building = null;

    SpriteRenderer sprite;
    Color color;
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        color = selectionStatus == Utility.LocationSelectionStatus.Selected ? Color.red : Color.white;

        var screenPosition = mainCamera.WorldToScreenPoint(transform.position);
        isVisisble = !(screenPosition.x <= 10 || screenPosition.x >= Screen.width || screenPosition.y <= 10 || screenPosition.y >= Screen.height);

        color.a = isVisisble ? 0.2f : 1f;

        sprite.color = color;
    }

    public void SpawnBuilding(GameObject buildingPrefab)
    {
        if (building == null && selectionStatus == Utility.LocationSelectionStatus.Selected)
        {
            building = Instantiate(buildingPrefab, transform.position, Quaternion.identity);
            status = Utility.LocationStatus.Building;
        }
        else
        {
            UIManager.Instance.DialogWindow("This location all ready has a building on it");
        }
    }
}
