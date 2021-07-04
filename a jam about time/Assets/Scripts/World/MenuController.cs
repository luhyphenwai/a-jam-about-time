using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameManager gm;
    public string playString;
    private void Awake() {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void PlayButton(){
        gm.StartCoroutine(gm.SwitchScenes(playString));
    }

}
