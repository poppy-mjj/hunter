using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuard : IEnemyState
{

    private string name = "isGuard";
    private bool isAnim = false;
    EnemyInfo _enemyInfo;
    private bool isFound;
    
    public EnemyGuard(EnemyInfo enemyInfo)
    {
        _enemyInfo = enemyInfo;
    }

    public bool ExitCondition()
    {
        if (isFound)
            return true;
        return
            false;
    }

    public IEnemyState ExitAndSwitch(IEnemyStateFactory stateFactory)
    {
        isAnim = false;
        _enemyInfo.anim.SetBool(name, isAnim);
        return stateFactory.GetState(_enemyInfo);
    }
    public IEnemyState Exit()
    {
        return ExitAndSwitch(new AttackFactory());
    }
    public void Resposibility()
    {
        
        Vector3 guardPos = _enemyInfo.originalPosition;
        Quaternion guardRotation = _enemyInfo.originalRotation;
        //Debug.Log(_enemyInfo.enemyTransform.position != guardPos);
        if (_enemyInfo.enemyTransform.position != guardPos)
        {
            isAnim = true;
            _enemyInfo.agent.isStopped = false;
            _enemyInfo.agent.destination = guardPos;

            //比distance节省开销
            if (Vector3.SqrMagnitude(guardPos - _enemyInfo.enemyTransform.position) <= _enemyInfo.agent.stoppingDistance)
            {
                isAnim = false;
                _enemyInfo.enemyTransform.rotation = Quaternion.Lerp(_enemyInfo.enemyTransform.rotation, guardRotation, 0.01f);
            }

        }

        
    }

    public void UpdateState(GameObject target)
    {
        isFound = target!=null;
        _enemyInfo.target = target;
        Resposibility();
        _enemyInfo.anim.SetBool(name, isAnim);

    }
}
