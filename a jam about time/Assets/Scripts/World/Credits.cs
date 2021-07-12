using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Credits : MonoBehaviour
{

    public float endTime;
    public float wait;
    public Animator flash;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Ending());
    }

    IEnumerator Ending(){
        Destroy(GameObject.FindGameObjectWithTag("GameManager"));
        AsyncOperation load = SceneManager.LoadSceneAsync("Main Menu", LoadSceneMode.Single);
        load.allowSceneActivation = false;
        yield return new WaitForSeconds(endTime);
        flash.SetTrigger("Flash");
        yield return new WaitForSeconds(wait);
        load.allowSceneActivation = true;

        
    }
}
