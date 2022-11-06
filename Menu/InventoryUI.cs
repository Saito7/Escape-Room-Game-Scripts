using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image crosshair;
    [SerializeField] private GameObject objectToToggle = null;
    [SerializeField] private GameObject interactor = null;
    [SerializeField] private GameObject resumeMenu = null;

    private bool interacting = false;

    public void ToggleInventory()
    {
        if (!objectToToggle.activeSelf && !resumeMenu.activeSelf) //if the pause menu is not active and inventory is not open
        {
            //open inventory and remove ability to interact and freeze time
            objectToToggle.SetActive(true);
            interactor.SetActive(false);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            crosshair.enabled = false;
        }
        else
        {
            objectToToggle.SetActive(false);
            interactor.SetActive(true);
            Time.timeScale = 1f;
            if (!interacting)
            {
                crosshair.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }            
    }
    public void SetInteracting(bool isPlayerInteracting)
    {
        interacting = isPlayerInteracting;
    }
}
