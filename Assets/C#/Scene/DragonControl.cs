using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonControl : MonoBehaviour {

    Animator animator;
    ParticleSystem particle;
    public bool moveToward = false;
    float moveSpeed = 10;
	// Use this for initialization
	void Start () {
        animator = GetComponent<Animator>();
        particle = transform.Find("fire").GetComponent<ParticleSystem>();
        particle.Stop();
	}


	// Update is called once per frame
	void Update () {
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    animator.SetBool("isRunning", true);  
        //}
        //if (Input.GetKeyUp(KeyCode.UpArrow))
        //    animator.SetBool("isRunning", false);
        //if (Input.GetKeyDown(KeyCode.A))
        //    animator.SetTrigger("skill1");
        //if (Input.GetKeyDown(KeyCode.B))
        //    animator.SetTrigger("skill2");
        if(moveToward)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

 
    public void Move()
    {
        animator.SetBool("isRunning", true);
        moveToward = true;
    }


    public void StopMove()
    {
        animator.SetBool("isRunning", false);
        moveToward = false;
    }


    public void DoPartical()
    {
        if (!particle.isPlaying)
            particle.Play();
        else
            particle.Stop();
    }

    


}
