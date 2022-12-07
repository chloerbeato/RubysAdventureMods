using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class BeeController : MonoBehaviour
{
    public float speedBee;
    public bool vertical;
    public float changeTime = 3.0f;

    public AudioClip beeBuzz;
    
    Rigidbody2D rigidbody2D;
    float timer;
    int direction = 1;
    bool broken = true;

    public float slowRemaining = 10;
    public bool slowIsRunning = false;

    
    Animator animator;
    private RubyController rubyController;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();

        
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");

        if (rubyControllerObject != null)

        {

            rubyController = rubyControllerObject.GetComponent<RubyController>();

           // print ("Found the RubyController Script!");

        }

        if (rubyController == null)

        {

          //  print ("Cannot find GameController Script!");
        }
    }

    void Update()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }
    
    void FixedUpdate()
    {
        //remember ! inverse the test, so if broken is true !broken will be false and return won’t be executed.
        if(!broken)
        {
            return;
        }
        
        Vector2 position = rigidbody2D.position;
        
        if (vertical)
        {
            position.y = position.y + Time.deltaTime * speedBee * direction;
            animator.SetFloat("Move X", 0);
           // animator.SetFloat("Move Y", direction);
        }
        else
        {
            position.x = position.x + Time.deltaTime * speedBee * direction;
            animator.SetFloat("Move X", direction);
           // animator.SetFloat("Move Y", 0);
        }
        
        rigidbody2D.MovePosition(position);
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();
        if (player != null)
        {
            player.ChangeHealth(-1);
            player.speed=1.0f; 
            player.PlaySound(beeBuzz);
        }
    }
    

}
