using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour {

	Vector3 originalPos;
	Rigidbody rb;


	// Use this for initialization
	void Start () {
		originalPos = transform.position;
		rb = GetComponent<Rigidbody> ();
	}
	
	void FixedUpdate () {
		if (Input.GetKey (KeyCode.Escape) || Input.GetKey (KeyCode.R)) {
			transform.position = originalPos;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero; 
			transform.rotation = Quaternion.Euler(0,0,0);
		}
	}

	void OnTriggerEnter(Collider other) {
		points pp = GameObject.FindWithTag ("Points").GetComponent<points> ();
		float oldPoints = pp.getPoints ();
		float newPoints = oldPoints + 150;
		pp.setPoints (newPoints);
	}
}
