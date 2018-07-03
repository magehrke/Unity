using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour {

	public float myTime = 60;

	public Text counterText;
	public bool finished;

	// Use this for initialization
	void Start () {
		counterText = GetComponent<Text>() as Text;
		finished = false;
	}

	// Update is called once per frame
	void Update () {
		if (myTime > 0) {
			myTime = myTime - Time.deltaTime;
			counterText.text = "Time left: " + myTime.ToString ();
		} else {
			myTime = 0;
			counterText.text = "Time left: " + myTime.ToString ();
			finished = true;
		}

		if (Input.GetKey (KeyCode.R)) {
			myTime = 60;
			finished = false;
		}
	}

	public bool isFinished() {
		return finished;
	}
}
