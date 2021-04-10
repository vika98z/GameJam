using System;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public bool push;
    public bool unPush;

    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        print(other.gameObject.name);
        if (other.gameObject.tag == "Player")
        {
            //push = true;
            //unPush = false;
            
            animator.SetTrigger("Push 0");
            //animator.SetBool("UnPush", unPush);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        print(other.gameObject.name);

        if (other.gameObject.tag == "Player")
        {
            //unPush = true;
            //push = false;
            animator.SetTrigger("UnPush 0");

            //animator.SetBool("Push", push);
            // animator.SetBool("UnPush", unPush);
        }
    }
}
