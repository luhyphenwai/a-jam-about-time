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
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Reset(){
        // Reset variables
        currentLevel.loadedScene = false;

        StartCoroutine(SwitchScenes(SceneManager.GetActiveScene().name));
    }

    public IEnumerator SwitchScenes(string scene){

        switchingScenes = true;
        anim.SetTrigger("Flash");
        yield return new WaitForSeconds(sceneTransitionTime);
        AsyncOperation load = SceneManager.LoadSceneAsync(SceneUtility.GetBuildIndexByScenePath(scene), LoadSceneMode.Single);
        while (!load.isDone){
            yield return null;
        }

        
        if (player != null) player.GetComponent<PlayerController>().movementLocked = true;

        yield return new WaitForSeconds(sceneTransitionTime);
        
        if (player != null) player.GetComponent<PlayerController>().movementLocked = false;
        
        switchingScenes = false;
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
