using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [Header("References")]
    public LevelObjects currentLevelObject;
    public GameObject player;
    public Animator flashAnim;
    public string pastScene;
    public string futureScene;
    private GameManager gm;
    public AudioSource timeSound;

    [Header("Scene Settings")]
    public bool loadedScene; // Used to check if the past version of the level has ever been loaded
    public bool loadingScene; // Used to check if a scene is currently being loaded
    public float sceneSwitchSpeed;
    public float sceneTransitionTime;
    public Scene lastScene;
    public string currentScene;
    
    [Header("Player Settings")]
    public Vector2 playerPosition;
    public Vector2 playerStartPosition;
    public Vector2 playerEndPosition;
    public Vector2 playerVelocity;
    
    [Header("Scene Object Settings")]
    public Vector2[] movableObjects; // Holds the positions of the movable items
    public bool[] doors; // Holds the state of the doors

    private void Start() {
        gm = transform.GetComponentInParent<GameManager>();
        lastScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if (lastScene != SceneManager.GetActiveScene()){
            OnNewScene();
        }
        lastScene = SceneManager.GetActiveScene();

        if (Input.GetKeyDown(KeyCode.F) && (currentScene == pastScene || currentScene == futureScene) && !loadingScene){
            StartCoroutine(SwitchLevel());
        }
    }

    void OnNewScene(){
        // Set References
        player = GameObject.FindGameObjectWithTag("Player");
        currentLevelObject = GameObject.FindGameObjectWithTag("LevelObject").GetComponent<LevelObjects>();

        if ((lastScene.name != futureScene && lastScene.name != pastScene) && (currentScene == futureScene || currentScene == pastScene)){
            gm.currentLevel = this;
            if (lastScene.buildIndex > SceneUtility.GetBuildIndexByScenePath(futureScene)){
                player.transform.position = playerEndPosition;
            }   else {
                player.transform.position = playerStartPosition;
            }
        }
        // Update info if first time entering past scene
        if (currentScene == pastScene && !loadedScene){
            UpdateInfo();
            loadedScene = true;
        }
    }

    void CheckPlayerDeath(){
        
    }

    // Used to switch scenes
    IEnumerator SwitchLevel(){
        loadingScene = true;

        gm.switchingScenes = true;

        player.GetComponent<PlayerController>().movementLocked = true; // lock player

        if (currentScene == pastScene){UpdateInfo();} // Update past scene info if switching from past scene

        // player.GetComponent<Animator>().SetTrigger("Dissapate");
        flashAnim.SetTrigger("Flash");

        timeSound.Play();
        yield return new WaitForSeconds(sceneSwitchSpeed); // Wait animation scene switch time

        playerPosition = player.transform.position; // Record player current position
        playerVelocity = player.GetComponent<Rigidbody2D>().velocity; // Record player velocity

        // Switch scenes and wait 
        // Load future or past scene depending on what scene is on currently
        if (currentScene == futureScene){
            AsyncOperation load = SceneManager.LoadSceneAsync(SceneUtility.GetBuildIndexByScenePath(pastScene), LoadSceneMode.Additive);
            while (!load.isDone){
                yield return null;
            }
            AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            while (!unload.isDone){
                yield return null;
            }
        }   else if (currentScene == pastScene){
            
            AsyncOperation load = SceneManager.LoadSceneAsync(SceneUtility.GetBuildIndexByScenePath(futureScene), LoadSceneMode.Additive);
            while (!load.isDone){
                yield return null;
            }
            AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            while (!unload.isDone){
                yield return null;
            }
        }

        // Wait a frame and reapply velocity
        yield return new WaitForEndOfFrame();

        OnNewScene();
        player.GetComponent<PlayerController>().movementLocked = true; // lock player
        player.transform.position = playerPosition; // Set player position
        player.GetComponent<Rigidbody2D>().velocity = playerVelocity; // Set player velocity
        // player.GetComponent<Animator>().SetTrigger("Appear"); // Set animation

        // Set camera position
        CameraController camera = GameObject.Find("Main Camera").GetComponent<CameraController>();
        camera.transform.position = player.transform.position + camera.offset;

        // Update scene object info
        if (currentScene == futureScene && loadedScene){UpdateScene();}
        else if (currentScene == pastScene && loadedScene){UpdateScene();}


        // Wait to fade back in
        yield return new WaitForSeconds(sceneTransitionTime);

        player.GetComponent<PlayerController>().movementLocked = false;
        loadingScene = false;

        gm.switchingScenes = false;
    }

    // Call before changing scenes
    void UpdateInfo(){
        movableObjects = new Vector2[currentLevelObject.movableObjects.Length];
        for (int i = 0; i < currentLevelObject.movableObjects.Length; i++){
            movableObjects[i] = currentLevelObject.movableObjects[i].transform.position;
        }
        for (int i = 0; i < currentLevelObject.doors.Length; i++){
            doors[i] = currentLevelObject.doors[i].GetComponent<DoorController>().open;
        }
    }

    // Call after schanging scenes
    void UpdateScene(){
        for (int i = 0; i < currentLevelObject.movableObjects.Length; i++){
            currentLevelObject.movableObjects[i].transform.position = movableObjects[i];
        }

        for (int i = 0; i < currentLevelObject.doors.Length; i++){
            currentLevelObject.doors[i].GetComponent<DoorController>().open = doors[i];
        }
    }
}
