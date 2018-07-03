using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RunnerControl : MonoBehaviour {

	public bool running = false;
	public Vector3 direction = new Vector3(0, 0, 1);
	float speed = 5;
	float maxSpeed = 5;

	float jumpForce = 100;
	float maxJumpingHeight = 5; // was by default 8

	public float maxJumpTime = 20;
	float airTime;

	float rotation;

	new Camera camera;

	SimulatedTouch touch;
	double touchAngleToXaxis = 0;

	GameObject startButton;

	public bool isLeftLane = false;
	public bool isMidLane = true;
	public bool isRightLane = false;

	// Use this for initialization
	void Start () {

		Screen.SetResolution(440,700,false);

		camera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		startButton = GameObject.Find ("StartButton");

		//Camera.main.transform.eulerAngles = Vector3.Lerp(Camera.main.transform.eulerAngles, new Vector3(0, 90, 0), Time.deltaTime*camSpeed);
	}

	// Update is called once per frame
	void Update () {
		Vector3 camPos = camera.transform.position;

		Vector2 vecBallCam = new Vector2 (camPos.x - transform.position.x , camPos.z - transform.position.z);
		vecBallCam.Normalize ();
		vecBallCam = new Vector2 (vecBallCam.x * 15, vecBallCam.y * 15);

		camera.transform.position = new Vector3(transform.position.x + vecBallCam.x , 3, transform.position.z + vecBallCam.y);

		camera.transform.LookAt (this.transform);

		if (running) {


			transform.rotation = Quaternion.AngleAxis(rotation, Vector3.up);

			checkInput();

			processOrientation();

			freezePosition();

			if (GetComponent<Rigidbody>().velocity.magnitude < maxSpeed) {
				GetComponent<Rigidbody>().AddForce(direction * speed);
			}

			if (transform.position.y == maxJumpingHeight && airTime > 0) {
				airTime -= Time.deltaTime;
			}

			if (transform.position.y == maxJumpingHeight && airTime < 0) {
				transform.position = new Vector3(transform.position.x, 1, transform.position.z);
			}


			checkDeath();
		}
	}

	/**
	 * checks the orientation of the smartphone and moves the runner according to the actual orientation 
	 */
	void processOrientation() {

		if (GetComponent<BuildLane> ().elementBallIsOn.tag != "LeftCorner"
			&& GetComponent<BuildLane>().elementBallIsOn.tag != "RightCorner") {

			if (InputSimulation.acceleration.x < -0.2 && !isLeftLane) {
				Vector3 midLanePosition = GetComponent<BuildLane> ().midLanePosition;
				Debug.Log (midLanePosition);
				switch (GetComponent<BuildLane> ().rotationBallPOV) {
				case 90:
					transform.position = new Vector3 (transform.position.x, transform.position.y, midLanePosition.z + 1);
					break;
				case 180:
					transform.position = new Vector3 (midLanePosition.x + 1, transform.position.y, transform.position.z);
					break;
				case 270:
					transform.position = new Vector3 (transform.position.x, transform.position.y, midLanePosition.z - 1);
					break;
				default:
					transform.position = new Vector3 (midLanePosition.x - 1, transform.position.y, transform.position.z);
					break;
				}
				isLeftLane = true;
				isRightLane = false;
				isMidLane = false;
			}
			if (InputSimulation.acceleration.x > -0.2 && InputSimulation.acceleration.x < 0.2 && !isMidLane) {
				Vector3 midLanePosition = GetComponent<BuildLane> ().midLanePosition;
				Debug.Log (midLanePosition);

				switch (GetComponent<BuildLane> ().rotationBallPOV) {
				case 90:
					transform.position = new Vector3 (transform.position.x, transform.position.y, midLanePosition.z);
					break;
				case 180:
					transform.position = new Vector3 (midLanePosition.x, transform.position.y, transform.position.z);
					break;
				case 270:
					transform.position = new Vector3 (transform.position.x, transform.position.y, midLanePosition.z);
					break;
				default:
					transform.position = new Vector3 (midLanePosition.x, transform.position.y, transform.position.z);
					break;
				}
				isLeftLane = false;
				isMidLane = true;
				isRightLane = false;
			}
			if (InputSimulation.acceleration.x > 0.2 && !isRightLane) {
				Vector3 midLanePosition = GetComponent<BuildLane> ().midLanePosition;
				Debug.Log (midLanePosition);
				switch (GetComponent<BuildLane> ().rotationBallPOV) {
				case 90:
					transform.position = new Vector3 (transform.position.x, transform.position.y, midLanePosition.z - 1);
					break;
				case 180:
					transform.position = new Vector3 (midLanePosition.x - 1, transform.position.y, transform.position.z);
					break;
				case 270:
					transform.position = new Vector3 (transform.position.x, transform.position.y, midLanePosition.z + 1);
					break;
				default:
					transform.position = new Vector3 (midLanePosition.x + 1, transform.position.y, transform.position.z);
					break;
				}
				isLeftLane = false;
				isMidLane = false;
				isRightLane = true;
			}
		}
	}

	SimulatedTouch beginTouch;
	bool processNextTouch = false;

	/**
	 * verarbeitet die Touch-Eingaben
	 */
	void processTouches(){
		if (InputSimulation.touchCount != 0) {
			touch = InputSimulation.GetTouch (0);
			if (touch.phase == TouchPhase.Began) {
				beginTouch = touch;
			} else if (touch.phase == TouchPhase.Ended) {
				if (beginTouch == null) {
					beginTouch = touch;
				}
				Vector2 touchDirection = new Vector3 (touch.position.x - beginTouch.position.x, touch.position.y - beginTouch.position.y);

				Debug.Log ("Vec: " + touchDirection);

				float dist = touchDirection.magnitude;

				if (dist > 25) {
					//touchAngleToXaxis = RadianToDegree (AngleToXaxis (touchDirection));
					touchAngleToXaxis = RadianToDegree (Mathf.Atan2 (touchDirection.y, touchDirection.x));

					processNextTouch = true;
				}
			}
		} else {
			touch = null;
		}
	}

	// Converts Radians to Degrees
	private double RadianToDegree(double angle)
	{
		return angle * (180.0 / System.Math.PI);
	}


	double AngleToXaxis(Vector2 vec) {
		Vector2 vec2 = new Vector2 (1, 0);
		return System.Math.Acos ((vec2.x * vec2.x + vec.y * vec2.y) / (vec.magnitude * vec2.magnitude));
	}

	//sagt, ob ein entsprechender swipe vom Spieler ausgeführt wurde.
	bool swipeLeft(){
		if(processNextTouch && touchAngleToXaxis <= -135 || touchAngleToXaxis >= 135)  {
			if (!GetComponent<BuildLane> ().checkNextLaneElement) {
					processNextTouch = false;
					return true;
			} 
		} else if(Input.GetKeyUp(KeyCode.J)) {
			processNextTouch = false;
			return true;
		}

		return false;
	}


	bool swipeRight(){
		if (processNextTouch && touchAngleToXaxis <= 45 && touchAngleToXaxis >= -45) {
			if (!GetComponent<BuildLane> ().checkNextLaneElement) {
				processNextTouch = false;
				return true;
			}

		} else if(Input.GetKeyUp(KeyCode.K)) {
			processNextTouch = false;
			return true;
		}

		return false;
	}

	bool swipeUp(){
		if (processNextTouch) {
			if (touchAngleToXaxis < 135 && touchAngleToXaxis > 45) {
				processNextTouch = false;
				return true;
			}
		}
		return false;
	}

	//Input interpretieren
	void checkInput(){

		processTouches();

		if (swipeLeft()) {

			// Change variables in BuildLane to set the lanes (left/mid/right)
			// correctly to the rotated coordinate system
			GetComponent<BuildLane> ().rotationBallPOV = (270 + GetComponent<BuildLane>().rotationBallPOV) % 360;
			GetComponent<BuildLane> ().checkNextLaneElement = true;

			direction = Quaternion.AngleAxis(-90, Vector3.up) * direction;
			rotation -= 90;
			float upwardsMovement = GetComponent<Rigidbody>().velocity.y;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().velocity += new Vector3(0, upwardsMovement, 0);
//			camera.transform.Translate (Vector3.left);

		}

		if (swipeRight()){
			// Change variables in BuildLane to set the lanes (left/mid/right)
			// correctly to the rotated coordinate system
			GetComponent<BuildLane> ().rotationBallPOV = (90 + GetComponent<BuildLane>().rotationBallPOV) % 360;
			GetComponent<BuildLane> ().checkNextLaneElement = true;


			direction = Quaternion.AngleAxis(+90, Vector3.up) * direction;
			rotation += 90;
			float upwardsMovement = GetComponent<Rigidbody>().velocity.y;
			GetComponent<Rigidbody>().velocity = Vector3.zero;
			GetComponent<Rigidbody>().velocity += new Vector3(0, upwardsMovement, 0);
//			camera.transform.Translate (Vector3.right);

		}

		if (swipeUp()) {

			if (transform.position.y < 2) {
//				transform.position = new Vector3(transform.position.x, maxJumpingHeight, transform.position.z);
				GetComponent<Rigidbody>().AddForce(new Vector3(0, jumpForce, 0));
			}
		}

	}
	

	void die(){
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		direction = new Vector3(0, 0, 1);
		rotation = 0;
		transform.position = new Vector3(0, 1, 0);
		running = false;
		startButton.gameObject.SetActive (true);

		isMidLane = true;
		isLeftLane = false;
		isRightLane = false;

		camera.transform.position = new Vector3 (0, 3, -10);
		camera.transform.rotation = Quaternion.identity;

		GetComponent<BuildLane>().midLanePosition = new Vector3 (0, 0, 0);
		GetComponent<BuildLane> ().rotationBallPOV = 0;
		GetComponent<BuildLane> ().checkNextLaneElement = false;
	}


	void checkDeath() {
		if (transform.position.y < -1f) {
			die ();
		}
	}

	/**
	 * make sure the player runs only in one direction.
	 */
	void freezePosition(){
		if (direction == new Vector3(0, 0, 1) || direction == new Vector3(0, 0, -1)) {
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX;
		} else {
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ;
		}
		GetComponent<Rigidbody>().constraints |= RigidbodyConstraints.FreezeRotation;
	}


	// This function is passed to the startbutton
	public void StartGame() {
		running = true;
		startButton.gameObject.SetActive (false);
		Debug.Log ("Start Game");
	}
}
