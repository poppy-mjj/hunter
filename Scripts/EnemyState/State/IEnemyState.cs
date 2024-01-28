using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState 
{
    public void UpdateState(GameObject Target);
    public bool ExitCondition();
    public void Resposibility();
    public IEnemyState ExitAndSwitch(IEnemyStateFactory stateFactory);
    public IEnemyState Exit();
}
