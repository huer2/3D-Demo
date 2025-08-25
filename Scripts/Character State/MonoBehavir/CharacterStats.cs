using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    [HideInInspector]
    public bool isCritical;
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
}
