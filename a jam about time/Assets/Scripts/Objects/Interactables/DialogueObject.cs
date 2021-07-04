using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueObject : MonoBehaviour
{
    [Header("References")]
    public DialogueManager dm;
    private Animator anim;

    [Header("Dialogue Settings")]
    [TextArea(3, 5)]
    public string[] dialogue;
    public bool playerInRange;
    
    // Start is called before the first frame update
    void Start()
    {
        dm = GameObject.Find("Dialogue Manager").GetComponent<DialogueManager>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the player inputs the dialogue button when they are in range
        if (Input.GetKeyDown(KeyCode.E) && !dm.displayingText && playerInRange){
            // Set the manager's dialogue to the current dialogue
            dm.dialogue = dialogue;

            // Start the dialogue sequence in the manager
            StartCoroutine(dm.StartDialogue());

            // Hide the interactable sign
            // anim.SetBool("Interactable", false);
        }
    }

    // Called when an object collides with a trigger collider
    private void OnTriggerEnter2D(Collider2D other) {
        // Check if the object is the player
        if (other.tag == "Player"){
            // Set the player to be in range
            playerInRange = true;

            // Put up the interactable sign
            // anim.SetBool("Interactable", true);
        }
    }   
    
    // Called when an object leaves the trigger collider
    private void OnTriggerExit2D(Collider2D other) {
        // Check if the object is the player
        if (other.tag == "Player"){
            // Set the player to be out of range
            playerInRange = false;
            
            // Take down the interactable sign
            // anim.SetBool("Interactable", false);
        }
    }
}
