using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class CameraScript : NetworkBehaviour
{
    //Variables 
    [SerializeField] float cameraHeight = 20f;
    [SerializeField] float zAxisOffset = 3f;

    public GameObject player;
    PlayerScript playerScript;
    float smoothSpeed = 0.2f;
    Vector3 velocity = Vector3.zero;


    private void Start()
    {
        //if (!IsOwner) return;
        //if (!playerScript.IsAlive) return;

        //player = GameObject.FindGameObjectWithTag("Player");
        //playerScript = player.GetComponent<PlayerScript>();
    }
    private void FixedUpdate()
    {
        //if (!IsOwner) return;
        //if (!playerScript.IsAlive) return;

        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + cameraHeight, player.transform.position.z - zAxisOffset);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothSpeed);
    }
}
