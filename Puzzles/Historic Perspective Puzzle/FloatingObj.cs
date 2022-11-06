using UnityEngine;

public class FloatingObj : MonoBehaviour
{
    private Vector3 upPos;
    private Vector3 downPos;
    public float bobbingSpeed = 1.5f;
    public float rotationSpeed = 75f;
    public float maxBobbingDistance = 0.5f;
    public float travelTime = 4f;

    private void Start()
    {
        upPos = transform.position + transform.up * maxBobbingDistance;
        downPos = transform.position + transform.up * -maxBobbingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        transform.position = Vector3.Lerp(upPos, downPos, Mathf.Cos(Time.time / travelTime * Mathf.PI * 2) * -.5f + .5f);
    }
}
