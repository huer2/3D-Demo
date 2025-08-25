using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private GameObject attackTarget;
    private float lastAttackTime;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
    }
    void Start()
    {
        MouseManager.Instance.OnMouseClicked+=MoveToTarget;
        MouseManager.Instance.OnEnemyClicked+=EventAttack;
    }
    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        agent.isStopped = false;
        agent.SetDestination(target);
    }
    public void EventAttack(GameObject target)
    {
        if(target != null)
        {
            attackTarget = target;
            StartCoroutine(MoveToAttackTarget());
        }
        
    }
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        transform.LookAt(attackTarget.transform);
        //TODO:Ìí¼Ó¹¥»÷¾àÀë
        while(Vector3.Distance(transform.position,attackTarget.transform.position)>1.5f)
        { 
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
         agent.isStopped = true;
        // Attack
        if(lastAttackTime  < 0)
        {
            anim.SetTrigger("Attack");
            //ÖØÖÃÀäÈ´
            lastAttackTime = 0.5f;
        }
       
    }
 
}
