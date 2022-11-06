using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform playerBody;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Transform eyeSightFollow;
    [SerializeField] private Transform fpsFollow;
    [SerializeField] private PlayerInput playerInput;

    public float xRotation = 0f;
    public bool mouseCanMove = true;

    private void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 10f);
        Cursor.lockState = CursorLockMode.Locked; //lock cursor in the center of the screne for POV perspective
    }

    private void Update()
    {
        if (mouseCanMove)
        {
            transform.position = eyeSightFollow.position; //set the camera position to the eyeline of the playerbody

            //using the Vector2 values determine the desired x and y movement
            float mouseX = playerInput.actions["Look"].ReadValue<Vector2>().x * mouseSensitivity * Time.deltaTime;
            float mouseY = playerInput.actions["Look"].ReadValue<Vector2>().y * mouseSensitivity * Time.deltaTime;

            //The rotation is just changed everyframe by the change in mouse y
            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -70f, 70f);

            //the rotation of the camera is set to the desired rotation
            transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            playerBody.Rotate(Vector3.up * mouseX);
        }
        else
        {
            transform.position = fpsFollow.position;
        }
    }
}
