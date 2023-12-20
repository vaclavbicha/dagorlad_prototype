using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draggable : MonoBehaviour
{
    public string owner;
    Vector3 mousePositionOffset;
    MoveTo moveTo;
    public OnTrigger onTrigger;
    public GameObject home;

    public List<OurUnit> currentArmy = new List<OurUnit>();
    public List<GameObject> EnemiesInRange = new List<GameObject>();

    public int MAX_UNITS = 30;
    public List<Vector3> targetPositionlist = new List<Vector3>();

    //public class FormationPosition
    //{
    //    public Vector3 
    //}

    private void Start()
    {
        //Debug.Log(transform.parent.TransformPoint(transform.localPosition));
        // for enemies targetPositionlist = GetPositionListAround2(transform.parent.TransformPoint(transform.localPosition), new float[] { 0.3f, 0.6f, 0.9f }, new int[] { 5, 10, 15 });
        //targetPositionlist = GetPositionListAround2(transform.position - new Vector3(0, 0.25f, 0), new float[] { 0.5f, 0.8f, 1.1f }, new int[] { 5, 10, 15 });
        //targetPositionlist.RemoveAt(0);

        //var cicle = transform.GetChild(1).GetComponent<SpriteRenderer>().sprite;
        //foreach(var x in targetPositionlist)
        //{
        //    var attk_pos = new GameObject();

        //    attk_pos.AddComponent<SpriteRenderer>().sprite = cicle;
        //    var colr = Color.white;
        //    colr.a = 0.25f;
        //    attk_pos.GetComponent<SpriteRenderer>().color = colr;
        //    attk_pos.transform.localScale *= 0.5f;// Debug.Log(x.magnitude);

        //    attk_pos.transform.position = x;

        //    attk_pos.transform.parent = transform;
        //    attk_pos.name = "attk_pos";
        //}

        moveTo = GetComponent<MoveTo>();
        GetComponentInChildren<OnTrigger>().AddEvent("Enter", owner == "Player" ? "Enemy" : "Player", (sender, collider) => {
            if (EnemiesInRange.Find(x => x.name == collider.name) == null)
            {
                EnemiesInRange.Add(collider.gameObject);
                ManageTargets();
                //home.AttackEnemiesInRange();
            }
        });
        GetComponentInChildren<OnTrigger>().AddEvent("Exit", owner == "Player" ? "Enemy" : "Player", (sender, collider) => {
            if (EnemiesInRange.Find(x => x.name == collider.name) != null)
            {
                foreach (var x in currentArmy)
                {
                    x.RemoveAttacker(collider.transform);
                }
                EnemiesInRange.Remove(collider.gameObject);
                ManageTargets();
                //home.StopAttackEnemiesInRange(collider.gameObject);
            }
        });
        ManageTargets();
    }
    private void Update()
    {
        if (owner == "Enemy" && currentArmy.Count == 0)
        {
            if(home != null) home.GetComponent<Base>().owner = "";
            GameManager.Instance.enemies.Remove(this);
            if(GameManager.Instance.enemies.Count == 0)
            {
                Debug.Log("GameWon");
                UIManager.Instance.winWindow.SetActive(true);
            }
            Destroy(gameObject);
        }
    }
    public void NewUnitSpawned(OurUnit unit)
    {
        currentArmy.Add(unit);
        ManageTargets();
    }
    public void ManageTargets()
    {
        Debug.Log("Enemies : " + EnemiesInRange.Count + "  Troops : " + currentArmy.Count);
        var i = 0;
        var j = 0;
        foreach (var x in currentArmy)
        {
            if (EnemiesInRange.Count > 0)
            {
                if (i >= EnemiesInRange.Count)
                {
                    i = 0;
                }
                if (x.status != Utility.UnitStatus.Dead && EnemiesInRange[i].GetComponent<OurUnit>().AvailableAttackerPosition(null) != null) x.Attack(EnemiesInRange[i]);
                //else
                //{
                //    x.StopAttack();
                //    x.GetComponent<MoveTo>().offsetRallyPoint = targetPositionlist[j];//- transform.position;
                //}
                i++;
            }
            else
            {
                x.StopAttack();
                x.GetComponent<MoveTo>().offsetRallyPoint = targetPositionlist[j];//- transform.position;
            }
            j++;
        }
    }
    public void ON()
    {
        Debug.Log("AAAAABB");
        Camera.main.GetComponent<MoveTo>().Lock = true;
        mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();

    }
    public void HOLD()
    {
        foreach (var x in GetComponentsInChildren<Animator>())
        {
            x.SetBool("HOLD", true);
        }
        moveTo.SetDestination(GetMouseWorldPosition() + mousePositionOffset);
    }
    public void OFF()
    {
        Camera.main.GetComponent<MoveTo>().Lock = false;
        foreach (var x in GetComponentsInChildren<Animator>())
        {
            x.SetBool("HOLD", false);
        }
        Camera.main.GetComponent<Animator>().SetTrigger("SmallShake");
    }
    private void OnMouseClickDown()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResults);
        foreach (var ev in raycastResults)
        {
            if (ev.gameObject == gameObject)
            {
                Camera.main.GetComponent<MoveTo>().Lock = true;
                mousePositionOffset = gameObject.transform.position - GetMouseWorldPosition();
            }
        }
    }
    public void TapAndTap()
    {
        Debug.Log("PLACEEE");
        foreach (var x in GetComponentsInChildren<Animator>())
        {
            x.SetBool("HOLD", true);
        }
        //foreach (var child2 in UIManager.Instance.bottomPanelContent.GetChild(1).GetComponentsInChildren<ItemManager>())
        //{
        //    if (child2.RallyPoint == gameObject.transform) UIManager.Instance.LookToPlaceRallyPoint(transform, child2.mid.transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>());
        //}
        UIManager.Instance.LookToPlaceRallyPoint(transform);
    }
    private Vector3 GetMouseWorldPosition()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private List<Vector3> GetPositionListAround2(Vector3 startPosition, float[] ringDistanceArray, int[] ringpositioncountArray)
    {
        Debug.Log("!!" + startPosition);
        List<Vector3> positionlist = new List<Vector3>();
        positionlist.Add(startPosition);
        for (int i = 0; i < ringDistanceArray.Length; i++) {
            positionlist.AddRange(GetPositionlistAround(startPosition, ringDistanceArray[i], ringpositioncountArray[i]));
        }
        return positionlist;
    }
    private List<Vector3> GetPositionlistAround(Vector3 startPosition, float distance, int positionCount)
    {
        List<Vector3> positionlist = new List<Vector3>();
        for (int i = 0; i < positionCount; i++)
        {
            float angle = i * (360f / positionCount);
            Vector3 dir = ApplyRotationToVector(new Vector3(1, 0), angle);
            Vector3 position = startPosition + dir * distance;
            positionlist.Add(position);
        }
        
        return positionlist;
    }
    private Vector3 ApplyRotationToVector(Vector3 vec, float angle)
    { 
        return Quaternion.Euler(0, 0, angle) * vec;
    }
}