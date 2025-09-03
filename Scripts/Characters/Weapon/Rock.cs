using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rock : MonoBehaviour
{
    public enum Rockstates { HitPlayer, Hitenemy, HitNothing }
    private Rigidbody rb;
    public Rockstates rockstates;
    [Header("Rock Settings")]
    public float froce;
    public int damage;
    public GameObject target;
    private Vector3 direction;
    public GameObject breakEffect;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.one;
        rockstates = Rockstates.HitPlayer;
    }
    void FixUpDate()
    { 
        if (rb.velocity.sqrMagnitude <= 0.1f)
            rockstates = Rockstates.HitNothing;
    }
    public void FlyToTarget()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();
        if (target == null)
            target = FindObjectOfType<PlayController>().gameObject;
        if (target == null) return; // ·ÀÖ¹targetÒ²Îªnull
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * froce, ForceMode.Impulse);
    }
    void OnCollisionEnter(Collision collision)
    {
        switch(rockstates)
        { 
            case Rockstates.HitPlayer:
                if (collision.gameObject.CompareTag("Player"))
                {
                    collision.gameObject.GetComponent<NavMeshAgent>().isStopped = true;
                    collision.gameObject.GetComponent<NavMeshAgent>().velocity = direction * froce;
                    collision.gameObject.GetComponent<Animator>().SetTrigger("Dizzy");
                    collision.gameObject.GetComponent<CharacterStats>().TakeDamage(damage, collision.gameObject.GetComponent<CharacterStats>());
                    rockstates = Rockstates.HitNothing;
                }
                break;
            case Rockstates.Hitenemy:
                if (collision.gameObject.GetComponent<Golem>())
                {
                    var otherStats = collision.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);
                    Destroy(gameObject);
                }
                break;
            case Rockstates.HitNothing:
                
                break;
        }
    }   
}
