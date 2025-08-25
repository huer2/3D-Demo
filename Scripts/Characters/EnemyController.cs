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
    private EnemyState enemyState = EnemyState.GUARD; // ��ʼ��
    private NavMeshAgent agent;

    [Header("Basic Settings")]
    public float sightRadius = 10f; // Ĭ��ֵ

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        SwitchState();
    }

    void SwitchState()
    {
        //����������
        if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE;
            Debug.Log("�������");
        }
        switch (enemyState)
        {
            case EnemyState.GUARD:
                break;
            case EnemyState.PATROL:
                break;
            case EnemyState.CHASE:
                break;
            case EnemyState.DEAD:
                break;
        }
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }
}
