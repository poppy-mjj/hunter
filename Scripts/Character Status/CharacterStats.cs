using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBarOnAttack;
    public event Action<float, float> UpdateEnergyBarOnAttack;


    public CharacterData_SO templateData;
    public CharacterData_SO characterData;

    public AttackData_SO attackData;
    private AttackData_SO baseAttackData;
    [HideInInspector]
    public bool isCritical;

    private void Awake()
    {

        if (templateData != null)
            characterData = Instantiate(templateData);

        baseAttackData = Instantiate(attackData);
        //baseAnimator = GetComponent<Animator>().runtimeAnimatorController;
    }
    #region Read from Data_SO

    public int MaxHealth
    {
        get
        {
            if (characterData != null)
                return characterData.maxHealth;
            else return 0;
        }
        set
        {
            characterData.maxHealth = value;
        }
    }

    public int CurrentHealth
    {
        get
        {
            if (characterData != null)
                return characterData.currentHealth;
            else return 0;
        }
        set
        {
            characterData.currentHealth = value;
        }
    }    
    public float MaxEnergy
    {
        get
        {
            if (characterData != null)
                return characterData.maxEnergy;
            else return 0;
        }
        set
        {
            characterData.maxEnergy = value;
        }
    }

    public float CurrentEnergy
    {
        get
        {
            if (characterData != null)
                return characterData.currentEnergy;
            else return 0;
        }
        set
        {
            characterData.currentEnergy = value;
        }
    }
    public float AltCostEnergy
    {
        get
        {
            if (characterData != null)
                return characterData.altCostEnergy;
            else return 0;
        }
        set
        {
            characterData.altCostEnergy = value;
        }
    }    
    public float JumpCostEnergy
    {
        get
        {
            if (characterData != null)
                return characterData.jumpCostEnergy;
            else return 0;
        }
        set
        {
            characterData.jumpCostEnergy = value;
        }
    }    
    public float HealEnergyPerTime
    {
        get
        {
            if (characterData != null)
                return characterData.healEnergyPerTime;
            else return 0;
        }
        set
        {
            characterData.healEnergyPerTime = value;
        }
    }
    public float RunCostEnergyPerTime
    {
        get
        {
            if (characterData != null)
                return characterData.runCostEnergyPerTime;
            else return 0;
        }
        set
        {
            characterData.runCostEnergyPerTime = value;
        }
    }

    public int BaseDefence
    {
        get
        {
            if (characterData != null)
                return characterData.baseDefence;
            else return 0;
        }
        set
        {
            characterData.baseDefence = value;
        }
    }

    public int CurrentDefence
    {
        get
        {
            if (characterData != null)
                return characterData.currentDefence;
            else return 0;
        }
        set
        {
            characterData.currentDefence = value;
        }
    }
    #endregion

    #region Character Combat
    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = Mathf.Max(attacker.CurrentDamage() - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);


        if (attacker.isCritical)
        {
            defender.GetComponent<Animator>().SetTrigger("Hit");
        }
        //TODO:Update UI
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);
        //¾­Ñéupdate
        if (CurrentHealth <= 0)
        {
            attacker.characterData.UpdateExp(characterData.killPoint);
        }

    }
    public void TakeDamage(int damage, CharacterStats defender)
    {
        int currentDamage = Mathf.Max(damage - defender.CurrentDefence, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - currentDamage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        //TODO:GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("±©»÷£¡" + coreDamage);
        }

        return (int)coreDamage;
    }
    #endregion    
    #region Character Dodge

    public void CostEnergy(float cost)
    {

        CurrentEnergy = Mathf.Max(Mathf.Min(CurrentEnergy - cost,MaxEnergy),0);
        UpdateEnergyBarOnAttack?.Invoke(CurrentEnergy, MaxEnergy);

        //TODO:GameManager.Instance.playerStats.characterData.UpdateExp(characterData.killPoint);
    }


    #endregion

}
