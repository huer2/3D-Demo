using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Attack", menuName = "Attack/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown;
    public float minDamge;
    public float maxDamge; 
    public float criticalChance;//±©»÷ÂÊ
    public float criticalMultiplier;//±©»÷¼Ó³É
}
