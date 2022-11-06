using UnityEngine;

public class GlowManager : MonoBehaviour
{
    [SerializeField] private Material glow, nonGlow;

    public bool isGlowing = false;

    public void setGlowTrue()
    {
        gameObject.GetComponent<MeshRenderer>().material = glow;
        isGlowing = true;
    }

    public void setGlowFalse()
    {
        gameObject.GetComponent<MeshRenderer>().material = nonGlow;
        isGlowing = false;
    }
}
