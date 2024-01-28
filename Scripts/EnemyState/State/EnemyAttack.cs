using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : IEnemyState
{
    private string name = "isAttack";
    private bool isAnim = false;
    private bool isFollow = false;
    private bool isAttacking = false;
    EnemyInfo _enemyInfo;
    private bool isFound;
    private float _lastAttackTime;
    public GameObject healthHolder;
    public EnemyAttack(EnemyInfo enemyInfo)
    {
        _enemyInfo = enemyInfo;
    }
    public bool ExitCondition()
    {
        //Debug.Log(_enemyInfo.target.GetComponent<CharacterStats>().CurrentHealth);
        return (!isAnim)|| _enemyInfo.enemyStats.CurrentHealth <= 0;
    }

    public IEnemyState ExitAndSwitch(IEnemyStateFactory stateFactory)
    {
        isAnim = false;
        _enemyInfo.anim.SetBool(name, isAnim);
        return stateFactory.GetState(_enemyInfo);
    }

    public IEnemyState Exit()
    {
        if (_enemyInfo.enemyStats.CurrentHealth <= 0)
            return ExitAndSwitch(new DeadFactory());
        return ExitAndSwitch(new GuardFactory());
    }
    public void Resposibility()
    {
        _lastAttackTime -= Time.deltaTime;

        isAnim = true;
        if (!isFound)
        {
            isFollow=false;
            if (_enemyInfo.remainLookAtTime > 0)
            {
                _enemyInfo.agent.destination = _enemyInfo.enemyTransform.position;
                _enemyInfo.remainLookAtTime -= Time.deltaTime;
            }
            else
            {
                isAnim = false;
            }


        }
        else
        {
            if(!_enemyInfo.anim.GetCurrentAnimatorStateInfo(1).IsName("attack"))
            {
                _enemyInfo.agent.speed=5;//TODO:run velocity
                //Debug.Log(_enemyInfo.anim.GetCurrentAnimatorStateInfo(0));
                isFollow = true;
                _enemyInfo.agent.isStopped = false;
                _enemyInfo.agent.destination = _enemyInfo.target.transform.position;
            }
            else
            {
                if (_enemyInfo.anim.GetCurrentAnimatorStateInfo(1).normalizedTime > 1.0f)
                {
                    isAttacking = false;
                }
            }
        }

        //ÔÚ¹¥»÷·¶Î§ÄÚÔò¹¥»÷
        if (TargetInAttackRange())
        {
            isFollow = false;
            _enemyInfo.agent.isStopped = true;
            _enemyInfo.agent.destination = _enemyInfo.enemyTransform.position;

            //Debug.Log(_lastAttackTime);
            if (_lastAttackTime < 0)
            {
                _lastAttackTime = _enemyInfo.enemyStats.attackData.coolDown;
                
                //Ö´ÐÐ¹¥»÷
                Attack();
            }

        }
    }
    void Attack()
    {
        _enemyInfo.enemyTransform.LookAt(_enemyInfo.target.transform);
        if (TargetInAttackRange())
        {
            isAttacking = true;
            
            //½üÉí¹¥»÷¶¯»­
            
            
        }
    }
    bool TargetInAttackRange()
    {
        if (_enemyInfo.target != null)
            return Vector3.Distance(_enemyInfo.target.transform.position, 
                _enemyInfo.enemyTransform.position) 
                <= _enemyInfo.enemyStats.attackData.attackRange;
        else
            return false;
    }
    public void UpdateState(GameObject target)
    {
        isFound = target!=null;
        _enemyInfo.target = target;
        Resposibility();
        _enemyInfo.anim.SetBool(name, isAnim);
        _enemyInfo.anim.SetBool("Follow", isFollow);
        _enemyInfo.anim.SetBool("isAttacking", isAttacking);

    }
}