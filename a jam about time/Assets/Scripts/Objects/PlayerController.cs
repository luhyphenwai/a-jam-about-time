using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All player movement and control
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public LayerMask groundLayer;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private SpriteRenderer sr;
    private Animator anim;

    [Header("General Settings")]
    public bool movementLocked;


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
    public float pushTime;
    public float pushWait;
    public bool pushRunning;

    [Header("Animation Settings")]
    public float runMargin;

    // Set references
    private void Awake() {
        rb = gameObject.GetComponent<Rigidbody2D>();
        bc = gameObject.GetComponent<BoxCollider2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
        anim = gameObject.GetComponent<Animator>();
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
            rb.velocity = Vector2.zero;
        }   
        oldGrounded = IsGrounded();

        Animations();
        if (!movementLocked){
            PlayerMovement();
        }

        if (IsGrounded()){
            wallJump = false;
        }

        
    }

    // Animations
    void Animations(){

        anim.SetBool("IsGrounded", IsGrounded()); // Set Grounded

        anim.SetFloat("Y Velocity", rb.velocity.y); // So animator knows y velocity

        if (Mathf.Abs(rb.velocity.x) > runMargin && IsGrounded() && !movementLocked){ // Check running
            anim.SetBool("IsRun", true);
        }   else {
            anim.SetBool("IsRun", false);
        }

        // Flip the player in direction
        if (!movementLocked && Mathf.Abs(rb.velocity.x) > runMargin) sr.flipX = rb.velocity.x < 0;
       

        if (rb.velocity.y < -1.5f){
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

        if ((lwall || rwall) && !IsGrounded() && (canHoldWall || onWall)) {
            onWall = (lwall || rwall);
            WallMovement(rwall);

            anim.SetBool("OnWall", true);
        }   else {
            onWall = false;
            Movement();
            anim.SetBool("OnWall", false);
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

            // Set wall timer
            StopAllCoroutines();
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

    IEnumerator RecoveryTimer(){
        movementLocked = true;
        float distance = lastFall - transform.position.y;
        if (distance > jumpRecoveryMargin){
            anim.SetTrigger("Land");
            yield return new WaitForSeconds(jumpRecovery);
        }
        movementLocked = false;
    }

    void PushObjects(){
        // Raycast for walls on either side of player
        RaycastHit2D[] objects = Physics2D.RaycastAll(bc.bounds.center, Vector2.left,bc.bounds.size.x/2 + wallDetectDistance);

        if (Input.GetAxisRaw("Horizontal") == 1){ // Check wall right
            objects = Physics2D.RaycastAll(bc.bounds.center, Vector2.right,bc.bounds.size.x/2 + wallDetectDistance); // Set raycast

            for(int i = 0; i < objects.Length; i++){
                if (objects[i].collider.tag == "Pushable"){
                    
                    if (canPush){
                        velocity.x = pushSpeed;
                        
                    }else {
                        velocity.x = 0;
                    }
                    anim.SetBool("IsRun", true);
                    anim.SetBool("Pushing", true);
                    if (!pushRunning){
                        StartCoroutine(Pushing());
                    }
                    break;
                }   else {
                    anim.SetBool("Pushing", false);
                }
            }
            
        }   else if (Input.GetAxisRaw("Horizontal") == -1){ // Check wall right
            objects = Physics2D.RaycastAll(bc.bounds.center, Vector2.left,bc.bounds.size.x/2 + wallDetectDistance); // Set raycast

            for(int i = 0; i < objects.Length; i++){
                if (objects[i].collider.tag == "Pushable"){
                    if (canPush){
                        velocity.x = -pushSpeed;
                    }   else {
                        velocity.x = 0;
                    }
                    anim.SetBool("IsRun", true);
                    anim.SetBool("Pushing", true);
                    if (!pushRunning){
                        StartCoroutine(Pushing());
                    }
                    break;
                }   else {
                    anim.SetBool("Pushing", false);
                }
            }
        }
    }

    IEnumerator Pushing(){
        pushRunning =true;
        canPush = false;
        yield return new WaitForSeconds(pushWait);
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
