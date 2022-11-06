using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class Examine : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject interactTextUI;
    [SerializeField] private Image crosshair;
    [SerializeField] private Interactor interactor;
    [SerializeField] private PlayerInput playerInput;

    #region CheckForInteractable Variables
    private Camera mainCam;

    [Header("Interaction Area Settings")]
    [SerializeField] private float rayDistance = 3f;
    [SerializeField] private LayerMask interactableLayer;
    #endregion

    #region Highlight variables
    private Outline_Controller currentcontroller;
    private Outline_Controller prevcontroller;
    #endregion

    #region Examine variables
    //if true allow rotation of object
    private bool examineMode = false;
    private int buttonPressedCounter = 0;
    //Holds orginal position and rotation so object can be replaced correctly
    private Vector3 originalPosition;
    private Vector3 originalRotation;
    //Currently clicked Object
    private GameObject clickedObject;
    //Camera variable is already initialized in interaction controller
    [Header("Examine System Settings")]
    [SerializeField] private float rotationSpeed = 5f; //how fast to rotate objects
    [SerializeField] private float distanceFromCamera = 1f; //how far away the object should be from the camera
    #endregion

    private void Start()
    {
        mainCam = Camera.main;
        rayDistance = 3f;
    }

    private void Update()
    {
        if (examineMode)
        {
            TurnObject();
        }
        if((buttonPressedCounter >= 2) || (Keyboard.current.escapeKey.wasPressedThisFrame))
        {
            ExitExamineMode();
        }
    }

    public void examineButtonPressed(InputAction.CallbackContext ctx)
    {

        if(!ctx.performed) { return; }

        ClickObject();
        buttonPressedCounter++;
    }

    #region Examine Functions
    private void ClickObject()
    {
        Ray ray = new Ray(mainCam.transform.position, mainCam.transform.forward); //creates a ray from camera, in the direction that camera is facing
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance, interactableLayer);

        if (hitSomething && hitInfo.collider.tag == "Examinable")
        {
            currentcontroller = hitInfo.collider.GetComponent<Outline_Controller>();
            if (!examineMode)
            {
                crosshair.enabled = false;

                //ClickedObject Will Be The Object Hit By The Raycast
                clickedObject = hitInfo.transform.gameObject;

                //Save The Original Postion And Rotation
                originalPosition = clickedObject.transform.position;
                originalRotation = clickedObject.transform.rotation.eulerAngles;

                //Now Move Object In Front Of Camera
                if (clickedObject.GetComponent<ItemPickup>() != null){ 
                    distanceFromCamera = clickedObject.GetComponent<ItemPickup>().desiredDistanceFromCamera;
                }
                else if(clickedObject.GetComponent<AnimatedBookInteract>()!= null)
                {
                    distanceFromCamera = clickedObject.GetComponent<AnimatedBookInteract>().desiredDistanceFromCamera;
                }
                else if (clickedObject.GetComponent<ParchmentClueExamine>() != null)
                {
                    distanceFromCamera = clickedObject.GetComponent<ParchmentClueExamine>().desiredDistanceFromCamera;
                }
                clickedObject.transform.position = mainCam.transform.position + (mainCam.transform.forward * distanceFromCamera);
                mainCam.focalLength = 4;

                //Pause The Game
                Time.timeScale = 0;

                //Turn Examine Mode To True
                interactor.enabled = false;
                interactTextUI.SetActive(false);
                currentcontroller.HideOutLine();
                examineMode = true;
                Cursor.lockState = CursorLockMode.Confined;
                Debug.Log("Inspecting " + clickedObject.name);
            }
        }
    }

    private void TurnObject()
    {
        if (Mouse.current.leftButton.isPressed && examineMode)
        {             
            float xAxis = playerInput.actions["Look"].ReadValue<Vector2>().x * rotationSpeed;
            float yAxis = playerInput.actions["Look"].ReadValue<Vector2>().y * rotationSpeed;

            clickedObject.transform.Rotate(Camera.main.transform.up, -xAxis, Space.World);
            clickedObject.transform.Rotate(Camera.main.transform.right, yAxis, Space.World);
        }
    }

    private void ExitExamineMode()
    {
        if (examineMode)
        {
            crosshair.enabled = true;

            //Reset Object To Original Position
            clickedObject.transform.position = originalPosition;
            clickedObject.transform.eulerAngles = originalRotation;

            //Unpause Game
            Time.timeScale = 1;

            //Return To Normal State
            interactor.enabled = true;
            interactTextUI.SetActive(true);
            currentcontroller.ShowOutline();
            examineMode = false;
            buttonPressedCounter = 0;
            Cursor.lockState = CursorLockMode.Locked;
            Debug.Log("Stopped Inspecting " + clickedObject.name);
            buttonPressedCounter = 0;
        }
    }
    #endregion

    #region Highlighting functions
    private void highlight(RaycastHit hitInfo)
    {
        currentcontroller = hitInfo.collider.GetComponent<Outline_Controller>();

        if (prevcontroller != currentcontroller)
        {
            HideOutLine();
            ShowOutline();
        }

        prevcontroller = currentcontroller;
    }


    private void ShowOutline()
    {
        if (currentcontroller != null)
        {
            currentcontroller.ShowOutline();
        }
    }

    private void HideOutLine()
    {
        if (prevcontroller != null)
        {
            prevcontroller.HideOutLine();
            prevcontroller = null;
        }
    }
    #endregion
}
