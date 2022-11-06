using UnityEngine.InputSystem;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System;

public class GearPuzzleManager : MonoBehaviour, IInteractable, ISaveable
{
    [Header("Events")]
    [SerializeField] protected StringEvent onHoveringOverInteractable = null;
    [SerializeField] protected VoidEvent onHoveringOverInteractableEnd = null;
    [SerializeField] private VoidEvent interactingWithObject = null;
    [SerializeField] private VoidEvent endInteractingWithobject = null;
    [SerializeField] protected VoidEvent deselectEmpty = null;
    [SerializeField] protected VoidEvent puzzleCompleted = null;

    [Header("References")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private Transform gearView = null;
    [SerializeField] private PlayerController playerController = null;
    [SerializeField] private MouseLook mouseLookScript = null;
    [SerializeField] private Image crosshair = null;
    [SerializeField] private Inventory inventory;
    [SerializeField] private LockedDrawerInteract lockedDrawerInteract = null;
    [SerializeField] private AudioSource unlockingSound;
    [SerializeField] private AudioSource gearboxLockedSound;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed;

    private string interactText = "Interact with ";
    private bool lerping = false;
    private bool interacting = false;
    private bool boxUnlocked = false;
    private bool puzzleComplete = false;
    private int correctGearCount = 0;
    private HotbarItem playerHotbarSelected = null;
    private Animation unlockingAnimation = null;
    private MeshCollider meshCollider = null;


    private void Start()
    {
        unlockingAnimation = GetComponentInParent<Animation>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void LateUpdate()
    {
        if (lerping)
        {
            MoveCameraAboveBoard();
        }
        if (Keyboard.current.escapeKey.wasPressedThisFrame && interacting && !lerping)
        {
            crosshair.enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            mouseLookScript.enabled = true;
            playerController.enabled = true;
            meshCollider.enabled = true;
            interacting = false;
            endInteractingWithobject.Raise();
        }
    }

    public void Interact(GameObject other)
    {
        if (!interacting && boxUnlocked) //If the playr is not interacting and the box is unlocked, start interacting
        {                                             
            mouseLookScript.enabled = false;
            playerController.enabled = false;
            lerping = true;
            interacting = true;
            meshCollider.enabled = false;
            interactingWithObject.Raise();
        }
        else if(!boxUnlocked)
        {
            //if box is locked, check if the player is holding the key
            if (playerHotbarSelected != null)
            {
                if(playerHotbarSelected.Name == "Old Key")   //If they are, unlock the box and remove the key from their inventory
                {
                    boxUnlocked = true;
                    inventory.RemoveItem(new ItemSlot(playerHotbarSelected as InventoryItem, 1));
                    if (!inventory.HasItem(playerHotbarSelected as InventoryItem))
                    {
                        deselectEmpty.Raise();
                    }
                    unlockingAnimation.Play();
                    unlockingSound.Play();
                }
                else
                {
                    gearboxLockedSound.Play();
                }
            }
            else
            {
                gearboxLockedSound.Play();
            }                                    
        }
    }

    public void OnEndHover()
    {
        onHoveringOverInteractableEnd.Raise();
    }

    public void OnStartHover()
    {
        if (boxUnlocked) //If unlocked then the player can hover over the gearbox
        {
            onHoveringOverInteractable.Raise(GetInteractText());
        }
        else if(playerHotbarSelected != null && !boxUnlocked) //if the box is not unlocked
        {
            if (playerHotbarSelected.Name == "Old Key") //If the player has selected the key
            {
                interactText = "unlock ";  //to show that the player can unlock the box
                onHoveringOverInteractable.Raise(GetInteractText()); //This updates the interaction text
                interactText = "Interact with";  //changes to Interact with as the box is unlocked so they can now hover over the gearbox
            }
        }
        else
        {
            onHoveringOverInteractable.Raise("Locked"); //Otherwise the box is locked.
        }
    }

    private string GetInteractText()
    {
        int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

        StringBuilder builder = new StringBuilder();

        builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + gameObject.name);

        return builder.ToString();
    }

    private void MoveCameraAboveBoard()
    {
        //Lerp position of camera
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, gearView.position, Time.deltaTime * transitionSpeed);

        Vector3 currentAngle = new Vector3(
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.x, gearView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.y, gearView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
           Mathf.LerpAngle(Camera.main.transform.rotation.eulerAngles.z, gearView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed));

        Camera.main.transform.eulerAngles = currentAngle;

        if (Vector3.Distance(Camera.main.transform.position, gearView.transform.position) < 0.001f)
        {
            lerping = false;
            Camera.main.transform.position = gearView.transform.position;
            Camera.main.transform.eulerAngles = gearView.transform.eulerAngles;
            crosshair.enabled = false;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void checkForCompletion()
    {
        correctGearCount++;
        if (correctGearCount == 5)
        {
            if (!puzzleComplete)
            {
                Debug.Log("Gear puzzle complete");
                puzzleComplete = true;
                lockedDrawerInteract.drawerUnlocked();
                //play sound effect
                puzzleCompleted.Raise();
            }
        }
    }

    public void removeCorrectGearCount()
    {
        correctGearCount--;
        Debug.Log("A correct Gear was removed");
    }

    public void getPlayerSelect(HotbarItem currentlySelected)
    {
        playerHotbarSelected = currentlySelected;
    }

    [Serializable] 
    private struct SaveData
    {
        public bool puzzleComplete;
        public bool boxUnlocked;
    }

    public object CaptureState()
    {
        return new SaveData
        {
            puzzleComplete = puzzleComplete,
            boxUnlocked = boxUnlocked
        };
    }

    public void RestoreState(object state)
    {
        var saveData = (SaveData)state;

        boxUnlocked = saveData.boxUnlocked;
        if (boxUnlocked)
        {
            unlockingAnimation = GetComponentInParent<Animation>();
            unlockingAnimation.Play();
        }
        puzzleComplete = saveData.puzzleComplete;
    }
}

