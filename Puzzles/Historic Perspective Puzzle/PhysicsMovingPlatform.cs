using UnityEngine;

public class PhysicsMovingPlatform : MonoBehaviour
{
	private Vector3 startPoint;
	public Transform endPoint;
	public float travelTime;
	private Rigidbody rb;
	private Vector3 currentPos;

	private void Start()
	{
		startPoint = transform.position;
		rb = GetComponent<Rigidbody>();
	}
	void FixedUpdate()
	{
		currentPos = Vector3.Lerp(startPoint, endPoint.position,Mathf.Cos(Time.time / travelTime * Mathf.PI * 2) * -.5f + .5f);
		rb.MovePosition(currentPos);
	}

}