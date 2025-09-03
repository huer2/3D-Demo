using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    [HideInInspector]
    public bool isCritical;
    void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
    }
    #region Read from Data_SO
    public int MaxHealth
    {
        get => characterData != null ? characterData.maxHealth : 0;
        set { if (characterData != null) characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get => characterData != null ? characterData.currentHealth : 0;
        set { if (characterData != null) characterData.currentHealth = value; }
    }
    public int BaseDefense
    {
        get => characterData != null ? characterData.baseDefense : 0;
        set { if (characterData != null) characterData.baseDefense = value; }
    }
    public int CurrentDefense
    {
        get => characterData != null ? characterData.currentDefense : 0;
        set { if (characterData != null) characterData.currentDefense = value; }
    }
    #endregion
    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefense, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO: Show damage UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //TODO:¾­Ñéupdate
    }
    public void TakeDamage(int damage, CharacterStats defender)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
    }
    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("±©»÷" + coreDamage);
        }
        return (int)coreDamage;
    }
    #endregion
}
