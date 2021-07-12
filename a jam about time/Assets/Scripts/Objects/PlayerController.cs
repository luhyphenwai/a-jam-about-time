using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All player movement and control
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public LayerMask groundLayer;
    public LayerMask enemyLayer;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private Animator anim;
    private GameManager gm;

    [Header("General Settings")]
    public bool movementLocked;

    [Header("Death Settings")]
    public bool dead;
    public float deathTime;
    public Vector2 deathVelocity;


    [Header("Wall Movement Settings")]
    public float wallSlideSpeed;
    public Vector2 wallJumpVelocity;
    public float wallDetectDistance;
    public bool onWall;
    public bool wallJump;
    public float wallTime;
    public bool canHoldWall;
    
    [Header("Air Settings")]
    public float airAccel;
    public bool oldGrounded;

    [Header("Falling Settings")]
    public bool wasFalling;
    public float lastFall;
    public float jumpRecoveryMargin;
    public float jumpRecovery;
    public float maxJumpRecovery;

    [Header("Movement Settings")]
    public float speed;
    public float sSpeed;
    public float jumpHeight;
    public float fallMultiplier;
    public float jumpMultiplier;
    public Vector2 velocity;

    [Header("Interaction Settings")]
    public float pushSpeed;
    public bool canPush;
    public float pullBoost;
    public float pushTime;
    public float pushWait;
    public bool pushRunning;
    public float lastPushDirection;

    [Header("Animation Settings")]
    public float lastPosition;

    [Header("Audio Settings")]
    public AudioSource run;
    public float runTime;
    public bool runBool;
    public AudioSource jump;
    public AudioSource land;
    public AudioSource death;

    // Set references
    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
        if (GameObject.FindGameObjectWithTag("GameManager") != null ){
            gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        }
    }

    // Check ground
    public bool IsGrounded(){
        RaycastHit2D groundCast = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return groundCast == true;
    }

    // Update is called once per frame
    void Update()
    {

        // Set up jump recovery
        if (!oldGrounded && IsGrounded()){
            
            StartCoroutine(RecoveryTimer());
            land.Play();
            rb.velocity = Vector2.zero;
        }   
        oldGrounded = IsGrounded();

        if (!dead){
            Animations();
        }
        
        if (!movementLocked && !dead){
            PlayerMovement();
        }   else if (!dead){
            // Do gravity reduce
            if (!IsGrounded() && rb.velocity.y > 0){
                velocity = rb.velocity;
                velocity.y += rb.gravityScale * jumpMultiplier * Time.deltaTime;
                rb.velocity = velocity;
            }
        }

        if (IsGrounded()){
            wallJump = false;
        }

        // Check for enemies 
        RaycastHit2D enemyCast = Physics2D.BoxCast(bc.bounds.center, bc.bounds.size, 0, Vector2.down, 0f, enemyLayer);
        if (enemyCast && !dead){
            StartCoroutine(Dead(enemyCast.collider.gameObject));
            dead = true;
        }

        if (Input.GetAxisRaw("Horizontal") != 0 && IsGrounded() && !pushRunning && !runBool ){
            StartCoroutine(RunSound());
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy"){
            StartCoroutine(Dead(other.collider.gameObject));
            dead = true;
        } 
    }
    IEnumerator RunSound(){
        runBool = true;
        if (Input.GetAxisRaw("Horizontal") != 0 && IsGrounded()){
            run.Play();
            yield return new WaitForSeconds(runTime);
            runBool = false;
        }

        runBool = false;
    }
    IEnumerator Dead(GameObject enemy){
        rb.velocity = new Vector2(deathVelocity.x * Mathf.Sign(transform.position.x - enemy.transform.position.x), deathVelocity.y);
        if ( Mathf.Sign(transform.position.x - enemy.transform.position.x) >= 0){
            anim.SetTrigger("DeadRight");
        }   else {
            anim.SetTrigger("DeadLeft");
        }
        death.Play();
        yield return new WaitForSeconds(deathTime);
        gm.Reset();
    }

    // Animations
    void Animations(){

        anim.SetBool("IsGrounded", IsGrounded()); // Set Grounded

        anim.SetFloat("Y Velocity", rb.velocity.y); // So animator knows y velocity

        if (Input.GetAxisRaw("Horizontal") != 0 && IsGrounded() && !movementLocked){ // Check running
            anim.SetBool("IsRun", true);
        }   else {
            anim.SetBool("IsRun", false);
        }

        lastPosition = rb.position.x;
        // Flip the player in direction
        if (!movementLocked && Mathf.Abs(rb.velocity.x) > 0.5f) sr.flipX = rb.velocity.x < 0;
       

        if (rb.velocity.y < -1f){
            if (!wasFalling){
                anim.SetTrigger("Falling");
                lastFall = transform.position.y;
            }
            wasFalling = true;
        }   else {
            wasFalling = false;
        }
        
    }

    // Player movement
    void PlayerMovement(){
        // Raycast for walls on either side of player
        RaycastHit2D lwall = Physics2D.Raycast(bc.bounds.center, Vector2.left,bc.bounds.size.x/2 + wallDetectDistance, groundLayer);
        RaycastHit2D rwall = Physics2D.Raycast(bc.bounds.center, Vector2.right,bc.bounds.size.x/2 + wallDetectDistance, groundLayer);

        // Check if conditions are right for wall
        if ((lwall || rwall) && !IsGrounded() && (canHoldWall || onWall)) {
            onWall = (lwall || rwall); // Record if player is on wall
            WallMovement(rwall); // Call wall movement function
            anim.SetBool("OnWall", true); // Set animation
        }   else {
            onWall = false; // Record that the player is not on wall
            Movement(); // Run regular movement
            anim.SetBool("OnWall", false); // Set aniamtion
        }
    }

    IEnumerator WallJumpTime(){
        canHoldWall = false;
        yield return new WaitForSeconds(wallTime);
        canHoldWall = true;
    }

    void Movement(){
        // Get player movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        // Set velocity to that if it doesn't change the player velocity doesn't cahnge
        velocity = rb.velocity;
        
        // Set horizontal movement
        if (!IsGrounded()){
            velocity.x += input.x * airAccel * Time.deltaTime; // Air acceleration
            if (wallJump){
                velocity.x = Mathf.Clamp(velocity.x, -wallJumpVelocity.x, wallJumpVelocity.x); // Wall Jump
            }
            else{
                velocity.x = Mathf.Clamp(velocity.x, -speed, speed); // Regular speed
            }
        }    
        else {
            velocity.x = input.x * speed; // Regular speed
        }

        // Check for vertical input
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && IsGrounded()){
            //  Move vertically
            velocity.y = jumpHeight; 

            // Set Animation
            anim.SetTrigger("Jump");
            jump.Play();

            // Set wall timer
            StopAllCoroutines();
            runBool = false;
            canPush = true;
            pushRunning = false;
            StartCoroutine(WallJumpTime());
        }


        // Better jumping
        BetterJumping();
        PushObjects();

        // Set velocity
        rb.velocity = velocity;
    }

    // Landing recovery
    IEnumerator RecoveryTimer(){
        // Lock the players movement
        movementLocked = true;

        // Check how far the player has fallen
        float distance = lastFall - transform.position.y;

        // Pause if the distence is above the jump recovery margin
        if (distance > jumpRecoveryMargin){
            // Animation trigger
            anim.SetTrigger("Land");

            // Wait for time
            yield return new WaitForSeconds(jumpRecovery);
        }

        // Unlock movement
        movementLocked = false;
    }

    // Detect and push nearby objects
    void PushObjects(){
        // Raycast for walls on either side of player
        RaycastHit2D[] lwall = Physics2D.RaycastAll(bc.bounds.center, Vector2.left,bc.bounds.size.x/2 + wallDetectDistance); // Set left raycast
        RaycastHit2D[] rwall = Physics2D.RaycastAll(bc.bounds.center, Vector2.right,bc.bounds.size.x/2 + wallDetectDistance); // Set right raycast

        if (Input.GetKey(KeyCode.E)){
            bool foundSide = false;
            // Check left wall
            for(int i = 0; i < lwall.Length; i++){
                if (lwall[i].collider.tag == "Pushable"){
                    // Flip x
                    sr.flipX = true;

                    foundSide = true;
                    float input = Input.GetAxisRaw("Horizontal");
                    anim.SetBool("Pushing", true);
                    anim.SetBool("Pulling", true);
                    if (canPush && input != 0){
                        velocity.x = pushSpeed * input; // Set velocity
                        anim.SetBool("IsRun", true); // Set animations

                        // Reset timer if not the same as input
                        if (lastPushDirection != input){
                            pushRunning = false;
                            canPush = true;
                            StopAllCoroutines();
                            runBool = false;
                        }
                        lastPushDirection = input; // Record pushing direction
                        

                        // Set pushing or pulling animation
                        if (input == -1){
                            anim.SetBool("Pushing", true);
                            anim.SetBool("Pulling", false);

                            lwall[i].collider.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x-pullBoost, lwall[i].collider.GetComponent<Rigidbody2D>().velocity.y);
                        }   else {
                            anim.SetBool("Pushing", false);
                            anim.SetBool("Pulling", true);

                            lwall[i].collider.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x+pullBoost, lwall[i].collider.GetComponent<Rigidbody2D>().velocity.y);

                        }

                        if (!pushRunning){
                            StartCoroutine(Pushing());
                        }
                        
                    }else {
                        velocity.x = 0;
                    }
                    
                    break;
                }  else {
                    
                    anim.SetBool("Pushing", false);
                    anim.SetBool("Pulling", false);
                }
            }

            // Check right wall
            
            for(int i = 0; i < rwall.Length; i++){
                if (foundSide) break;
                
                if (rwall[i].collider.tag == "Pushable"){
                    // Flip x
                    sr.flipX = false;
                    
                    float input = Input.GetAxisRaw("Horizontal");
                    anim.SetBool("Pushing", true);
                    anim.SetBool("Pulling", true);
                    if (canPush && input != 0){
                        velocity.x = pushSpeed * input;
                        anim.SetBool("IsRun", true);
                        lastPushDirection = input;

                        // Set pushing or pulling animation and move object
                        if (input == 1){
                            anim.SetBool("Pushing", true);
                            anim.SetBool("Pulling", false);

                            rwall[i].collider.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x+pullBoost, rwall[i].collider.GetComponent<Rigidbody2D>().velocity.y);
                        }   else {
                            anim.SetBool("Pushing", false);
                            anim.SetBool("Pulling", true);
                            
                            rwall[i].collider.GetComponent<Rigidbody2D>().velocity = new Vector2(velocity.x-pullBoost, rwall[i].collider.GetComponent<Rigidbody2D>().velocity.y);
                        }

                        if (!pushRunning){
                            StartCoroutine(Pushing());
                        }
                        
                    }else {
                        velocity.x = 0;
                    }
                    
                    break;
                }  else {
                    anim.SetBool("Pushing", false);
                    anim.SetBool("Pulling", false);
                }
            }
        }   else {
            anim.SetBool("Pushing", false);
            anim.SetBool("Pulling", false);
        }
    }

    IEnumerator Pushing(){
        pushRunning =true;
        canPush = false;
        yield return new WaitForSeconds(pushWait);
        run.Play();
        canPush = true;
        yield return new WaitForSeconds(pushTime);
        pushRunning = false;
    }

    void BetterJumping(){
        // Increase fall speed
        if (rb.velocity.y < 0){
            velocity.y += -rb.gravityScale * fallMultiplier * Time.deltaTime;
        }

        // Increase jump when holding down button
        if (rb.velocity.y > 0 && (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W))){
            velocity.y += rb.gravityScale * jumpMultiplier * Time.deltaTime;
        }   
    }

    // Player wall movement
    void WallMovement(bool right){
        
        int dir = (System.Convert.ToInt32(right)*2)-1;
        // Get player movement
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity = rb.velocity;

        // Flip player in direction opposite to wall
        if (right){
            sr.flipX = true;
        }   else {
            sr.flipX = false;
        }
        
        // Set horizontal movement
        if (Input.GetKey(KeyCode.LeftShift)){
            velocity.x = input.x * sSpeed; // Sprint speed
        }   else {
            velocity.x = input.x * speed; // Regular speed
        }

        // Move down at constant speed
        velocity.y = -wallSlideSpeed;

        // Check for jump key and jump opposite to the wall
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)){
            wallJump = true;
            // Set Animation
            anim.SetTrigger("Jump");
            jump.Play();

            // Move player off wall
            transform.position = new Vector2(transform.position.x+(-(wallDetectDistance+0.1f)*dir), transform.position.y);

            // Set x velocity
            velocity.x = -wallJumpVelocity.x*dir;

            // Set y velocity
            velocity.y = wallJumpVelocity.y;
        }
        rb.velocity = velocity;
    }


}
