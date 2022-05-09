using UnityEngine;

public class HelicopterCameraFollower : MonoBehaviour {
	public Transform target;
	public Vector3 offset;
	private Vector3 velocity = Vector3.zero;
	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	void LateUpdate() {
		Vector3 posWithOffset = target.position + target.transform.right * offset.x + target.transform.up * offset.y + target.transform.forward * offset.z;
		Vector3 desiredPosition = Vector3.SmoothDamp(transform.position, posWithOffset, ref velocity, 0.125f);
		transform.position = desiredPosition;
		transform.LookAt(target.transform.position);
	}
}
