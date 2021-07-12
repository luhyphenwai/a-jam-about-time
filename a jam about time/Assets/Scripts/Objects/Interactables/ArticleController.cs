using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticleController : MonoBehaviour
{
    public GameObject article;
    public Camera cam;
    public SpriteRenderer sr;
    public Sprite idle;
    public Sprite selected;
    public PlayerController player;
    public float speed;

    public Vector2 hiddenPosition;
    public Vector2 shownPosition;
    public bool shown;
    public bool playerInRange;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E)){
            shown = !shown;
            player.movementLocked = shown;
        }

        if (cam == null) cam = Camera.main;
        Vector2 show = new Vector2(cam.transform.position.x, cam.transform.position.y+shownPosition.y);
        Vector2 hide = new Vector2(cam.transform.position.x, cam.transform.position.y+hiddenPosition.y);
        if (shown){
            article.transform.position = Vector2.Lerp(article.transform.position, show, speed);
        }   else {
            article.transform.position = Vector2.Lerp(article.transform.position, hide, speed);
        }

        if (playerInRange) sr.sprite = selected;
        else sr.sprite = idle;
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
