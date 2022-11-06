using UnityEngine;

public class Outline_Controller : MonoBehaviour
{
    private Outline outliner;
    private MeshRenderer meshRenderer;
    public float maxOutlineWidth;
    public Color OutlineColor;

    // Start is called before the first frame update
    void Start()
    {
        outliner = GetComponent<Outline>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void ShowOutline()
    {
        if (outliner != null)
        {
            outliner.OutlineWidth = maxOutlineWidth;
            outliner.OutlineColor = OutlineColor;
        }
        else
        {
            if (meshRenderer != null)
            {
                meshRenderer.material.SetFloat("_OutlineWidth", maxOutlineWidth);     //gets data from the outline shader made earlier
                meshRenderer.material.SetColor("_OutlineColor", OutlineColor);  //stuff in speech marks is name of those variables from shader 
            }
        }        
    }

    public void HideOutLine()
    {
        if (outliner != null)
        {
            outliner.OutlineWidth = 0f;
        }
        else
        {
            meshRenderer.material.SetFloat("_OutlineWidth", 0f);
        }
    }
}
