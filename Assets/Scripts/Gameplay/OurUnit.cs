using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitMovement))]
public class OurUnit : MonoBehaviour
{
    public string unitName;
    public Sprite Icon;
    public Sprite InfoSprite;
    public Utility.UnitTypes type;
    public Utility.UnitStatus status;
    public int minimumBuildingTier;
    public Amount[] cost;
    public float attackRange;
    UnitMovement unitMovement;
    OnTrigger onTrigger;
    OnCollision onCollision;

    public Structure home;
    public Transform Rally_Point;

    public Timer attackTimer;
    public StatsManager statsManager;

    public StatsManager currentTarget;

    int attackid = 0;

    public delegate void OnSpawned(OurUnit sender);
    public event OnSpawned onSpawned;

    Vector2 previousPosition;
    public Vector2 lastMoveDirection;

    Animator animator;

    public GameObject bloodParticle;
    
    // Combat improvement variables
    private float targetLockTimer = 0f;
    private const float TARGET_LOCK_DURATION = 0.5f; // Time to lock onto target
    private float lastTargetSearchTime = 0f;
    private const float TARGET_SEARCH_INTERVAL = 0.3f; // How often to search for better targets
    private float movementStabilityTimer = 0f;
    private const float MOVEMENT_STABILITY_THRESHOLD = 0.2f; // Minimum time before changing direction
    
    // Enemy detection for enemy units
    private float enemyDetectionRange = 5f;
    private float lastEnemySearchTime = 0f;
    private const float ENEMY_SEARCH_INTERVAL = 0.4f; // How often enemies search for targets
    
    // Enemy spawn point and radius-based behavior
    private Vector3 spawnPoint;
    private float spawnRadius = 3f; // Radius from spawn point where enemy can attack (will be overridden by rally point radius)
    private bool isEnemyUnit = false;
    private CircleCollider2D rallyPointCollider; // Reference to rally point's collider for radius

    [System.Serializable]
    public class Attacker
    {
        public Transform position;
        public Transform attacker;
    }
    public List<Attacker> attackersSlots = new();

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
   
