using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;

public class Interactor : MonoBehaviour
{
    private IInteractable currentInteractable = null;

    [Header("References")]
    [SerializeField] private GameObject interactTextUI;
    [SerializeField] private Image crosshair;

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

    private void Start()
    {
        mainCam = Camera.main;
        rayDistance = 3f;
    }

    private void Update()
    {
        CheckForInteractable();            
    }

    public void CheckForInteraction(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed) { return; }
        //if the player pressed the interact button, check that they are looking at an interactable
        if(currentInteractable == null) { return; }
        //if they are, then call the interact function on the gameobject
        if (currentInteractable != null)
        {
            currentInteractable.Interact(transform.root.gameObject);
        }
    }

    void CheckForInteractable()
    {
        Ray ray = mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());

        bool hitSomething = Physics.Raycast(ray, out RaycastHit hitInfo, rayDistance); //writes out information into variable hitInfo

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, hitSomething ? Color.green : Color.red);

        if (hitSomething && (hitInfo.collider.gameObject.layer == 6)) // checks if player is looking at something which is examinable
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>(); //if it does then makes new interactable equal to it
            if (interactable != null)
            {                                           

                highlight(hitInfo); //create an outline around the object
                //if the user is looking at the same object as the previous frame, then stop
                if (interactable == currentInteractable)
                {
                    return;
                }
                else if (currentInteractable != null) //if it is a different object, remove previous interaction text and activate new text
                {
                    currentInteractable.OnEndHover();
                    currentInteractable = interactable;
                    currentInteractable.OnStartHover();
                }
                else if (currentInteractable == null)
                {
                    currentInteractable = interactable;
                    currentInteractable.OnStartHover();
                }
            }
            else
            {
                Debug.Log("Missing IInteractable Component");
            }
        }
        else
        {
            if(currentInteractable != null)  //if not looking at an interactable then remove any outline and text and clear variable
            {
                currentInteractable.OnEndHover();
                currentInteractable = null;
            }
            HideOutLine();            
        }  
    }

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
