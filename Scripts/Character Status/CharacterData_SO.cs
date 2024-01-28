using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data", menuName = "Charater Stats/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    public int maxHealth;
    public int currentHealth;    
    public int baseDefence;
    public int currentDefence;
    
    
    [Header("Player Info")]
    public float maxEnergy;
    public float currentEnergy;
    public float altCostEnergy;
    public float jumpCostEnergy;
    public float runCostEnergyPerTime;
    public float healEnergyPerTime;

    [Header("Kill")]
    public int killPoint;

    [Header("Level")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;


    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }
    public void UpdateExp(int point)
    {
        currentExp += point;

        if (currentExp >= baseExp)
            LevelUp();

    }

    private void LevelUp()
    {
        //所有你想提升的数据方法
        currentLevel = Mathf.Clamp(currentLevel + 1, 0, maxLevel);
        baseExp += (int)(baseExp * LevelMultiplier);

        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
        maxEnergy = (float)(maxEnergy * LevelMultiplier);
        currentEnergy = maxEnergy;
        //Debug.Log("LEVEL UP" + currentLevel + "Max Health:" + maxHealth+"Max Energy:"+maxEnergy);
    }
}
