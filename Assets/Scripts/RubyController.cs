using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{
    public static int level;

    public AudioClip winMusic;

    public AudioClip loseMusic;

    public AudioSource winLoseMusic;

    public TextMeshProUGUI cogText;

    private int cogs = 4;

    public TextMeshProUGUI scoreText;

    public TextMeshProUGUI winText;

    public GameObject winTextObject;
   
    public float speed = 3.0f;
    
    public int maxHealth = 5;

    public GameObject hitEffectPrefab;
    
    public GameObject projectilePrefab;
    
    public AudioClip throwSound;
    public AudioClip hitSound;
    
    public int health { get { return currentHealth; }}
    int currentHealth;
    
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    
    AudioSource audioSource;
    
    private int score = 0;

    private bool gameOver = false; 
    
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        winTextObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
        
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        
        Vector2 move = new Vector2(horizontal, vertical);
        
        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            Launch();
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (score == 4)
                 {
                     level = 2;
                     SceneManager.LoadScene("StageTwo");
                 }
                else if (character != null)
                {
                    character.DisplayDialog();
                }
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (gameOver == true)
            {
              SceneManager.LoadScene("MainScene"); // this loads the currently active scene
            }
        }
    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible)
                return;
            
            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject hitEffectObject = Instantiate(hitEffectPrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
           // Destroy(hitEffectObject); 
            PlaySound(hitSound);
        }   
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);

        if (health == 0)
        {
            winText.text = "You lose! Press R to try again!";
            gameOver = true;
            winTextObject.SetActive(true);
            winLoseMusic.clip = loseMusic;
            winLoseMusic.Play();

            Destroy(GetComponent<SpriteRenderer>()); 
            Destroy(GetComponent<BoxCollider2D>());
            speed = 0;    
   

        }

    }
    
    public void ChangeScore(int scoreAmount)
    {
         if (scoreAmount > 0)
        {
           score = score + scoreAmount;
           scoreText.text = "Fixed Robots: " + score.ToString() + "/4";
        }   

       if (score == 4)
        {
          winTextObject.SetActive(true);
        }

       if (score == 4 && level == 2)
        {
          gameOver = true;
          winText.text = "Winner! Created by Chloe Rose Beato";
          winLoseMusic.clip = winMusic;
          winTextObject.SetActive(true);
          winLoseMusic.Play();  

          
         Destroy(GetComponent<SpriteRenderer>()); 
         Destroy(GetComponent<BoxCollider2D>());
         speed = 0; 
        }

    }

    void Launch()
    {
        if (cogs > 0)
        {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 300);

        animator.SetTrigger("Launch");
        PlaySound(throwSound);

        cogs --;
        cogText.text = "Cogs " + cogs.ToString();

        }
    } 
    
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Cogs")
        {
           cogs = cogs + 4;
           cogText.text = "Cogs " + cogs.ToString();
           Destroy(other.gameObject);
        }
    }
}