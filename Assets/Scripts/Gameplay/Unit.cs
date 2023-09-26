using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveTo))]
public class Unit : MonoBehaviour
{
    public string unitName;
    public Sprite Icon;
    public Utility.UnitTypes type;
    public Utility.UnitStatus status;
    public Amount buildTime;
    public Amount[] cost;
    MoveTo moveTo;
    OnTrigger onTrigger;
    OnCollision onCollision;

    public Transform rangeOrigin;
    public float range;

    public Timer attackTimer;
    public StatsManager statsManager;
    public StatsManager currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        moveTo = GetComponent<MoveTo>();
        //onTrigger = GetComponent<OnTrigger>();
        onCollision = GetComponent<OnCollision>();
        statsManager = GetComponent<StatsManager>();
        //onTrigger.AddEvent("Enter", "Enemy", OnEnemyEncounter);
        //moveTo.On_FinalDestinationReach += RandomBetween;
        //moveTo.SetDestination(new Vector3(Random.Range(rangeOrigin.position.x - range, rangeOrigin.position.x + range), Random.Range(rangeOrigin.position.y - range, rangeOrigin.position.y + range), 0));
    }

    //private void OnEnemyEncounter(GameObject sender, Collider2D otherCollider)
    //{
    //    if (otherCollider.GetComponent<StatsManager>().owner != Player.Instance.name) Debug.Log("ATTACK");
    //}

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<StatsManager>()) if (collision.gameObject.GetComponent<StatsManager>().owner != Player.Instance.name)
        {
            if(status == Utility.UnitStatus.LookingToAttack)
                {
                    Attack(collision.gameObject);
                }
        }
    }
    public void Attack(GameObject Enemy)
    {
        if (attackTimer == null)
        {
            Debug.Log("ATTACK " + Enemy.name);
            if (Enemy.GetComponent<Unit>() != null)
            {
                status = Utility.UnitStatus.Attacking;
                attackTimer = gameObject.AddComponent<Timer>();
                foreach (var x in statsManager.stats)
                {
                    if (x.type == Utility.StatsTypes.AttackSpeed)
                    {
                        currentTarget = Enemy.GetComponent<StatsManager>();
                        attackTimer.AddTimer("Attacking", x.value, false);
                        attackTimer.On_Duration_End += DealDamage;
                    }
                }
            }
        }
        //if (Enemy.GetComponent<Structure>())
        //{
        //    status = Utility.UnitStatus.Attacking;
        //}
    }
    public void DealDamage(Timer timer)
    {
        Destroy(timer);
        if (statsManager.GetStat(Utility.StatsTypes.Attack) != null)
        {
            var dead = currentTarget.TakeRawDamage(statsManager.GetStat(Utility.StatsTypes.Attack).value);
            if (dead)
            {
                Debug.Log(gameObject.name + " - KILLED - " + currentTarget.name);
                currentTarget = null;
            }
            else
            {
                Attack(currentTarget.gameObject);
            }
        }
        else { UIManager.Instance.DialogWindow("Tried to deal damage without owning attack"); }
    }
    // Update is called once per frame
    public void RandomBetween(GameObject sender)
    {
        //Debug.Log("SET DESTINATION");
        StartCoroutine(waiter(Random.Range(0,range*10)));
    }
    IEnumerator waiter(float t)
    {
        yield return new WaitForSeconds(t);
        moveTo.SetDestination(new Vector3(Random.Range(rangeOrigin.position.x - range, rangeOrigin.position.x + range), Random.Range(rangeOrigin.position.y - range, rangeOrigin.position.y + range), 0));
    }
}
