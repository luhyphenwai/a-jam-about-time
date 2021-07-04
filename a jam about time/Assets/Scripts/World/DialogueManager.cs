using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Used to display all dialogue
public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    public TMP_Text text;
    private Animator anim;
    public GameObject player;

    [Header("General Settings")]
    public bool displayingText;
    public bool writingText;
    public string[] dialogue;
    public int currentDialogueIndex;

    [Header("Animation Settings")]
    public float textStartTime;
    public float textEndTime;
    public float textAnimTime;
    public float textWriteSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // Get initial references
        anim = gameObject.GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Displaying current dialogue
        DisplayDialogue();
    }

    // Displaying Dialogue
    void DisplayDialogue(){
        // Only display text when needed
        if (displayingText){
            // When displaying text, check for input to continue or end text
            if (Input.GetKeyDown(KeyCode.E) && !writingText){
                if (currentDialogueIndex >= dialogue.Length){
                    StartCoroutine(EndDialogue());
                }   else {
                    StartCoroutine(TextAnimation());
                }
            }   else if (Input.GetKeyDown(KeyCode.E) && writingText){
                text.text = dialogue[currentDialogueIndex-1];
                writingText = false;
            }

        }   
    }

    // Coroutine that runs to start dialogue
    public IEnumerator StartDialogue(){
        // Run animation to show text
        // anim.SetBool("ShowText", true);

        // Set the current dialogue index as the first dialogue
        currentDialogueIndex = 0;
    
        // Lock player movement
        player.GetComponent<PlayerController>().movementLocked = true;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, player.GetComponent<Rigidbody2D>().velocity.y);

        // Wait for tehe time it takes for the animation to run
        yield return new WaitForSeconds(textStartTime);

        // Start displaying text and checking for input
        displayingText = true;
        StartCoroutine(DisplayText());
        currentDialogueIndex += 1;
    }
    IEnumerator DisplayText(){
        text.text = "";
        writingText = true;
        char[] charArray = dialogue[currentDialogueIndex].ToCharArray(); 
        for (int i = 0; i < charArray.Length; i++){
            
            text.text += charArray[i].ToString();
            
            if (!writingText){
                break;
            }

            yield return new WaitForSeconds(textWriteSpeed);
        }
        writingText = false;
    }

    // Coroutine that runs to end dialogue
    public IEnumerator EndDialogue(){
        // Run animation to hide text
        // anim.SetBool("ShowText", false);
        
        // Unlock player movement
        player.GetComponent<PlayerController>().movementLocked = false;

        // Wait for the text animation end time
        yield return new WaitForSeconds(textStartTime);
        displayingText = false;
        writingText = false;
        text.text = "";
    }

    // Coroutine that runs whenever the text is transitioning
    public IEnumerator TextAnimation(){
        // anim.SetTrigger("ChangeText");
        yield return new WaitForSeconds(textAnimTime);
        StartCoroutine(DisplayText());
        currentDialogueIndex += 1;
    }

}
