using UnityEngine;

public interface IInteractable
{
    void OnStartHover();
    void Interact(GameObject other);
    void OnEndHover();
}
