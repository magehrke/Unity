using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class CountPoints : MonoBehaviour {

	public static int pointsLeft = 0;
	public static int pointsRight = 0;

	public float speed = 5;

	System.Random rnd = new System.Random();

	public GameObject ball;

	void OnTriggerEnter(Collider col) {

		if (col.gameObject.name == "LeftTrigger") {
			pointsRight += 1;
		} else if (col.gameObject.name == "RightTrigger") {
			pointsLeft += 1;
		}

		Debug.Log ("Left: " + pointsLeft + ", Right: " + pointsRight);

		this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
		this.transform.position = Vector3.zero;

		StartCoroutine(PutBallInMid());



		//CmdgetPoints();

	}

	IEnumerator PutBallInMid() {
		yield return new WaitForSeconds (2);
		GetComponent<Rigidbody>().velocity = new Vector3(GetRandomDir(),0,0) * speed;
	}

	int GetRandomDir() {
		int num = rnd.Next (0, 2);
		if (num == 0) {
			return -1;
		} else {
			return 1;
		}
	}

//	[Command]
//	public void CmdgetPoints()
//	{
//		points.text = "TEST";  //pointsLeft + " : " + pointsRight;
//	}
		
	public string RpcGetPointsString() {
		return pointsLeft + " : " + pointsRight;
	}

}
