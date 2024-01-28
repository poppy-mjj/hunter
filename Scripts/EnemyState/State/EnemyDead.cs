using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDead : IEnemyState
{
    private string name = "isDead";
    private bool isAnim = false;
    EnemyInfo _enemyInfo;

    public EnemyDead(EnemyInfo enemyInfo)
    {
        _enemyInfo = enemyInfo;
    }
    public bool ExitCondition()
    {
        var info = _enemyInfo.anim.GetCurrentAnimatorStateInfo(2);
        
        return info.IsName("Dead")&&info.normalizedTime>1.0f;
    }

    public IEnemyState ExitAndSwitch(IEnemyStateFactory stateFactory)
    {
        isAnim = false;
        return null;
    }

    public void Resposibility()
    {
        isAnim = true;
        _enemyInfo.anim.SetBool(name, isAnim);
    }

    public void UpdateState(GameObject target)
    {
        
        Resposibility();
    }

    public IEnemyState Exit()
    {
        return ExitAndSwitch(null);
    }
}