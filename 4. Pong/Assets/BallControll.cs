using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class BallControll : NetworkBehaviour {

    public GameObject ball;
	public float speed = 5;
	System.Random rnd = new System.Random();


	int GetRandomDir() {
		int num = rnd.Next (0, 2);
		if (num == 0) {
			return -1;
		} else {
			return 1;
		}
	}
	
    [Command]
    public void CmdcreateBall(){
        GameObject start = Instantiate(ball, new Vector3(0f, 0f, 0f), Quaternion.identity);
        NetworkServer.Spawn(start);
    }

	[Command]
	public void CmddestroyBall()
	{
		Destroy(this);
	}

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().velocity = new Vector3(GetRandomDir(),0,0)* speed;
	}
	
	float hitFactor(Vector3 ballPos, Vector3 racketPos, float racketHeight) {
		return (ballPos.y - racketPos.y) / racketHeight;
	}
	
	void OnCollisionEnter(Collision col) {
		if (col.gameObject.transform.position.x == -9 && col.gameObject.tag == "Player") {
			
			float y = hitFactor(transform.position,
				col.transform.position,
				col.collider.bounds.size.y);

			Vector3 dir = new Vector3(1, y, 0).normalized;
			GetComponent<Rigidbody>().velocity = dir * speed;
		}

		if (col.gameObject.transform.position.x == 9 && col.gameObject.tag  == "Player") {
			float y = hitFactor(transform.position,
				col.transform.position,
				col.collider.bounds.size.y);

			Vector3 dir = new Vector3(-1, y, 0).normalized;
			GetComponent<Rigidbody>().velocity = dir * speed;
		}
	}
}
