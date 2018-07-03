using UnityEngine;
using System.Collections;

//Erstellt ein Objekt, dass die Rotation des Smartphones darstellt.
public class SmartphoneOrientation : MonoBehaviour {

	GameObject visualize;

	// Use this for initialization
	void Start () {
		visualize = GameObject.CreatePrimitive(PrimitiveType.Cube);
		visualize.transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
		visualize.transform.localPosition = new Vector3(0, 7, 15);
		visualize.transform.localScale = new Vector3(8, 1, 4);
		visualize.GetComponent<Collider>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		visualize.transform.localRotation = (Quaternion.Euler ( new Vector3(InputSimulation.acceleration.y * 90.0f, 0, -InputSimulation.acceleration.x * 90.0f)));
	}
}
