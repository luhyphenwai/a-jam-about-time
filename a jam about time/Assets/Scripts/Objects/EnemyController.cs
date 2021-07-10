using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public LayerMask groundLayer;
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public bool floating;
    public float chaseSpeed;
    public float chasingDistance;

    [Header("Patrol Settings")]
    public bool patrolling;
    public float patrolWaitTime;
    public float patrolSpeed;
    public Vector2[] patrolPoints;
    public float margin;
    
    private void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, player.transform.position) < chasingDistance){
            ChasePlayer();
            StopAllCoroutines();
        }   else if (!patrolling){
            StartCoroutine(Patrol());
        }
    }

    IEnumerator Patrol(){
        patrolling = true;
        // Loop through all patrol points
        for (int i = 0; i < patrolPoints.Length; i++){
            // Move player towards the position until they get close to the next patrol point
            while (Vector2.Distance(transform.position, patrolPoints[i]) > margin){
                rb.velocity = new Vector2(patrolSpeed*Mathf.Sign(patrolPoints[i].x-transform.position.x), rb.velocity.y);
                if (floating) DetectWall();
                yield return null;
            }
            rb.velocity = new Vector2(0, rb.velocity.y);
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
        rb.velocity = new Vector2(chaseSpeed*dir, 0);
    }
}
