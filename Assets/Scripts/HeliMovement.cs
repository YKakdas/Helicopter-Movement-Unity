using UnityEngine;

public class HeliMovement : MonoBehaviour {
	public Transform HUD;
	public float HUDOffset;

	private Vector3 originalUp;

	private float tiltSpeed = 2f;
	private float tiltMaxAngle = 20f;

	private float forwardAngle;
	private float rightAngle;

	private float rotationSpeed = 5f;
	private float translationSpeed = 11f;

	private float maxAltitude = 300f;
	private float throttle;

	private Animator animator;
	// Start is called before the first frame update
	void Start() {
		originalUp = transform.up;
		animator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update() {
		if(throttle < 5 && transform.position.y > 30) {
			animator.SetFloat("speed", 3);
		} else {
			animator.SetFloat("speed", throttle / 10f);
		}

		forwardAngle = transform.localEulerAngles.z.SignedAngle();
		rightAngle = transform.localEulerAngles.x.SignedAngle();

		Vector3 translationForward = Vector3.zero;
		Vector3 translationUp = Vector3.zero;

		Quaternion tiltForward = RestoreForwardTilt();
		Quaternion tiltRight = RestoreRightTilt();

		Quaternion rotation = Quaternion.identity;

		if(Input.GetKey("r")) {
			if(throttle < 100)
				throttle += 3 * Time.deltaTime;
			translationUp = RotorMovement(throttle, translationUp, 1);
		}

		if(Input.GetKey("f")) {
			if(throttle > 0)
				throttle -= 3 * Time.deltaTime;
			translationUp = RotorMovement(throttle, translationUp, -1);
		}

		if(Input.GetKey("w")) {
			tiltRight = GetRightTilt(-1);
			Vector3 projected = Vector3.ProjectOnPlane(transform.forward, originalUp).normalized;
			translationForward = translationSpeed * Time.deltaTime * -projected;
		}

		if(Input.GetKey("s")) {
			tiltRight = GetRightTilt(1);
			Vector3 projected = Vector3.ProjectOnPlane(transform.forward, originalUp).normalized;
			translationForward = translationSpeed * Time.deltaTime * projected;
		}

		if(Input.GetKey("d")) {
			tiltForward = GetForwardTilt(1);
			rotation = Quaternion.AngleAxis(rotationSpeed, originalUp);
		}

		if(Input.GetKey("a")) {
			tiltForward = GetForwardTilt(-1);
			rotation = Quaternion.AngleAxis(-rotationSpeed, originalUp);
		}

		transform.rotation = Quaternion.Slerp(transform.rotation, rotation * transform.rotation * tiltForward * tiltRight, 0.15f);
		transform.Translate(new Vector3(translationForward.x, translationUp.y, translationForward.z), Space.World);

	}

	void LateUpdate() {
		HUD.position = new Vector3(transform.position.x, transform.position.y + HUDOffset, transform.position.z);
	}

	private Vector3 RotorMovement(float throttle, Vector3 translationUp, float direction) {
		throttle = Mathf.Clamp(throttle, 0, 100);
		translationUp = direction * Vector3.up * 10f * Time.deltaTime;
		if(direction == 1) {
			if(throttle < 5 || transform.position.y > maxAltitude) translationUp = Vector3.zero;
		} else {
			if(transform.position.y < 21.5) {
				translationUp = Vector3.zero;
			}
		}
		return translationUp;
	}

	private Quaternion GetForwardTilt(int direction) {
		if(direction == 1) {
			return (forwardAngle > tiltMaxAngle) ? Quaternion.identity : Quaternion.AngleAxis(tiltSpeed, Vector3.forward);
		} else {
			return (forwardAngle < -tiltMaxAngle) ? Quaternion.identity : Quaternion.AngleAxis(-tiltSpeed, Vector3.forward);
		}
	}

	private Quaternion GetRightTilt(int direction) {
		if(direction == 1) {
			return (rightAngle > tiltMaxAngle) ? Quaternion.identity : Quaternion.AngleAxis(tiltSpeed, Vector3.right);
		} else {
			return (rightAngle < -tiltMaxAngle) ? Quaternion.identity : Quaternion.AngleAxis(-tiltSpeed, Vector3.right);
		}
	}

	private Quaternion RestoreForwardTilt() {
		if(forwardAngle > 0.3) {
			return Quaternion.AngleAxis(-tiltSpeed, Vector3.forward);
		} else if(forwardAngle < -0.3) {
			return Quaternion.AngleAxis(tiltSpeed, Vector3.forward);
		} else {
			return Quaternion.identity;
		}
	}

	private Quaternion RestoreRightTilt() {
		if(rightAngle > 0.3) {
			return Quaternion.AngleAxis(-tiltSpeed, Vector3.right);
		} else if(rightAngle < -0.3) {
			return Quaternion.AngleAxis(tiltSpeed, Vector3.right);
		} else {
			return Quaternion.identity;
		}
	}

}
