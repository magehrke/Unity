using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpScript : MonoBehaviour {

	GameObject ball;

	// Use this for initialization
	void Start () {
		ball = GameObject.FindGameObjectWithTag ("Ball");
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.position.x < ball.GetComponent<BallControl> ().GetBallPosition ().x - 20) {
			ball.GetComponent<BallControl> ().DecreasePowerUpCount ();
			Destroy (this.gameObject);
		}
	}
}
