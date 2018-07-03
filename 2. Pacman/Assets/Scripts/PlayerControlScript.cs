using UnityEngine;
using System.Collections.Generic;

public enum Direction {LEFT, UP, DOWN, RIGHT, NONE};

public class PlayerControlScript : MonoBehaviour {

	public WayPoint currentWaypoint;
	public Direction currentDirection;

	public float movementSpeed;

	public List<WayPoint> wayPoints;

	bool collisionAbove = false;
	bool collisionBelow = false;
	bool collisionLeft = false;
	bool collisionRight = false;

	public bool invincible;
	public float invincibleTime;
	float remainingInvincibleTime = 0.0f;
	
	public Material yellow;
	public Material invincibleMat;

	// Use this for initialization
	void Start () {
		currentDirection = Direction.NONE;

		//sammele alle Wegpunkte
		GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
		foreach (GameObject go in tiles) {
			wayPoints.Add(go.GetComponent<WayPoint>());
		}
		updateCurrentWayPoint();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		checkInput();

		move();

		updateCurrentWayPoint();

		updateVincibility();

		clearCollisionFlags();
	}

	//timer for invincibility
	private void updateVincibility(){
		if (invincible) {
			if (remainingInvincibleTime <= 0.0f) {
				invincible = false;
				GetComponent<Renderer>().material = yellow;
			}
			else remainingInvincibleTime -= 0.2f;
		}
	}

	//for invincibility. Berry is a Trigger
	public void OnTriggerEnter(Collider col){
		if (col.name.Equals("Berry")) {
			setInvincible();
			Destroy(col.gameObject);
		} 
	}
	
	public bool isInvincible(){
		return invincible;
	}
	
	public void setInvincible(){
		invincible = true;
		GetComponent<Renderer>().material = invincibleMat;
		remainingInvincibleTime = invincibleTime;
	}

	void clearCollisionFlags() {

		collisionLeft = false;
		collisionRight = false;
		collisionAbove =false;
		collisionBelow = false;
	}

	//setze den momentan nahesten Wegpunkt als aktuellen Wegpunkt
	private void updateCurrentWayPoint(){

		//initialisiere mit einem sehr hohen Wert, damit dieser auf jeden Fall überschrieben wird
		float minDistance = 1000.0f;
		

		foreach (WayPoint wp in wayPoints) {
			Vector3 vecDistance = wp.transform.position - transform.position;
			vecDistance.y = 0;
			float distance = vecDistance.magnitude;
			
			if (distance < minDistance) {
				minDistance = distance;
				currentWaypoint = wp;
			}
		}

	}

	//Abfrage der Steuerung
	private void checkInput(){

		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			currentDirection = Direction.LEFT;
		}
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			currentDirection = Direction.RIGHT;
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			currentDirection = Direction.UP;
		}
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
			currentDirection = Direction.DOWN;
		}
	}

	//bewege Pacman
	private void move(){
		switch (currentDirection) {
		case Direction.LEFT:
			if (!collisionLeft) {
				transform.position += new Vector3(-1.0f, 0.0f, 0.0f) * movementSpeed * Time.fixedDeltaTime;
			} else {
				currentDirection = Direction.NONE;
			}
			break;
		case Direction.RIGHT:
			if (!collisionRight) {
				transform.position += new Vector3(1.0f, 0.0f, 0.0f) * movementSpeed * Time.fixedDeltaTime;
			} else {
				currentDirection = Direction.NONE;
			}
			break;
		case Direction.UP:
			if (!collisionAbove) {
				transform.position += new Vector3(0.0f, 0.0f, 1.0f) * movementSpeed * Time.fixedDeltaTime;
			} else {
				currentDirection = Direction.NONE;
			}
			break;
		case Direction.DOWN:
			if (!collisionBelow) {
				transform.position += new Vector3(0.0f, 0.0f, -1.0f) * movementSpeed * Time.fixedDeltaTime;
			} else {
				currentDirection = Direction.NONE;
			}
			break;
		case Direction.NONE:

			break;
		}
	}

	void OnCollisionEnter(Collision col) {

		setCollisionFlags(col);
	}

	void OnCollisionStay(Collision col) {
		
		setCollisionFlags(col);
	}

	void resolveCollision(Collision col){

	}

	//lese CollisionFlags aus dem  Kontaktpunkt
	void setCollisionFlags(Collision col) {

		foreach (ContactPoint cp in col.contacts) {

			Vector3 localContactPoint = transform.InverseTransformPoint(cp.point);

			if (localContactPoint.x > 0.1f) {
				collisionRight = true;
			} else if (localContactPoint.x < -0.1f) {
				collisionLeft = true;
			}
			if (localContactPoint.z > 0.1f) {
				collisionAbove = true;
			} else if (localContactPoint.z < -0.1f) {
				collisionBelow = true;
			}

			//resolve collision. Very simple but unprecise method.
			transform.position -= localContactPoint * movementSpeed * Time.fixedDeltaTime;
		}
	}
}
