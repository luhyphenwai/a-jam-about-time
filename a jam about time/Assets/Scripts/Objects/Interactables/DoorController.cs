using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator anim;
    public bool playerInRange;
    public bool open;
    public bool inFuture;
    public bool lastOpen;
    public int direction;

    public GameObject[] doors;
    public AudioSource doorSound;
    
    
    private void Start() {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inFuture) anim.SetBool("PlayerInRange", playerInRange);
        anim.SetInteger("Direction", direction);
        anim.SetBool("InFuture", inFuture);
        anim.SetBool("Open", open);
        if ((!inFuture && (playerInRange && Input.GetKeyDown(KeyCode.E)) )|| lastOpen != open){
            if (lastOpen==open) open = !open;
            for(int i = 0; i<doors.Length; i++){
                BoxCollider2D bc = doors[i].GetComponent<BoxCollider2D>();
                if (bc.enabled){
                    bc.enabled = false;
                    doors[i].GetComponent<Animator>().SetBool("Open", true);
                }   else {
                    bc.enabled = true;
                    doors[i].GetComponent<Animator>().SetBool("Open", false);
                }
                doorSound.Play();
            }
        }

        lastOpen = open;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player"){
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player"){
            playerInRange = false;
        }
    }
}
