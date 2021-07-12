using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScript : MonoBehaviour
{
    public Animator anim;
    public GameManager gm;
    public bool playerInRange;
    public bool ending;
    public float endTime;
    public AudioSource endSound;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetBool("Selected", playerInRange);
        if (playerInRange && Input.GetKeyDown(KeyCode.E) &&!ending){
            anim.SetTrigger("End");
            StartCoroutine(End());
        }
    }
    IEnumerator End(){
        ending = true;
        yield return new WaitForSeconds(endTime);
        SceneManager.LoadScene("Credits", LoadSceneMode.Single);
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
