using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pinScript : MonoBehaviour {

	Vector3 originalPos;

	GameObject ball;

	Rigidbody rb;

	bool hasCollided;

	// Use this for initialization
	void Start () {
		originalPos = transform.position;
		ball = GameObject.Find ("Bowling Ball");
		rb = GetComponent<Rigidbody> ();
		hasCollided = false;

	}

	void OnCollisionEnter(Collision collision){
		if (!hasCollided) {
			if (collision.gameObject.name == "Bowling Ball" || collision.gameObject.tag == "Pin") {
				GameObject aa = GameObject.FindWithTag ("Points");
				points bb = aa.GetComponent<points> ();
				float cc = bb.getPoints ();
				cc = cc + 10;
				bb.setPoints (cc);
				hasCollided = true;
			}
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Respawn
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.R)) {
			this.gameObject.transform.position = originalPos;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero; 
			transform.rotation = Quaternion.Euler(0,0,0);
			hasCollided = false;
		}
	}
}
