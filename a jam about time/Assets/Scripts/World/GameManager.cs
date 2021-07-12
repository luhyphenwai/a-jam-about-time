using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public Animator anim;
    public LevelManager currentLevel;
    public float sceneTransitionTime;
    public bool switchingScenes;
    public AudioSource menu;
    public AudioSource forest;
    public AudioSource industry;
    public AudioSource travel;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        OnNewScene();
    }

    public void Reset(){
        // Reset variables
        currentLevel.loadedScene = false;

        StartCoroutine(SwitchScenes(SceneManager.GetActiveScene().name));
    }

    public IEnumerator SwitchScenes(string scene){
        AsyncOperation load = SceneManager.LoadSceneAsync(SceneUtility.GetBuildIndexByScenePath(scene), LoadSceneMode.Single);
        load.allowSceneActivation= false;
        switchingScenes = true;
        anim.SetTrigger("Flash");
        yield return new WaitForSeconds(sceneTransitionTime);
        load.allowSceneActivation = true;

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            player.GetComponent<PlayerController>().movementLocked = true;
            GameObject.FindGameObjectWithTag("MainCamera").transform.position = player.transform.position;
        }
        yield return new WaitForSeconds(sceneTransitionTime);
        
        if (player != null) {
            player.GetComponent<PlayerController>().movementLocked = false;
        }
        if (SceneManager.GetActiveScene().buildIndex == 0){
            menu.Play();
            forest.Stop();
            industry.Stop();
        } else if (SceneManager.GetActiveScene().buildIndex <= 8){
            menu.Stop();
            forest.Play();
            industry.Stop();
        }else if (SceneManager.GetActiveScene().buildIndex == 19){
            menu.Stop();
            forest.Stop();
            industry.Stop();
        }
        else {
            menu.Stop();
            forest.Stop();
            industry.Play();
        }
        switchingScenes = false;
        travel.Play();
        

    }

    private Scene lastScene;
    void OnNewScene(){
        if (lastScene != SceneManager.GetActiveScene()){
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.buildIndex > 0){
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }   
        
        lastScene = SceneManager.GetActiveScene();
    }
}
