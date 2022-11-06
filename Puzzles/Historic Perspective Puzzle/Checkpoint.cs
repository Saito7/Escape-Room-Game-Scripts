using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public Transform playerSpawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<GravityPlayerController>().setCheckPoint(playerSpawnPoint);    
    }

}
