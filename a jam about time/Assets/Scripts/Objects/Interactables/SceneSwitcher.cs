using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcher : MonoBehaviour
{

    public GameManager gm;
    public string scene;
    private void Start() {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player" && !gm.switchingScenes){
            gm.StartCoroutine(gm.SwitchScenes(scene));
        }
        
    }


}
