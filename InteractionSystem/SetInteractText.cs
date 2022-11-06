using TMPro;
using UnityEngine;

public class SetInteractText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI interactionText = null;
    [SerializeField] private GameObject interactionTextHolder = null;

    public void DisplayInfo(InventoryItem infoItem)
    {
        //Set info text to be displayed
        interactionText.text = infoItem.GetInteractText();

        //Activate UI canvas object
        interactionTextHolder.SetActive(true);
    }

    public void DisplayNonItemInfo(string objectInteractText)
    {
        //Set info text to be displayed
        interactionText.text = objectInteractText;

        //Activate UI canvas object
        interactionTextHolder.SetActive(true);
    }

    public void HideInfo() => interactionTextHolder.SetActive(false);
}
