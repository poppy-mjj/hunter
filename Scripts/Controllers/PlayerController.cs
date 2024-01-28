using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public GameObject _mainCamera;
    [SerializeField] private float speed = 1.0f;
    private CharacterStats characterStats;

    private Transform mainCameraTransform;
    private Animator anim;
    private AnimatorStateInfo animatorInfo;
    private Rigidbody rigidBody;
    private Rigidbody axeRigidBody;
    private int boxType = 2;

    public float throwPower = 20;

    private bool isReturning = false;
    private Vector3 armingPosition;
    private Quaternion armingRotation;


    private Vector3[] cameraDirections = new Vector3[5];
    float horizontal;
    float vetical;
    float speedScale = (float)1.0;

    private int currentLayer = 0;

    bool isTransforming;
    int altType = 1;

    bool isWalking = false;
    bool isRunning = false;
    bool isBoxing = false;
    bool isAxeAttack = false;
    bool isAlting = false;
    bool isJumping = false;
    bool isDead = false;
    bool isEquiped = false;
    int isEquipedIdle = 0;
    private bool throwing=false;

    public Vector3 EquipedRot;
    public Transform playerLeftHandBone;
    public Transform playerRightHandBone;
    private Transform weaponTrans;
    public GameObject weaponPrefab;

    private Vector3 oldPos;
    // Is the axe returning? To update the calculations in the Update method
    public Transform middleTarget;
    // The middle point between the axe and the player's hand, to give it a curve
    public Transform curve_point;
    // Last position of the axe before returning it, to use in the Bezier Quadratic Curve formula

    private float returningTime = 0.0f;

    private void Awake()
    {

        mainCameraTransform = _mainCamera.transform;
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        rigidBody = GetComponent<Rigidbody>();
        speedScale = 1;
    }
    private void Start()
    {
        //string prefabPath = "Assets/Prefabs/Axe.prefab";
        
        //weaponPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        axeRigidBody = weaponPrefab.GetComponent<Rigidbody>();
        //axeRigidBody.isKinematic = true;
        if (weaponPrefab != null)
        {
            //weaponTrans = Instantiate(weaponPrefab).transform;
            weaponTrans = weaponPrefab.transform;
            weaponTrans.parent = playerLeftHandBone;
            //-0.036 0.225 -0.22
            //73.388 -135.019 23.81
            weaponTrans.localPosition = new Vector3(-0.036f, 0.225f, -0.22f);
            weaponTrans.localRotation = Quaternion.Euler(73.388f, -135.019f, 23.81f);
            //weaponTrans.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        }
        else
        {
            print("找不到axe");
        }
    }
    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vetical = Input.GetAxis("Vertical");

        JudegeDead();
        if (isDead)
        {
            currentLayer = 2;
        }
        animatorInfo = anim.GetCurrentAnimatorStateInfo(currentLayer);

        if (!isDead)
        {
            EquipInput();
            RunInput();
            DodgeInput();
            AttackInput();
            JumpInput();
            ThrowInput();
            AxeReturnWay();
            if (!isAlting && !isBoxing && !isRunning&&!isAxeAttack)
                characterStats.CostEnergy(-Time.deltaTime * characterStats.HealEnergyPerTime);
        }
        anim.SetBool("isDead", isDead);
        ExitInput();

    }

    private void AxeReturnWay()
    {
        // If the axe is returning
        if (isReturning)
        {
            // Returning calcs
            // Check if we haven't reached the end point, where time = 1
            if (returningTime < 1.0f)
            {
                // Update its position by using the Bezier formula based on the current time
                weaponTrans.position = getBQCPoint(returningTime, oldPos, curve_point.position, middleTarget.position);
                // Reset its rotation (from current to the targets rotation) with 50 units/s
                weaponTrans.rotation = Quaternion.Slerp(weaponTrans.transform.rotation, middleTarget.rotation, 50 * Time.deltaTime);
                // Increase our timer, if you want the axe to return faster, then increase "time" more
                // With something like:
                // time += Timde.deltaTime * 2;
                // It will return as twice as fast
                returningTime += Time.deltaTime;
            }
            else
            {
                // Otherwise, if it is 1 or more, we reached the target so reset
                ResetAxe();
            }
        }
    }
    Vector3 getBQCPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        // "t" is always between 0 and 1, so "u" is other side of t
        // If "t" is 1, then "u" is 0
        float u = 1 - t;
        // "t" square
        float tt = t * t;
        // "u" square
        float uu = u * u;
        // this is the formula in one line
        // (u^2 * p0) + (2 * u * t * p1) + (t^2 * p2)
        Vector3 p = (uu * p0) + (2 * u * t * p1) + (tt * p2);
        return p;
    }

    private void RunInput()
    {
        if (Input.GetKey(KeyCode.LeftShift) && characterStats.CurrentEnergy >= Time.deltaTime * characterStats.RunCostEnergyPerTime)
        {
            characterStats.CostEnergy(Time.deltaTime * characterStats.RunCostEnergyPerTime);
            isWalking = false;
            isRunning = true;
            speedScale = 2;
        }
        else
        {
            isRunning = false;
            speedScale = 1;
        }
    }

    private void AttackInput()
    {
        if (isEquiped)
            AxeInput();
        else
            BoxingInput();

    }

    private void AxeInput()
    {
        if (Input.GetMouseButtonDown(0) && !isAxeAttack && !isAlting && !isRunning)
        {
            isAxeAttack = true;
            weaponTrans.localPosition = new Vector3(0.234f, -0.041f, 0.259f);
            weaponTrans.localRotation = Quaternion.Euler(49.243f, -47.694f, -66.273f);
            if (!isTransforming)
            {
                StartCoroutine(RotateWeapon(Quaternion.Euler(49.243f, -47.694f, -66.273f), new Vector3(0.234f, -0.041f, 0.259f), 1.0f));
            }
        }
        anim.SetBool("isAxeAttack", isAxeAttack);
    }

    private void EquipInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !isBoxing && !isAlting && !isAxeAttack)
        {
            //anim.SetInteger("isEquipedIdle", 0);
            if (!isEquiped)
            {
                isEquiped = true;
                currentLayer = 1;
            }
            else
            {
                isEquiped = false;
            }
        }

        anim.SetBool("isEquiped", isEquiped);
    }

    private void ExitInput()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Start");
        }
    }

    private void JudegeDead()
    {
        if (characterStats.CurrentHealth <= 0)
            isDead = true;
    }

    private void FixedUpdate()
    {
        if (!isDead)
            ProcessInput();
    }

    public void ProcessInput()
    {

        if ((horizontal != 0 || vetical != 0) && !isAlting)
        {
            UpdateRotation();
        }

        HandleMovementInput();


    }




    private void HandleMovementInput()
    {
        isWalking = false;
        if (Input.GetKey(KeyCode.W))
        {
            HandleMovement();
            altType = 1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            HandleMovement();
            altType = 2;
        }

        if (Input.GetKey(KeyCode.D))
        {
            HandleMovement();
            altType = 4;
        }

        if (Input.GetKey(KeyCode.S))
        {
            HandleMovement();
            altType = 3;
        }
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isRunning", isRunning);
    }

    void HandleMovement()
    {
        isWalking = !isRunning;

        if (!isBoxing && !isAlting)
        {
            // 移动角色
            rigidBody.velocity = transform.forward.normalized * speed * speedScale;//* Time.deltaTime;
            //print(direction * speed * Time.deltaTime * speedScale);
        }
    }
    private void OnAnimatorMove()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("alt") || anim.GetCurrentAnimatorStateInfo(0).IsTag("jump"))
        {
            transform.position += anim.deltaPosition;
            transform.rotation *= anim.deltaRotation;
        }
    }

    private void UpdateRotation()
    {
        cameraDirections[0] = new Vector3(mainCameraTransform.forward.x, 0, mainCameraTransform.forward.z);
        cameraDirections[1] = new Vector3(-mainCameraTransform.right.x, 0, -mainCameraTransform.right.z);
        cameraDirections[2] = new Vector3(-mainCameraTransform.forward.x, 0, -mainCameraTransform.forward.z);
        cameraDirections[3] = new Vector3(mainCameraTransform.right.x, 0, mainCameraTransform.right.z);

        Quaternion targetRotation = Quaternion.LookRotation(cameraDirections[altType - 1], Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3);
    }
    void AnimEnd()
    {
        if (isBoxing)
            if (animatorInfo.IsTag("box"))
            {
                isBoxing = false;
                anim.SetBool("isBoxing", isBoxing);
            }
        if (isAxeAttack)
            if (animatorInfo.IsTag("attack"))
            {
                isAxeAttack = false;
                anim.SetBool("isAxeAttack", isAxeAttack);
            }
        if (isAlting)
            if (animatorInfo.IsTag("alt"))
            {
                isAlting = false;
                anim.SetBool("isAlting", isAlting);
            }
        if (isJumping)
            if (animatorInfo.IsTag("jump"))
            {
                isJumping = false;
                anim.SetBool("isJumping", isJumping);
            }
        if (isDead)
            SceneManager.LoadScene("Start");
    }

    private void JumpInput()
    {
        //---判断是否按下空格
        if (Input.GetKeyDown(KeyCode.Space) && !isAlting && !isBoxing && characterStats.CurrentEnergy >= characterStats.JumpCostEnergy)
        {
            characterStats.CostEnergy(characterStats.JumpCostEnergy);
            isJumping = true;
        }
        anim.SetBool("isJumping", isJumping);
    }
    private void ThrowInput()
    {
        if (Input.GetKeyDown(KeyCode.R) && isEquiped&&!isAxeAttack)
        {
            isEquipedIdle = 1;
            //isEquiped = false;
            //anim.SetBool("isEquiped", false);
            //anim.SetInteger("isEquipedIdle", 1);

            throwing = true;
            weaponTrans.localPosition = new Vector3(0.234f, -0.041f, 0.259f);
            weaponTrans.localRotation = Quaternion.Euler(49.243f, -47.694f, -66.273f);
            if (!isTransforming)
            {
                StartCoroutine(RotateWeapon(Quaternion.Euler(49.243f, -47.694f, -66.273f), new Vector3(0.234f, -0.041f, 0.259f), 2.0f));
            }
            //if(!isTransforming)
            //StartCoroutine(RotateWeapon(new Quaternion(Quaternion.Inverse(weaponTrans.rotation).x, Quaternion.Inverse(weaponTrans.rotation).y, Quaternion.Inverse(weaponTrans.rotation).z, Quaternion.Inverse(weaponTrans.rotation).w), new Vector3(0.016f, -0.241f, 0.372f), 3.0f));
        }
        if(Input.GetKeyDown(KeyCode.F))
        {
            weaponTrans.SetParent(null);
            AxeReturn();

            
        }
        anim.SetBool("Throw", throwing);
    }
    private void BoxingInput()
    {

        if (Input.GetMouseButtonDown(0) && !isBoxing && !isAlting && !isRunning)
        {
            isBoxing = true;
            ToggleBoxingType();
        }
        anim.SetBool("isBoxing", isBoxing);
        anim.SetInteger("boxType", boxType);
    }
    private void DodgeInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (!isBoxing && !isAlting && !isEquiped && characterStats.CurrentEnergy >= characterStats.AltCostEnergy)
            {
                characterStats.CostEnergy(characterStats.AltCostEnergy);
                isAlting = true;
                characterStats.CurrentDefence = 3 * characterStats.BaseDefence;
            }

        }
        if (!isAlting)
            characterStats.CurrentDefence = characterStats.BaseDefence;
        anim.SetBool("isAlting", isAlting);
        anim.SetInteger("altType", altType);
    }
    private void ToggleBoxingType()
    {
        if (boxType == 1)
            boxType = 2;
        else if (boxType == 2)
            boxType = 1;

    }
    public GameObject AttackTarget()
    {
        var colliders = Physics.OverlapSphere(transform.position, isBoxing?characterStats.attackData.attackRange:characterStats.attackData.skillRange);
        foreach (var target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                if (InAttackAngle(target))
                {

                    return target.gameObject;
                }
            }
        }
        return null;
    }

    public bool InAttackAngle(Collider target)
    {
        return Vector3.Angle(target.transform.position - transform.position, transform.forward) <= characterStats.attackData.attackAngle;

    }

    //Animation Event 
    void Hit()
    {

        var targetStats = AttackTarget()?.GetComponent<CharacterStats>();
        targetStats?.TakeDamage(characterStats, targetStats);

    }

    void AttackEnding()
    {

        if (!isTransforming)
        {
            StartCoroutine(RotateWeapon(Quaternion.Euler(EquipedRot.x, EquipedRot.y, EquipedRot.z), new Vector3(-0.195f, 0.258f, 0.264f), 3.0f));
        }

    }
    void ChangeBind()
    {
        weaponTrans.parent = playerRightHandBone;
        //weaponTrans.localPosition = new Vector3(-0.07f, 0.18f, -0.30f);
        //-0.195  0.258 0.264
        //53.085 -46.604  7.873
        //weaponTrans.localPosition = new Vector3(-0.195f, 0.258f, 0.264f);
        armingPosition = new Vector3(weaponTrans.localPosition.x, weaponTrans.localPosition.y, weaponTrans.localPosition.z);
        armingRotation = new Quaternion(weaponTrans.localRotation.x, weaponTrans.localRotation.y, weaponTrans.localRotation.z, weaponTrans.localRotation.w);
        if (!isTransforming)
        {

            StartCoroutine(RotateWeapon(Quaternion.Euler(EquipedRot.x, EquipedRot.y, EquipedRot.z), new Vector3(-0.195f, 0.258f, 0.264f), 2.0f));
        }
        //weaponTrans.localRotation = Quaternion.Euler(EquipedRot.x,EquipedRot.y,EquipedRot.z);


    }
    // 协程来执行平滑旋转
    IEnumerator RotateWeapon(Quaternion targetRotation, Vector3 targetPosition, float transformSpeed)
    {
        isTransforming = true;
        // 获取当前旋转
        Quaternion startRotation = weaponTrans.localRotation;
        Vector3 startPosition = weaponTrans.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < 1f)
        {
            // 使用Lerp进行插值旋转
            weaponTrans.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime);
            weaponTrans.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            // 累加经过的时间
            elapsedTime += Time.deltaTime * transformSpeed;

            yield return null;
        }

        // 设置最终的旋转

        weaponTrans.localPosition = targetPosition;
        weaponTrans.localRotation = targetRotation;
        isTransforming = false;

    }

    void ChangeBackBind()
    {

        if (!isTransforming)
        {
            StartCoroutine(RotateWeapon(armingRotation, armingPosition, 2.0f));
            //StartCoroutine(RotateWeapon(Quaternion.Euler(73.388f, -135.019f, 23.81f), new Vector3(-0.036f, 0.225f, -0.22f), 2.0f));
        }

        //weaponTrans.localPosition = new Vector3(-0.036f, 0.225f, -0.22f);
        //weaponTrans.localRotation = Quaternion.Euler(73.388f, -135.019f, 23.81f);
    }
    void BackLayer()
    {
        weaponTrans.parent = playerLeftHandBone;
        weaponTrans.localPosition = new Vector3(-0.036f, 0.225f, -0.22f);
        weaponTrans.localRotation = Quaternion.Euler(73.388f, -135.019f, 23.81f);
        currentLayer = 0;
    }

    void AxeThrow()
    {
        isReturning = false;
       
        weaponTrans.parent = null;
        
        axeRigidBody.isKinematic = false;
        axeRigidBody.velocity = Vector3.zero;
        //weaponTrans.rotation =new Quaternion(0,0,0,0);
        //weaponPrefab.GetComponent<BoxCollider>().enabled = true;
        weaponPrefab.GetComponent<Axe>().activated = true;
        weaponPrefab.GetComponent<TrailRenderer>().enabled = true;
        //Debug.Log("Camera Forward: " + mainCameraTransform.forward);//TransformDirection(Vector3.forward));
        //axeRigidBody.velocity = mainCameraTransform.forward; //mainCameraTransform.TransformDirection(Vector3.forward) * throwPower;
        axeRigidBody.AddForce(mainCameraTransform.TransformDirection(Vector3.forward) * throwPower, ForceMode.Impulse);

        throwing = false;
    }
    void AxeReturn()
    {
        //isEquiped = true;
        //anim.SetInteger("isEquipedIdle", 2);
        // We are returning the axe; so it is in its first point where time = 0
        returningTime = 0.0f;
        // Save its last position to refer to it in the Bezier formula
        oldPos = axeRigidBody.position;
        isReturning = true;
        axeRigidBody.velocity = Vector3.zero;
        axeRigidBody.isKinematic = true;
        //weaponPrefab.GetComponent<BoxCollider>().enabled = true;
        weaponPrefab.GetComponent<Axe>().activated = true ;
        

    }
    // Reset axe
    void ResetAxe()
    {
        //weaponPrefab.GetComponent<BoxCollider>().enabled = false;
        weaponPrefab.GetComponent<Axe>().activated = false;
        // Axe has reached, so it is not returning anymore
        isReturning = false;
        // Attach back to its parent, in this case it will attach it to the player (or where you attached the script to)
        axeRigidBody.transform.parent = transform;
        // Set its position to the target's
        axeRigidBody.position = middleTarget.position;
        // Set its rotation to the target's
        axeRigidBody.rotation = middleTarget.rotation;
        weaponTrans.parent = playerRightHandBone;
        weaponTrans.localPosition=new Vector3(-0.195f, 0.258f, 0.264f);
        weaponTrans.localRotation = Quaternion.Euler(EquipedRot.x, EquipedRot.y, EquipedRot.z);
        weaponPrefab.GetComponent<TrailRenderer>().enabled = false;
        //StartCoroutine(RotateWeapon(Quaternion.Euler(EquipedRot.x, EquipedRot.y, EquipedRot.z), new Vector3(-0.195f, 0.258f, 0.264f), 3.0f));
    }

    void AxeAttack()
    {
        var targetStats = AttackTarget()?.GetComponent<CharacterStats>();
        characterStats.isCritical = true;
        targetStats?.TakeDamage(characterStats, targetStats);
        print(targetStats.CurrentHealth);
        characterStats.isCritical = false;
    }

}
