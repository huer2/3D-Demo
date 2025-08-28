using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    GUARD,
    PATROL,
    CHASE,
    DEAD,
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStats))]
public class EnemyController : MonoBehaviour,IEndGameobserver
{
    private EnemyState enemyState = EnemyState.GUARD; // 初始化
    private NavMeshAgent agent;

    private Animator anim;
    private Collider coll;
    private CharacterStats characterStats;

    [Header("Basic Settings")]
    public bool isGuard = true; // 默认值
    public float sightRadius = 8f; // 默认值
    private float speed;
    protected GameObject attackTarget;

    public float lookAtTime;
    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guardRotation;

    [Header("Patrol Settings")]
    public float patrolRange = 0f;
    private Vector3 wayPoint;
    private Vector3 guardPos;
    //bool 配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead=false;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
        characterStats = GetComponent<CharacterStats>();
    }
    void Start()
    {
        if (isGuard)
        {
            enemyState = EnemyState.GUARD;
        }
        else
        {
            enemyState = EnemyState.PATROL;
            GetNewWayPoint();
        }
        //FIXME:场景切换后修改
        GameManager.Instance.Addbserver(this);
    }
    // void OnEnable()
    // {
    //      GameManager.Instance.Addbserver(this);
    // }
    void OnDisable()
    {
        if(GameManager.IsInitialized)
            GameManager.Instance.RemoveObserver(this);
    }
    void Update()
    {
        if (characterStats.CurrentHealth <= 0 && !isDead)
        {
            isDead = true;
        }
        if (!playerDead)
        {
            SwitchState();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }
    void SwitchState()
    {
        if(isDead)
        {
            enemyState = EnemyState.DEAD;
        }
        //如果发现玩家
        else if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE;
        }
        switch (enemyState)
        {
            case EnemyState.GUARD:
                isChase = false;
                agent.speed = speed * 0.5f;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.Distance(transform.position, guardPos) <= agent.stoppingDistance)
                    {
                        transform.position = guardPos;
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.2f);
                    }
                }
                break;
            case EnemyState.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;
                if (Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                        remainLookAtTime = lookAtTime;
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyState.CHASE:
                isWalk = false;
                isChase = true;
                agent.speed = speed;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                        agent.destination = transform.position;
                    }
                    else if (isGuard)
                    {
                        enemyState = EnemyState.GUARD;
                    }
                    else
                    {
                        enemyState = EnemyState.PATROL;
                    }
                }
                else if (attackTarget != null)
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.attackData.coolDown;
                        //暴击
                        characterStats.isCritical = Random.value < characterStats.attackData.criticalChance;
                        //执行攻击
                        Attack();
                    }
                }
                break;
            case EnemyState.DEAD:
                coll.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
        else if (TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        
    }
    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                attackTarget = collider.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null && Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.attackRange)
        {
            return true;
        }
        return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null && Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.skillRange)
        {
            return true;
        }
        return false;
    }
    void GetNewWayPoint()
    {
        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    // Animation Event
    void Hit()   
    {
        if(attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            targetStats.TakeDamage(characterStats, targetStats);
        }
    }

    public void EndNotify()
    {
        anim.SetBool("Win", true);
        playerDead=true;
        isChase = false;
        isFollow = false;
        isWalk = false;
        attackTarget = null;
    }
}
