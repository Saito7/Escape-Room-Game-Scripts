using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFinish : MonoBehaviour
{
    public Transform Finish;
    private GravityPlayerController player;

    private void OnTriggerEnter(Collider other)
    {
        player = other.GetComponent<GravityPlayerController>();
        other.transform.position = Finish.position;
        other.transform.rotation = Finish.rotation;
        Physics.gravity = player.orientation.up.normalized * player.gravityValue;
    }
}
