using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Placeable Item", menuName = "Items/Placeable Item")]

public class PlaceableItem : InventoryItem
{
    [SerializeField] private string interactText = "Store";
    [SerializeField] private InputActionReference interactAction;
    
    public override string GetInfoDisplayText()
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("<b>Info:</b> ").Append(InfoText).AppendLine();
        builder.Append("<b>Weight:</b> ").Append(Weight).Append("g").AppendLine();

        return builder.ToString();
    }

    public override string GetInteractText()
    {
        int bindingIndex = interactAction.action.GetBindingIndexForControl(interactAction.action.controls[0]);

        StringBuilder builder = new StringBuilder();

        builder.Append("Press ").Append(InputControlPath.ToHumanReadableString(interactAction.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice)).Append(" to ").Append(interactText + " " + Name);

        return builder.ToString();
    }
}
