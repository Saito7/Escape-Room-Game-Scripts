
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;



public class Block : MonoBehaviour, IPointerClickHandler
{
    public event System.Action<Block> OnBlockPressed;
    public event System.Action OnFinishedMoving;

    public Vector2Int coord;
    Vector2Int startingCoord;

    public void Init(Vector2Int startingCoord, Texture2D image)
    {
        this.startingCoord = startingCoord;
        coord = startingCoord;

        GetComponent<MeshRenderer>().material = Resources.Load<Material>("Block");
        GetComponent<MeshRenderer>().material.mainTexture = image;
    }

    public void MoveToPosition(Vector2 target, float duration)
    {
        StartCoroutine(AnimateMove(target, duration));  
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        OnBlockPressed.Invoke(this);
    }

    IEnumerator AnimateMove(Vector2 target, float duration)
    {
        Vector2 initalPos = transform.position;
        float percent = 0;
        
        while(percent < 1)
        {
            percent += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(initalPos, target, percent);
            yield return null;
        }

        if(OnFinishedMoving != null)
        {
            OnFinishedMoving();
        }
    }

    public bool IsAtStartingCoord()
    {
        return coord == startingCoord;
    }

}
