using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public GameObject Player;
    public Transform target;
    public float movementSpeed;

    private Vector3 finalPosition;
    private Vector3 initialPos;
    private bool movingForward = false;

    private void Start()
    {
        initialPos = transform.position;
        finalPosition = target.position;
    }


    private void Update()
    {
        if(transform.position == initialPos)
        {
            movingForward = true;
        }
        if (transform.position == finalPosition)
        {
            movingForward = false;
        }
        MovePlatform();
    }
    private void MovePlatform()
    {
        if (movingForward)
        {
            float step = movementSpeed * Time.deltaTime; // step size = speed * frame time
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, step); // moves position a step closer to the target position
        }
        else
        {
            float step = movementSpeed * Time.deltaTime; // step size = speed * frame time
            transform.position = Vector3.MoveTowards(transform.position, initialPos, step); // moves position a step closer to the target position
        }
    }
}