    public Transform AvailableAttackerPosition(Transform _attacker)
    {
        if (_attacker == null)
        {
            var result = attackersSlots.Find(x => x.attacker == null);
            return result?.position;//transform.TransformPoint(attackersSlots.Find(x => x.attacker == null).position);
        }
        else
        {
            var isAlreadyAttackedBy = attackersSlots.Find(x => x.attacker == _attacker);
            if (isAlreadyAttackedBy != null) return isAlreadyAttackedBy.position; //transform.TransformPoint(isAlreadyAttackedBy.position);
            else
            {
                isAlreadyAttackedBy = attackersSlots.Find(x => x.attacker == null);
                if (isAlreadyAttackedBy == null) return null;
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
        unitMovement = GetComponent<UnitMovement>();
        onTrigger = GetComponent<OnTrigger>();
        onCollision = GetComponent<OnCollision>();
        statsManager = GetComponent<StatsManager>();
        animator = GetComponent<Animator>();
        statsManager.On_Death += (sender) =>
        {
            if(unitMovement.CurrentMethod != UnitMovement.Method.NoMovement && statsManager.owner == "Player")
            {
                foreach (var x in cost)
                {
                    if (x.type == Utility.ResourceTypes.Supply)
                    {
                        //Player.Instance.currentSupply -= x.value;
                        //if (Player.Instance.currentSupply < 0) Player.Instance.currentSupply = 0;
                        //Player.Instance.resources.Find(y => y.amount.type == Utility.ResourceTypes.Supply).AmountUpdateWithText(0);
                    }
                }
            }
            unitMovement.CurrentMethod = UnitMovement.Method.NoMovement;
            status = Utility.UnitStatus.Dead;
            animator.SetBool("isDead", true);
            foreach(var i in GetComponents<CircleCollider2D>())
            {
                i.enabled = false;
            }
            if (Rally_Point)
            {
                Rally_Point.GetComponent<RallyPoint>().currentArmy.Remove(this);
                Rally_Point.GetComponent<RallyPoint>().ManageTargets();
            }
            Debug.Log("UNIT DIED");
            GameManager.Instance.GetComponent<AudioManager>().Play(unitName + "_death");
            Destroy(sender, 3f);
        };
        //onTrigger.AddEvent("Enter", "Enemy", OnEnemyEncounter);
        //onTrigger.AddEvent("Exit", "Enemy", OnEnemyLeave);

        //unitMovement.On_FinalDestinationReach += RandomBetween;
        //unitMovement.SetDestination(new Vector3(Random.Range(rangeOrigin.position.x - range, rangeOrigin.position.x + range), Random.Range(rangeOrigin.position.y - range, rangeOrigin.position.y + range), 0));
        //animator.speed = 0.7f;
    }

    public void Start() {
        onSpawned?.Invoke(this);
        var rng = new System.Random();
        Shuffle(rng, attackerPositions);
        foreach (var x in attackerPositions) {
            var attk_pos = new GameObject();

            //attk_pos.AddComponent<SpriteRenderer>().sprite = circle;
            //var colr = Color.white;
            //colr.a = 0.25f;
            //attk_pos.GetComponent<SpriteRenderer>().color = colr;
            //attk_pos.transform.localScale *= 0.5f;             Debug.Log(x.magnitude);

            attk_pos.transform.parent = transform;
            attk_pos.transform.localPosition = x;
            attk_pos.name = "attk_pos";
            attackersSlots.Add(new Attacker { position = attk_pos.transform, attacker = null });
            //attackersSlots.Add(new Attacker { position = x, attacker = null });
        }
        
        // If this is an enemy unit, start searching for player units
        if (GetComponent<StatsManager>().owner == "Enemy") {
            isEnemyUnit = true;
            spawnPoint = transform.position;
            // Set initial transform destination to spawn point to avoid null reference
            unitMovement.TransformDestination = transform;
            
            // Get spawn radius from rally point's Point_Range collider
            if (Rally_Point != null) {
                Transform pointRangeTransform = Rally_Point.Find("Point_Range");
                if (pointRangeTransform != null) {
                    rallyPointCollider = pointRangeTransform.GetComponent<CircleCollider2D>();
                    if (rallyPointCollider != null) {
                        // Calculate actual radius considering the scale
                        spawnRadius = rallyPointCollider.radius * pointRangeTransform.localScale.x;
                    }
                }
            }
            
            StartCoroutine(EnemySearchForTargets());
        }
    }

    private void FixedUpdate()
    {
        // Update target lock timer
        if (currentTarget != null) {
            targetLockTimer += Time.fixedDeltaTime;
        }

        if ((Vector2)transform.position == previousPosition) {
            animator.SetBool("isWalking", false);
            return;
        }

        Vector2 rawDirection = ((Vector2)transform.position - previousPosition).normalized;
        previousPosition = transform.position;
        animator.SetBool("isWalking", true);

        // Smooth direction changes to reduce jitter
        if (currentTarget && currentTarget.transform != null) {
            Vector2 directionToTarget = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
            // Lerp for smooth direction transitions
            lastMoveDirection = Vector2.Lerp(lastMoveDirection, directionToTarget, 0.08f);
        } else {
            // Smooth velocity-based direction changes
            lastMoveDirection = Vector2.Lerp(lastMoveDirection, rawDirection, 0.12f);
        }

        // Smoothly update animation parameters
        animator.SetFloat("x", lastMoveDirection.x);
        animator.SetFloat("y", lastMoveDirection.y);
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
        if (currentTarget != null)
        {
            Debug.Log("STOP ATTACK " + currentTarget.name);
            RemoveAttacker(currentTarget.transform);
        }
        else
        {
            Debug.Log("Stopped attacking nothing KEKW");
        }

        if (attackTimer != null)
        {
            Destroy(attackTimer);
            attackTimer = null;
        }

        currentTarget = null;
        status = Utility.UnitStatus.GoingToFlag;

        // Always set destination - for enemies, return to spawn point
        if (GetComponent<StatsManager>().owner == "Enemy")
        {
            // Return to spawn point
            unitMovement.TransformDestination = transform;
            unitMovement.CurrentMethod = UnitMovement.Method.SpeedWithTargetAndRange;
            unitMovement.range = 0.5f;
        }
        else
        {
            // Player units go to rally point
            if (Rally_Point != null)
            {
                unitMovement.TransformDestination = Rally_Point.transform;
            }
            else
            {
                unitMovement.TransformDestination = transform;
            }
            unitMovement.CurrentMethod = UnitMovement.Method.SpeedWithFormation;
            unitMovement.range = 0.1f;
        }

        unitMovement.IsLocked = false;
    }
    public void Attack(GameObject Enemy)
    {
        if (Enemy == null) return;
        
        StatsManager enemyStats = Enemy.GetComponent<StatsManager>();
        if (enemyStats == null) return;

        // If already attacking this target, don't switch (target lock)
        if (currentTarget == enemyStats && targetLockTimer < TARGET_LOCK_DURATION) {
            return;
        }

        // Set the attack position
        Transform attackPosition = null;
        OurUnit enemyUnit = Enemy.GetComponent<OurUnit>();
        if (enemyUnit != null)
        {
            attackPosition = enemyUnit.AvailableAttackerPosition(transform);
        }

        // If we can't get an attack position, don't attack
        if (attackPosition == null) return;

        // Only change target if we're not already attacking or lock time has expired
        if (currentTarget != enemyStats) {
            unitMovement.CurrentMethod = UnitMovement.Method.Attacking;
            unitMovement.TransformDestination = attackPosition;
            currentTarget = enemyStats;
            targetLockTimer = 0f; // Reset lock timer for new target
            status = Utility.UnitStatus.Attacking;
        } else {
            // Update destination to follow target, but stop if close enough
            float distanceToTarget = Vector2.Distance(transform.position, Enemy.transform.position);
            
            // If close enough to target, stop moving (lock in place)
            if (distanceToTarget < attackRange + 0.3f) {
                unitMovement.CurrentMethod = UnitMovement.Method.NoMovement;
            } else {
                // Still moving toward target
                unitMovement.CurrentMethod = UnitMovement.Method.Attacking;
                unitMovement.TransformDestination = attackPosition;
            }
        }

        // Clean up old timer if it exists
        if (attackTimer != null)
        {
            Destroy(attackTimer);
        }

        // Create new attack timer
        attackTimer = gameObject.AddComponent<Timer>();
        Stat attackSpeedStat = statsManager.GetStat(Utility.StatsTypes.AttackSpeed);
        
        if (attackSpeedStat != null)
        {
            attackTimer.AddTimer("Attacking" + attackid, attackSpeedStat.value, false);
            attackTimer.On_Duration_End += DealDamage;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " has no AttackSpeed stat!");
            Destroy(attackTimer);
            attackTimer = null;
        }
    }

    public void DealDamage(Timer timer)
    {
        Destroy(timer);
        
        // Check if current target is still valid
        if (currentTarget == null || currentTarget.dead)
        {
            currentTarget = null;
            return;
        }

        Stat attackStat = statsManager.GetStat(Utility.StatsTypes.Attack);
        if (attackStat == null)
        {
            UIManager.Instance.DialogWindow("Tried to deal damage without owning attack");
            return;
        }

        var dead = false;
        if (Mathf.Abs(Vector2.Distance(currentTarget.transform.position, transform.position)) < attackRange)
        {
            lastMoveDirection = ((Vector2)currentTarget.transform.position - (Vector2)transform.position).normalized;
            animator.SetFloat("x", lastMoveDirection.x);
            animator.SetFloat("y", lastMoveDirection.y);

            animator.SetTrigger("isAttacking");
            GameManager.Instance.GetComponent<AudioManager>().Play(unitName + "_attack");
            unitMovement.IsLocked = true;
            dead = currentTarget.TakeRawDamage(attackStat.value);
        }
        else
        {
            unitMovement.IsLocked = false;
        }

        if (dead)
        {
            Debug.Log(gameObject.name + " - KILLED - " + currentTarget.name);
            currentTarget = null;
        }
        else if (status != Utility.UnitStatus.Dead)
        {
            StartCoroutine(AttackAgain(0));
        }
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
        if(currentTarget != null && status != Utility.UnitStatus.Dead) Attack(currentTarget.gameObject);
    }

    // Enemy unit target detection and combat
    private IEnumerator EnemySearchForTargets()
    {
        while (status != Utility.UnitStatus.Dead)
        {
            yield return new WaitForSeconds(ENEMY_SEARCH_INTERVAL);
            
            // Check if unit has left spawn radius
            float distanceFromSpawn = Vector2.Distance(transform.position, spawnPoint);
            if (distanceFromSpawn > spawnRadius)
            {
                // Unit has left spawn radius, abandon pursuit and return to spawn
                if (currentTarget != null)
                {
                    StopAttack();
                }
                // Set destination to spawn point to return
                unitMovement.TransformDestination = transform;
                unitMovement.CurrentMethod = UnitMovement.Method.SpeedWithTargetAndRange;
                continue;
            }
            
            // Only search if not already in combat or if we should look for better targets
            if (currentTarget == null || Time.time - lastEnemySearchTime > ENEMY_SEARCH_INTERVAL)
            {
                OurUnit closestPlayerUnit = FindClosestPlayerUnit();
                
                if (closestPlayerUnit != null)
                {
                    // Check if target is within spawn radius
                    float targetDistance = Vector2.Distance(transform.position, closestPlayerUnit.transform.position);
                    float targetDistanceFromSpawn = Vector2.Distance(closestPlayerUnit.transform.position, spawnPoint);
                    
                    // Only attack if target is within spawn radius
                    if (targetDistanceFromSpawn <= spawnRadius)
                    {
                        // If we don't have a target or found a closer one, attack it
                        if (currentTarget == null || 
                            targetDistance < 
                            Vector2.Distance(transform.position, currentTarget.transform.position) - 0.5f)
                        {
                            Attack(closestPlayerUnit.gameObject);
                        }
                    }
                    else if (currentTarget != null)
                    {
                        // Target left spawn radius, stop attacking
                        StopAttack();
                    }
                }
                
                lastEnemySearchTime = Time.time;
            }
            
            yield return null;
        }
    }

    private OurUnit FindClosestPlayerUnit()
    {
        OurUnit closestUnit = null;
        float closestDistance = enemyDetectionRange;
        
        // Search through all game objects with OurUnit component
        OurUnit[] allUnits = FindObjectsOfType<OurUnit>();
        
        foreach (OurUnit unit in allUnits)
        {
            // Skip if unit is dead, is an enemy, or is this unit
            if (unit == null || unit.status == Utility.UnitStatus.Dead || 
                unit.GetComponent<StatsManager>().owner == "Enemy" || 
                unit == this)
                continue;
            
            float distance = Vector2.Distance(transform.position, unit.transform.position);
            
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestUnit = unit;
            }
        }
        
        return closestUnit;
    }
}
