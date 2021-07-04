using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    [Header("References")]
    public GameObject target;
    

    [Header("Camera Settings")]
    public float cameraSize;
    public float cameraSpeed;
    public Vector3 offset;
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Set the target to the player
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Like update, but is always run at the same time with the physics, should prevent camera stuttering
    void FixedUpdate()
    {
        followTarget();
    }

    void followTarget(){
        // Set the intended position
        Vector3 targetPosition = target.transform.position + offset;

        // Lerp the camera towards the position
        transform.position = Vector3.Lerp(transform.position, targetPosition, cameraSpeed);
    }
}

