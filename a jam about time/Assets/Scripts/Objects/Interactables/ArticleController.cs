using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticleController : MonoBehaviour
{
    public GameObject article;
    public PlayerController player;
    public float speed;

    public Vector2 hiddenPosition;
    public Vector2 shownPosition;
    public bool shown;
    public bool playerInRange;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)){
            shown = !shown;
            player.movementLocked = shown;
        }

        if (shown){
            article.transform.position = Vector2.Lerp(article.transform.position, shownPosition, speed);
        }   else {
            article.transform.position = Vector2.Lerp(article.transform.position, hiddenPosition, speed);
            }
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" ){
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (other.tag == "Player"){
            playerInRange = false;
        }
    }
}
