using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TextPoints : NetworkBehaviour {

	public Text pointsText;
	
	// Update is called once per frame
	void Update () {

		GameObject ball_go = GameObject.FindGameObjectWithTag ("ball");

		if (ball_go != null) {
			pointsText.text = ball_go.GetComponent<CountPoints> ().RpcGetPointsString ();
		}
	}

}
