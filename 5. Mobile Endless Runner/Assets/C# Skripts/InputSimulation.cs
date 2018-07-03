using UnityEngine;
using System.Collections;

public class SimulatedTouch {

	public Vector2 position;
	public Vector2 deltaPosition;
	public float deltaTime;
	public int fingerId;
	public TouchPhase phase;
	public int tapCount;
}

public class InputSimulation : MonoBehaviour {
	//variables

	//Die Anzahl an Touches. Verändert sich nicht während eines Frames.
	public static int touchCount;

	//Returns list of objects representing status of all touches during last frame. (Read Only)
	//Each entry represents a status of a finger touching the screen.
	public static SimulatedTouch[] touches;

	// Last measured linear acceleration of a device in three-dimensional space. (Read Only)
	public static Vector3 acceleration;

	//was there a touch in the last frame?
	private bool lastTouched = false;
	private Vector2 lastPosition;


	// Use this for initialization
	void Start () {
	
		lastTouched = false;
		lastPosition = Vector2.zero;
		acceleration = new Vector3(0.0f, 0.0f, -1.0f);
	}
	
	void FixedUpdate () {
		//read input and map it to the variables

		//touch
		//Touch is simulated with the mouse.
		//If Mouse is down then there is a touch. -> no Multi-Touch
		if (Input.GetMouseButton(0)) {

			touchCount = 1;
			SimulatedTouch currentTouch = new SimulatedTouch();

			currentTouch.position = Input.mousePosition;

			if (lastTouched) {
				currentTouch.deltaPosition = currentTouch.position - lastPosition;
			} else {
				currentTouch.deltaPosition = Vector2.zero;
			}
			currentTouch.deltaTime = Time.deltaTime;
			currentTouch.fingerId = 0;

			if (!lastTouched) {
				currentTouch.phase = TouchPhase.Began;
			} else if (currentTouch.deltaPosition.magnitude > 0) {
				currentTouch.phase = TouchPhase.Moved;
			} else {
				currentTouch.phase = TouchPhase.Stationary;
			}

			currentTouch.tapCount = 1;

			lastTouched = true;
			lastPosition = currentTouch.position;

			touches = new SimulatedTouch[1]{currentTouch};

		} else {

			touchCount = 0;
			touches = new SimulatedTouch[0];
			lastTouched = false;
		}

		//If MouseKey is going up then there is a ended touch.
		if (Input.GetMouseButtonUp(0))
		{
			touchCount = 1;
			SimulatedTouch currentTouch = new SimulatedTouch();
			currentTouch.position = Input.mousePosition;
			currentTouch.deltaPosition = currentTouch.position - lastPosition;
			currentTouch.phase = TouchPhase.Ended;
			currentTouch.fingerId = 0;
			touches = new SimulatedTouch[1] { currentTouch };
		}

		// acceleration
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			if (acceleration.x < 1.0f) {
				acceleration.x += 0.04f;
			}
		}

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			if (acceleration.x > -1.0f) {
				acceleration.x -= 0.04f;
			}
		}
						
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			if (acceleration.y < 1.0f) {
				acceleration.y += 0.04f;
			}
		}
		
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
			if (acceleration.y > -1.0f) {
				acceleration.y -= 0.04f;
			}
		}

	}

	//Returns object representing status of a specific touch.
	public static SimulatedTouch GetTouch(int index){
		return touches[index];
	}

}
