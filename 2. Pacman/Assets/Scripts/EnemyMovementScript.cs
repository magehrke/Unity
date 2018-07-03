using UnityEngine;
using System.Collections.Generic;

/**
 * This script is responsible for making the ghost walk to the next waypoint.
 */
public class EnemyMovementScript : MonoBehaviour {

	public float movementSpeed;

	Vector3 destination;
	Vector3 currentWaypointPosition;

	Vector3 currentDirection;

	public float waypointDistanceThreshold = 0.05f;
	
	// Use this for initialization
	void Start () {

		currentWaypointPosition = transform.position;
	}

	public void setDestination(Vector3 newDestination){
		destination = newDestination;
	}


	void FixedUpdate () {

		calculateDirection();

		move();

		if (checkCloseToDestiny()) {
			getNextWayPoint();
		}
	}

	private void calculateDirection(){

		currentDirection = destination - currentWaypointPosition;
	}

	//poll the next waypoint
	private void getNextWayPoint(){

		GetComponent<EnemyBehaviourScript>().destinationReached();
	}

	//check if we are close to the next waypoint
	private bool checkCloseToDestiny(){

		//we are there -> set as current waypoint
		if ((transform.position - destination).magnitude <= waypointDistanceThreshold) {
			currentWaypointPosition = destination;
			transform.position = destination;
			return true;
		}
		
		return false;
	}
	
	//move the ghost
	private void move(){

		transform.position += currentDirection * movementSpeed * Time.fixedDeltaTime;
	}
}
