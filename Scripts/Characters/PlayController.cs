using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private GameObject attackTarget;
    private float lastAttackTime;
    private bool isDead;
    private float stoppDistance;
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        stoppDistance = agent.stoppingDistance;
    }
    void Update()
    {
        if (characterStats.CurrentHealth <= 0 && !isDead){isDead = true;}
        if(isDead)
           GameManager.Instance.Notifyobservers();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }
    void Start()
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget;
        MouseManager.Instance.OnEnemyClicked += EventAttack;
        GameManager.Instance.RigisterPlayer(characterStats);
        characterStats.CurrentHealth = characterStats.MaxHealth;
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if(isDead) return;
        agent.stoppingDistance = stoppDistance;
        agent.isStopped = false;
        agent.SetDestination(target);
    }
    public void EventAttack(GameObject target)
    {
        if(isDead) return;
        if (target != null)
        {
            attackTarget = target;
            characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }

    }
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStats.attackData.attackRange;
        if (attackTarget == null) yield break;
        transform.LookAt(attackTarget.transform);
        while (attackTarget != null && Vector3.Distance(transform.position, attackTarget.transform.position) > characterStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        if (attackTarget == null) yield break;
        agent.isStopped = true;
        // Attack
        if (lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStats.isCritical);
            anim.SetTrigger("Attack");
            lastAttackTime = characterStats.attackData.coolDown;
        }
    }
    // Animation Event
    void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if (attackTarget.GetComponent<Rock>()&&attackTarget.GetComponent<Rock>().rockstates == Rock.Rockstates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockstates = Rock.Rockstates.Hitenemy;
                attackTarget.GetComponent<Rigidbody>().velocity=Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward*20,ForceMode.Impulse);
            }
        }
        else
        { 
        var targetStats = attackTarget.GetComponent<CharacterStats>();
        targetStats.TakeDamage(characterStats, targetStats);
        }
    }
 
}
