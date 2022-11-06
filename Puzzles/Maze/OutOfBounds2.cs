using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds2 : MonoBehaviour
{
    [SerializeField] private Transform start;

    private void OnTriggerEnter(Collider other)
    {
        other.transform.position = start.position;
        other.transform.rotation = start.rotation;
    }
}
