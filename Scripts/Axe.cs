using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    public bool activated;
    public float rotationSpeed;
    public int axeThrowDamage;
   
    private void Update()
    {
        if(activated)
        {
            transform.localEulerAngles += Vector3.forward * rotationSpeed * Time.deltaTime;
  
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        activated = false;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<CharacterStats>().TakeDamage(axeThrowDamage, collision.gameObject.GetComponent<CharacterStats>());
            collision.gameObject.gameObject.GetComponent<Animator>().SetBool("AxeHitting",true);
        }//
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().isKinematic = true;
        transform.SetParent(collision.transform);
        //print("collidePos:"+transform.position);
        //print(collision.gameObject.transform);
        //transform.SetParent(collision.gameObject.transform);
    }
}
