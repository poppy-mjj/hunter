using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackFactory : IEnemyStateFactory
{
    public IEnemyState GetState(EnemyInfo enemyInfo)
    {
        return new EnemyAttack(enemyInfo);
    }
}