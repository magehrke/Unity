using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerControll : NetworkBehaviour
{

	public float speed = 20;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
	    var y = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        //transform.Rotate(x, 0, 0);
        transform.Translate(0, y, 0);
    }
}
