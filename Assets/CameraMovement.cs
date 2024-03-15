using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    //    // Store off camera position.
    //    var cameraPosition = transform.position;
    //    // Grab player position.x and match it.
    //    cameraPosition.x = player.position.x;
    //    // Set mod position back.
    //    transform.position = cameraPosition;

        transform.position = new Vector3(player.position.x, transform.position.y, transform.position.z);
    }
}
