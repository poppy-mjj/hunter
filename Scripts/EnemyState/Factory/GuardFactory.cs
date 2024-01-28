using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardFactory : IEnemyStateFactory
{
    public IEnemyState GetState(EnemyInfo enemyInfo)
    {
        return new EnemyGuard(enemyInfo);
    }
}
