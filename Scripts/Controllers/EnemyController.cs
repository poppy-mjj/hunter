using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public struct EnemyInfo
{
    public Animator anim;
    public Transform enemyTransform;
    public Vector3 originalPosition;
    public Quaternion originalRotation;
    public GameObject target;
    public NavMeshAgent agent;
    public float remainLookAtTime;
    public CharacterStats enemyStats;
};
public class EnemyController : MonoBehaviour
{

    private EnemyInfo enemyInfo;
    IEnemyState enemyState;
    public GameObject healthHolder;

    [Header("Guard State")]
    public float sightRadius=20;
    public float lookAtTime;
    //CharacterStats enemyStats;

    private void Awake()
    {
        enemyInfo.anim = GetComponent<Animator>();
        enemyInfo.agent = GetComponent<NavMeshAgent>();
        enemyInfo.enemyStats = GetComponent <CharacterStats>();

        enemyInfo.enemyTransform = transform;
        enemyInfo.originalPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        enemyInfo.originalRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
        enemyInfo.remainLookAtTime = lookAtTime;
        GuardFactory guardFactory = new GuardFactory();
        enemyState = guardFactory.GetState(enemyInfo);
        
    }

    // Update is called once per frame
    void Update()
    {
        SwitchStates();

        //print(lastAttackTime);
        if (enemyState != null)
        {
            if (enemyState.ExitCondition())
                enemyState = enemyState.Exit();
        }
        else
        {
            Destroy(gameObject, 5f);
        }
        
    }

    private void SwitchStates()
    {
        enemyState?.UpdateState(FoundPlayer());
    }

    public GameObject FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                enemyInfo.target = target.gameObject;
                return target.gameObject;
            }
        }
        enemyInfo.target = null;
        return null;
    }
    void Hit()
    {

        var targetStats = enemyInfo.target?.GetComponent<CharacterStats>();
        targetStats?.TakeDamage(enemyInfo.enemyStats, targetStats);
        if (healthHolder.active == false)
            healthHolder.active = true;
    }
    void GetAxeHit()
    {
      
            enemyInfo.anim.SetBool("AxeHitting", false);
    }
}
