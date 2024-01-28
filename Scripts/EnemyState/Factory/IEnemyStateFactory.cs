using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyStateFactory 
{
    IEnemyState GetState(EnemyInfo enemyInfo);
}

