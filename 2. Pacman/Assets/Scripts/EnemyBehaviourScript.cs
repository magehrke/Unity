using UnityEngine;
using System.Collections.Generic;

public enum Behaviour {FOLLOW, RANDOM, FLEE};
public enum Type {AGGRESSIVE, STUPID, CONFUSED, STEADY};

public class EnemyBehaviourScript : MonoBehaviour {

	public Type type;
	public Behaviour behaviour;

	List<WayPoint> path = new List<WayPoint>();
	public WayPoint currentWaypoint;

	void Start () {
		
		destinationReached();
	}
	
	// Update is called once per frame
	void Update () {

	}

	/**
	 * the ghost reached the ent of the current path and calculates a new one
	 */
	public void destinationReached(){
		if (path.Count > 0) {
			path.RemoveAt(0);
		}
		performTypeAction();
		if (path.Count == 0) {
			generatePath();
		}

		currentWaypoint = path[0];

		GetComponent<EnemyMovementScript>().setDestination(path[0].transform.position);
	}

	//change behaviour according to the character of the ghost
	private void performTypeAction() {
		switch (type) {
		case Type.AGGRESSIVE:
			//check if he has to flee
			if (GameObject.FindGameObjectWithTag("Pacman").GetComponent<PlayerControlScript>().isInvincible()) {
				behaviour = Behaviour.FLEE;
				path.Clear();
			} else {
				//always generates a new closest path to pacman
				behaviour = Behaviour.FOLLOW;
				path.Clear();
			}
			break;
		case Type.STUPID:
			
			//doesn't do anything
			
			break;
		case Type.CONFUSED:
			//always chooses a new random behaviour
			int newRandomBehaviour = Random.Range(0, 3);
			behaviour = (Behaviour)newRandomBehaviour;
			break;
		case Type.STEADY:
			//check if he has to flee
			if (GameObject.FindGameObjectWithTag("Pacman").GetComponent<PlayerControlScript>().isInvincible()) {
				behaviour = Behaviour.FLEE;
				path.Clear();
			} else {
				
				behaviour = Behaviour.FOLLOW;
			}
			break;
		}
	}
	
	//create the path according to the current behaviour
	private void generatePath(){
		
		switch (behaviour) {
		case Behaviour.FOLLOW:
			path = PathfindingScript.getPath(getPacmanPosition(), currentWaypoint);
			if (type.Equals(Type.AGGRESSIVE)) {
				path.RemoveAt(0);
			}
			break;
		case Behaviour.RANDOM:
			path = PathfindingScript.getRandomPath(currentWaypoint);
			break;
		case Behaviour.FLEE:
			path = PathfindingScript.getEscapePath(getPacmanPosition(), currentWaypoint);
			path.RemoveAt(0);
			break;
		}


	}
	
	//get Pacman's position
	private WayPoint getPacmanPosition(){
		return GameObject.FindGameObjectWithTag("Pacman").GetComponent<PlayerControlScript>().currentWaypoint;
	}

	//handle running into Pacman
	public void OnTriggerEnter(Collider col){

		if (col.tag.Equals("Pacman")) {
			if (col.gameObject.GetComponent<PlayerControlScript>().isInvincible()) {
				Destroy(gameObject);
			} else {
			Debug.Log("Got him!");
				foreach  (GameObject ghost in GameObject.FindGameObjectsWithTag("Ghost")) {
					if (ghost.GetComponent<EnemyBehaviourScript>() != null ){
						ghost.GetComponent<EnemyBehaviourScript>().type = Type.STUPID;
						ghost.GetComponent<EnemyBehaviourScript>().behaviour = Behaviour.RANDOM;
					}
				}
				Destroy(col.gameObject);
			}
		} 
	}
}