using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MoveTo))]
public class OurUnit : MonoBehaviour
{
    public string unitName;
    public Sprite Icon;
    public Sprite InfoSprite;
    public Utility.UnitTypes type;
    public Utility.UnitStatus status;
    public Amount buildTime;
    public Amount[] cost;
    public float attackRange;
    MoveTo moveTo;
    OnTrigger onTrigger;
    OnCollision onCollision;

    public Structure home;
    public Transform Rally_Point;

    public Timer attackTimer;
    public StatsManager statsManager;

    public StatsManager currentTarget;

    int attackid = 0;


    Vector2 previousPosition;
    public Vector2 lastMoveDirection;

    Animator animator;

    public GameObject bloodParticle;

    [System.Serializable]
    public class Attacker
    {
        public Transform position;
        public Transform attacker;
    }
    public List<Attacker> attackersSlots = new List<Attacker>();

    public Sprite circle;

    Vector3[] attackerPositions = new Vector3[]
    {
        new Vector3(-0.78f, 0.45f, 0.0f) * 0.8f,
        new Vector3(0.78f, -0.45f, 0.00f)* 0.8f,
        new Vector3(0.00f, 0.90f, 0.00f)* 0.8f,
        new Vector3(0.00f, -0.90f, 0.00f)* 0.8f,
        new Vector3(-0.78f, -0.45f, 0.00f)* 0.8f,
        new Vector3(0.78f, 0.45f, 0.00f)* 0.8f
    };
    public static void Shuffle<T>(System.Random rng, T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {
            int k = rng.Next(n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
    public void Start()
    {
        var rng = new System.Random();
        Shuffle(rng, attackerPositions);
        foreach (var x in attackerPositions)
        {
            var attk_pos = new GameObject();

            //attk_pos.AddComponent<SpriteRenderer>().sprite = circle;
            //var colr = Color.white;
            //colr.a = 0.25f;
            //attk_pos.GetComponent<SpriteRenderer>().color = colr;
            //attk_pos.transform.localScale *= 0.5f;

            attk_pos.transform.parent = transform;
            attk_pos.transform.localPosition = x;
            attk_pos.name = "attk_pos";
            attackersSlots.Add(new Attacker { position = attk_pos.transform, attacker = null });
            Debug.Log(x.magnitude);
            //attackersSlots.Add(new Attacker { position = x, attacker = null });
        }
    }
    public Transform AvailableAttackerPosition(Transform _attacker)
    {
        if (_attacker == null) return attackersSlots.Find(x => x.attacker == null).position; //transform.TransformPoint(attackersSlots.Find(x => x.attacker == null).position);
        else
        {
            var isAlreadyAttackedBy = attackersSlots.Find(x => x.attacker == _attacker);
            if (isAlreadyAttackedBy != null) return isAlreadyAttackedBy.position; //transform.TransformPoint(isAlreadyAttackedBy.position);
            else
            {
                isAlreadyAttackedBy = attackersSlots.Find(x => x.attacker == null);
                isAlreadyAttackedBy.attacker = _attacker;
                return isAlreadyAttackedBy.position;//transform.TransformPoint(isAlreadyAttackedBy.position);
            }
        }
    }
    public void RemoveAttacker(Transform _attacker)
    {
        var isAlreadyAttackedBy = attackersSlots.Find(x => x.attacker == _attacker);
        if(isAlreadyAttackedBy != null)
        {
            isAlreadyAttackedBy.attacker = null;
        }
        else
        {
            //Debug.Log("TRIED TO REMOVE A NULL ATTACKER");
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        moveTo = GetComponent<MoveTo>();
        onTrigger = GetComponent<OnTrigger>();
        onCollision = GetComponent<OnCollision>();
        statsManager = GetComponent<StatsManager>();
        animator = GetComponent<Animator>();
        statsManager.On_Death += (sender) =>
        {
            if(moveTo.method != MoveTo.Method.NoMovement && statsManager.owner == "Player")
            {
                foreach (var x in cost)
                {
                    if (x.type == Utility.ResourceTypes.Supply)
                    {
                        Player.Instance.currentSupply -= x.value;
                        Player.Instance.resources.Find(y => y.amount.type == Utility.ResourceTypes.Supply).AmountUpdateWithText(0);
                    }
                }
            }
            moveTo.method = MoveTo.Method.NoMovement;
            status = Utility.UnitStatus.Dead;
            animator.SetBool("isDead", true);
            foreach(var i in GetComponents<CircleCollider2D>())
            {
                i.enabled = false;
            }
            if(Rally_Point) Rally_Point.GetComponent<Draggable>().currentArmy.Remove(this);
            Destroy(sender, 3f);
        };
        //onTrigger.AddEvent("Enter", "Enemy", OnEnemyEncounter);
        //onTrigger.AddEvent("Exit", "Enemy", OnEnemyLeave);

        //moveTo.On_FinalDestinationReach += RandomBetween;
        //moveTo.SetDestination(new Vector3(Random.Range(rangeOrigin.position.x - range, rangeOrigin.position.x + range), Random.Range(rangeOrigin.position.y - range, rangeOrigin.position.y + range), 0));
        //animator.speed = 0.7f;
    }
    private void FixedUpdate()
    {
        if ((Vector2)transform.position != previousPosition)
        {
            lastMoveDirection = ((Vector2)transform.position - previousPosition).normalized;
            previousPosition = transform.position;
            animator.SetBool("isWalking", true);
            if (currentTarget)
            {
                lastMoveDirection = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
                animator.SetFloat("x", lastMoveDirection.x);
                animator.SetFloat("y", lastMoveDirection.y);
            }
            else
            {
                animator.SetFloat("x", lastMoveDirection.x);
                animator.SetFloat("y", lastMoveDirection.y);
            }
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }

    private void OnEnemyEncounter(GameObject sender, Collider2D otherCollider)
    {
        if (otherCollider.GetComponent<StatsManager>().owner != Player.Instance.name)
        {
            Debug.Log("Unit " + gameObject.name + " encounter " + otherCollider.name + " and is going to attack him");

        }
    }
    private void OnEnemyLeave(GameObject sender, Collider2D otherCollider)
    {
        if (otherCollider.GetComponent<StatsManager>().owner != Player.Instance.name)
        {
            Debug.Log("Unit " + gameObject.name + " is leaving " + otherCollider.name + "-(LEFT THE ENCOUNTER RANGE) and is going back to the rally point");

        }
    }
    public void StopAttack()
    {
        if (currentTarget)
        {
            Debug.Log("STOP ATTACK " + currentTarget.name);
        }
        else Debug.Log("Stopped attacking nothing KEKW");
        Destroy(attackTimer);
        //new
        moveTo.method = MoveTo.Method.SpeedWithTargetAndRange;
        //
        currentTarget = null;
        status = Utility.UnitStatus.GoingToFlag;
        moveTo.TransformDestination = Rally_Point.transform;
        moveTo.range = 0.5f;
        moveTo.Lock = false;
    }
    public void Attack(GameObject Enemy)
    {
        //->older-> moveTo.method = MoveTo.Method.SpeedWithTarget;
        //moveTo.TransformDestination = Enemy.transform;
        //moveTo.range = 0.2f;
        //moveTo.rangeMin = 0.075f;
        //if(GetComponent<StatsManager>().owner == "Player")
        moveTo.method = MoveTo.Method.Attacking;
        moveTo.TransformDestination = Enemy.GetComponent<OurUnit>().AvailableAttackerPosition(transform);
        //moveTo.SetDestination(Enemy.GetComponent<OurUnit>().AvailableAttackerPosition(transform));
        if (attackTimer == null)
        {
            //Debug.Log("ATTACK " + Enemy.name);
            if (Enemy.GetComponent<OurUnit>() != null)
            {
                status = Utility.UnitStatus.Attacking;
                attackTimer = gameObject.AddComponent<Timer>();
                foreach (var x in statsManager.stats)
                {
                    if (x.type == Utility.StatsTypes.AttackSpeed)
                    {
                        currentTarget = Enemy.GetComponent<StatsManager>();
                        attackTimer.AddTimer("Attacking" + attackid, x.value, false);
                        attackTimer.On_Duration_End += DealDamage;
                    }
                }
            }else if (Enemy.GetComponent<Structure>() != null)
            {
                status = Utility.UnitStatus.Attacking;
                attackTimer = gameObject.AddComponent<Timer>();
                foreach (var x in statsManager.stats)
                {
                    if (x.type == Utility.StatsTypes.AttackSpeed)
                    {
                        currentTarget = Enemy.GetComponent<StatsManager>();
                        attackTimer.AddTimer("Attacking" + attackid, x.value, false);
                        attackTimer.On_Duration_End += DealDamage;
                    }
                }
            }
        }
        else
        {
            Debug.Log("The attack timer of " + gameObject.name + " is not null " + attackTimer.name);
            Destroy(attackTimer);
            attackTimer = gameObject.AddComponent<Timer>();
            foreach (var x in statsManager.stats)
            {
                if (x.type == Utility.StatsTypes.AttackSpeed)
                {
                    currentTarget = Enemy.GetComponent<StatsManager>();
                    attackTimer.AddTimer("Attacking" + attackid, x.value, false);
                    attackTimer.On_Duration_End += DealDamage;
                }
            }
        }
    }
    public void DealDamage(Timer timer)
    {
        Destroy(timer);
        if (statsManager.GetStat(Utility.StatsTypes.Attack) != null)
        {
            var dead = false;
            if (Mathf.Abs(Vector2.Distance(currentTarget.transform.position, transform.position)) < attackRange)
            {
                lastMoveDirection = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
                animator.SetFloat("x", lastMoveDirection.x);
                animator.SetFloat("y", lastMoveDirection.y);

                animator.SetTrigger("isAttacking");
                //GameManager.Instance.GetComponent<AudioManager>().Play(unitName + "_attack");
                moveTo.Lock = true;
                dead = currentTarget.TakeRawDamage(statsManager.GetStat(Utility.StatsTypes.Attack).value);
            }
            else
            {
                moveTo.Lock = false;
            }
            if (dead)
            {
                Debug.Log(gameObject.name + " - KILLED - " + currentTarget.name);
                currentTarget = null;
            }
            else
            {
                StartCoroutine(AttackAgain(0));
            }
        }
        else { UIManager.Instance.DialogWindow("Tried to deal damage without owning attack"); }
    }
    public void Bleed()
    {
        var blood = Instantiate(bloodParticle, transform.position, Quaternion.identity);
        Destroy(blood, blood.GetComponent<ParticleSystem>().main.duration);
    }
    IEnumerator AttackAgain(float t)
    {
        yield return new WaitForSeconds(t);
        attackid++;
        if(currentTarget != null) Attack(currentTarget.gameObject);
    }
}
