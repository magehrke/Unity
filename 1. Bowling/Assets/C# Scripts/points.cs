using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class points : MonoBehaviour {

	public float calcPoints;
	private Text pointText;

	// Use this for initialization
	void Start () {
		calcPoints = 0;
		pointText = GetComponent<Text>() as Text;
	}
	
	// Update is called once per frame
	void Update () {
		pointText.text = "Points: " + calcPoints.ToString();

		if (Input.GetKey (KeyCode.R)) {
			calcPoints = 0;
		}
	}

	public float getPoints() {
		return calcPoints;
	}

	public void setPoints(float poi) {
		calcPoints = poi;
	}
}
