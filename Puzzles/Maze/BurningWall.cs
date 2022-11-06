using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningWall : MonoBehaviour
{
    private float currentTime = 0;
    private bool startBurn;
    private int count = 0;

    [SerializeField] private GameObject flames;
    [SerializeField] private GameObject wall;

    private void OnTriggerEnter(Collider other)
    {
        if (count == 0)
        {
            flames.SetActive(true);
            startBurn = true;
        }
    }

    private void Update()
    {
        if (startBurn)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= 20)
            {
                currentTime = 0;
                flames.SetActive(false);
                wall.SetActive(false);
                startBurn = false;
                count++;
            }
        }
    }
}
