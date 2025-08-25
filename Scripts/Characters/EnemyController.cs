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
public class EnemyController : MonoBehaviour
{
    private EnemyState enemyState = EnemyState.GUARD; // 初始化
    private NavMeshAgent agent;

    private Animator anim;
    private CharacterStats characterStats;

    [Header("Basic Settings")]
    public bool isGuard = true; // 默认值
    public float sightRadius = 8f; // 默认值
    private float speed;
    private GameObject attackTarget;

    public float lookAtTime; 
    private float remainLookAtTime;
    private float lastAttackTime;

    [Header("Patrol Settings")]
    public float patrolRange = 0f;
    private Vector3 wayPoint;
    private Vector3 guardPos;
    //bool 配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        speed = agent.speed;
        guardPos = transform.position;
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
    }
    void Update()
    {
        SwitchState();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
    }
    void SwitchState()
    {
        //如果发现玩家
        if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE;
        }
        switch (enemyState)
        {
            case EnemyState.GUARD:
                break;
            case EnemyState.PATROL:
                isChase = false;
                agent.speed = speed *0.5f;
                if (Vector3.Distance(transform.position, wayPoint) <= agent.stoppingDistance)
                { 
                    isWalk = false;
                    if(remainLookAtTime>0)
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
                if (TargetInAttackRange()||TargetInSkillRange())
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
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }
        else if(TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
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
        if(attackTarget != null && Vector3.Distance(transform.position, attackTarget.transform.position) <= characterStats.attackData.attackRange)
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
}
