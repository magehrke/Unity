using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shootBall : MonoBehaviour {

	public Material material1;
	public Material material2;
	public Renderer rend;
	float distance = 2;

	public float speed = 0;
	public float maxSpeed = 200;	
	public bool increase = true;

	public bool followMouse = true;

	public Rigidbody rb;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();
		rend.material = material1;
		rb = GetComponent<Rigidbody> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (!GameObject.FindWithTag ("Finish").GetComponent<timer> ().isFinished ()) {
			if (Input.GetKeyUp ("space") && transform.position.z <= 3) {
				rb.AddForce (transform.forward * speed);
				followMouse = false;
			} else {

				// Position ball at mouse pointer
				if (followMouse) {
					Vector3 mousePosition = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, distance);
					Vector3 objPosition = Camera.main.ScreenToWorldPoint (mousePosition);
					transform.position = objPosition;
				}

				// Increase Speed & Change color of Ball
				if (Input.GetKey ("space") && transform.position.z < 3 ) {
					if (increase) {
						speed = speed + 2;
						if (speed >= maxSpeed) {
							increase = false;
						}
					} else {
						speed = speed - 2;
						if (speed <= 0) {
							increase = true;
						}
					}
					float lerp = speed / maxSpeed;
					rend.material.Lerp (material1, material2, lerp);
				}
			}
		}

	}


	// Respawn
	void FixedUpdate () {
		if (Input.GetKey(KeyCode.Escape) || Input.GetKey (KeyCode.R)) {
			transform.position = new Vector3 (0, 0, 2);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero; 
			transform.rotation = Quaternion.Euler(0,0,0);
			followMouse = true;
		}
	}
}
