using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadFactory : IEnemyStateFactory
{
    public IEnemyState GetState(EnemyInfo enemyInfo)
    {
        return new EnemyDead(enemyInfo);
    }
}
