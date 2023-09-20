using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    public Timer timer;
    [SerializeField]
    public Slider loadingBar;

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
        color = status == Utility.LocationStatus.Building ? Color.green : color;
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
            building.SetActive(false);
            status = Utility.LocationStatus.Building;

            if (timer == null)
            {
                timer = gameObject.AddComponent<Timer>();
                timer.AddTimer("Building", 15f, true);
                loadingBar = Instantiate(buildingPrefab.GetComponent<Structure>().loadingBarPrefab,
                    UIManager.Instance.bottomPanelContent.GetComponentsInChildren<ItemManager>().ToList().Find(x => x.locationID == id).transform.GetChild(1).GetChild(0)).GetComponent<Slider>();
                timer.On_PingAction += UpdateSlider;
                timer.On_Duration_End += isDoneBuilding;

            }
            else
            {
                UIManager.Instance.DialogWindow("This location all ready has a timer");
            }
        }
        else
        {
            UIManager.Instance.DialogWindow("This location all ready has a building on it");
        }
    }
    public void UpdateSlider(Timer _timer)
    {
        loadingBar.value = Mathf.Abs((Time.time - _timer.timeStarted) / (_timer.timeStarted - _timer.timeFinish));
    }
    public void isDoneBuilding(Timer _timer)
    {
        building.SetActive(true);
        status = Utility.LocationStatus.Built;
    }
}
