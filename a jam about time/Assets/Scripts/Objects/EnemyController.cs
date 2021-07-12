using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public LayerMask groundLayer;
    public SpriteRenderer sr;
    private Rigidbody2D rb;
    public Animator anim;
    public AudioSource explode;
    public AudioSource sound;
    public float soundTime;
    public bool playingSound;
    public float soundDistence;
    public bool dead;

    [Header("Movement Settings")]
    public bool floating;
    public float chaseSpeed;
    public float chasingDistance;
    public bool chasing;

    [Header("Patrol Settings")]
    public bool patrolling;
    public float patrolWaitTime;
    public float patrolSpeed;
    public Vector2[] patrolPoints;
    public float margin;
    
    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null )player = GameObject.FindGameObjectWithTag("Player");
        if (Vector2.Distance(transform.position, player.transform.position) < chasingDistance && !dead){
            ChasePlayer();
            if (!chasing){
                StopAllCoroutines();
                playingSound = false;
            }
            chasing = true;
        }   else if (!patrolling && !dead){
            chasing = false;
            StartCoroutine(Patrol());
        }   else if (dead ){
            rb.velocity = Vector2.zero;
        }

        if (Mathf.Abs(rb.velocity.x) > 0.5) sr.flipX = rb.velocity.x > 0;
        
        if (Vector2.Distance(player.transform.position, transform.position) < soundDistence && !playingSound){
            StartCoroutine(PlaySound());      
        }
    }
    IEnumerator PlaySound(){
        playingSound = true;
        sound.Play();
        yield return new WaitForSeconds(soundTime);
        playingSound = false;
    }

    IEnumerator Patrol(){
        patrolling = true;
        anim.SetBool("Attacking", false);
        // Loop through all patrol points
        for (int i = 0; i < patrolPoints.Length; i++){
            // Move player towards the position until they get close to the next patrol point
            while (Vector2.Distance(transform.position, patrolPoints[i]) > margin){
                rb.velocity = new Vector2(patrolSpeed*Mathf.Sign(patrolPoints[i].x-transform.position.x), rb.velocity.y);
                anim.SetBool("IsMoving", true);
                if (floating) DetectWall();
                yield return null;
            }
            rb.velocity = new Vector2(0, rb.velocity.y);
            anim.SetBool("IsMoving", false);
            yield return new WaitForSeconds(patrolWaitTime);
        }
        patrolling = false;
    }
    
    void DetectWall(){
        // Raycast the sides of the bot

        // Move up if player y is above and raycasts hit
    }
    void ChasePlayer(){
        float dir = Mathf.Sign(player.transform.position.x-transform.position.x);
        anim.SetBool("IsMoving", true);
        anim.SetBool("Attacking", true);

        rb.velocity = new Vector2(chaseSpeed*dir, 0);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player"){
            anim.SetTrigger("Explode");
            StopAllCoroutines();
            rb.velocity = Vector2.zero;
            playingSound = true;
            dead = true;
        }
    }
}
