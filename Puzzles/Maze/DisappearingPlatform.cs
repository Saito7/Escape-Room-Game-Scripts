using UnityEngine;

public class DisappearingPlatform : MonoBehaviour
{
    [SerializeField] private GameObject flames;
    public float timeToTogglePlatform = 2;

    private float currentTime = 0;
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;
    private bool platformVisible = true;
    private bool collisionOccured;

    private void Start()
    {
        flames.SetActive(false);
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        collisionOccured = true;
        flames.SetActive(true);
    }

    private void Update()
    {
        if (collisionOccured)
        {
            //if play has stepped on platform
            //Add the time elapsing to a counter
            currentTime += Time.deltaTime;
            if (currentTime >= timeToTogglePlatform)
            {
                //when the correct amount of time has passed
                //enable or disable the platform
                currentTime = 0;
                if (platformVisible)
                {
                    flames.SetActive(false);
                    meshRenderer.enabled = false;
                    boxCollider.enabled = false;
                    platformVisible = false;
                }
                else
                {
                    meshRenderer.enabled = true;
                    boxCollider.enabled = true;
                    platformVisible = true;
                    collisionOccured = false;
                }
            }
        }
    }
}
