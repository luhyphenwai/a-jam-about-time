using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameManager gm;
    public string playString;
    public float waitTime;
    public bool loadingScene;
    public AudioSource click;
    private void Awake() {
        gm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }
    public void PlayButton(){
        if (!loadingScene) StartCoroutine(Play());
    }

    IEnumerator Play(){
        click.Play();
        loadingScene = true;
        yield return new WaitForSeconds(waitTime);
        gm.StartCoroutine(gm.SwitchScenes(playString));
    }

}
