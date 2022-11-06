using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<GravityPlayerController>().playerOutOfBounds();    
    }
}
