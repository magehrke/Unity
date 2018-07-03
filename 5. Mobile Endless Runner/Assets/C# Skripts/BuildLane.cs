using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildLane : MonoBehaviour {

	// Contains all our lane game objects
	public List<GameObject> lane = new List<GameObject>();

	// Lane Elements
	GameObject leftLane;
	GameObject midLane;
	GameObject rightLane;
	GameObject leftCorner;
	GameObject rightCorner;
	GameObject obstacle;

	public GameObject elementBallIsOn;
	public int rotationBallPOV = 0;
	public Vector3 midLanePosition;
	public bool checkNextLaneElement = false;

	bool wasLastCornerRight = false;

	// Position of the lane, where the next mid lane element
	// would have been placed. So it refers always to the mid
	// of the lane, even if only the left part has been build
	Vector3 pos;

	// At start the lane builds into z-direction
	// This can change and will be displayed by 90,180,270.
	int rotation = 0;
	Quaternion quaternion = Quaternion.identity;

	static System.Random rnd = new System.Random();

	// Obstactle:
	int obstacleMin = 60;
	int obstacleMax = 100;
	// Corner:
	int cornerMin = 30;
	int cornerMax = 40;
	// Single lane: 
	int singleLaneMin = 40;
	int singleLaneMax = 60;
	// Single lane length:
	int singleLaneLengthMin = 10;
	int singleLaneLengthMax = 20;

	// When do we change directions or special events occur
	int nextObstacleIn;
	int nextCornerIn;
	int nextSingleLaneIn;
	int nextSingleLaneLength;

	// Use this for initialization
	void Start () {
		// Load Prefabs
		leftLane = LoadPrefab("Lane Left");
		midLane = LoadPrefab ("Lane Mid");
		rightLane = LoadPrefab ("Lane Right");
		leftCorner = LoadPrefab ("Left Corner");
		rightCorner = LoadPrefab ("Right Corner");
		obstacle = LoadPrefab ("Obstacle");

		// Add already existing elements to lane
		lane.Add (GameObject.Find ("StartLane"));

		midLanePosition = new Vector3 (0, 0, 0);

		// Initialize events
		nextObstacleIn = rnd.Next(obstacleMin, obstacleMax+1);
		nextCornerIn = rnd.Next(cornerMin, cornerMax+1);
		nextSingleLaneIn = rnd.Next(singleLaneMin, singleLaneMax);
		nextSingleLaneLength = rnd.Next(singleLaneLengthMin, singleLaneLengthMax);

		// Start the game with a long part of lanewidth=3
		SetPosition ();
		//AddThreeLanes (20);
	}
	
	// Update is called once per frame
	void Update () {

		

		while(Vector3.Distance (this.transform.position, pos) < 100) {
			AddLaneElement ();
		}


		for (int i = 0; i < lane.Count; i++) {
			GameObject el = lane [i];
			Vector2 ballPos = new Vector2 (transform.position.x, transform.position.z);
			Vector2 elPos = new Vector2 (el.transform.position.x, el.transform.position.z);
			Vector3 elPos3D = el.transform.position;
			float dist = Vector2.Distance (ballPos, elPos);
			if (dist <= 1) {
				elementBallIsOn = el;

				if (checkNextLaneElement && i-3 >= 0) {
					if(el.tag != "LeftCorner" && el.tag != "RightCorner") {
						if (el.tag == "LeftLane" && (lane[i-1].tag == "RightCorner" 
							|| lane[i-1].tag == "LeftCorner")) {
							midLanePosition = elPos3D;
							GetComponent<RunnerControl> ().isLeftLane = false;
							GetComponent<RunnerControl> ().isMidLane = false;
							GetComponent<RunnerControl> ().isRightLane = false;

							switch (rotationBallPOV) {

							case 90:
								midLanePosition.z = elPos3D.z - 1;
								break;
							case 180:
								midLanePosition.x = elPos3D.x - 1;
								break;
							case 270:
								midLanePosition.z = elPos3D.z + 1;
								break;
							default:
								midLanePosition.x = elPos3D.x + 1;
								break;
							}
							checkNextLaneElement = false;


						} else if (el.tag == "MidLane" && (lane[i-2].tag == "RightCorner" 
							|| lane[i-2].tag == "LeftCorner")) {
							midLanePosition = elPos3D;
							GetComponent<RunnerControl> ().isLeftLane = false;
							GetComponent<RunnerControl> ().isMidLane = false;
							GetComponent<RunnerControl> ().isRightLane = false;	
							checkNextLaneElement = false;

						} else if (el.tag == "RightLane" && (lane[i-3].tag == "RightCorner" 
							|| lane[i-3].tag == "LeftCorner")) {
							midLanePosition = elPos3D;

							switch (rotationBallPOV) {

							case 90:
								midLanePosition.z = elPos3D.z + 1;
								break;
							case 180:
								midLanePosition.x = elPos3D.x + 1;
								break;
							case 270:
								midLanePosition.z = elPos3D.z - 1;
								break;
							default:
								midLanePosition.x = elPos3D.x - 1;
								break;
							}

							GetComponent<RunnerControl> ().isLeftLane = false;
							GetComponent<RunnerControl> ().isMidLane = false;
							GetComponent<RunnerControl> ().isRightLane = false;	
							checkNextLaneElement = false;
						}

					}
					
				}
			}
			

		}
		
	}

	void AddLaneElement() {
		SetPosition ();
		if (nextObstacleIn < 0) {
			AddThreeLanes ();
			AddObstacle ();
			nextObstacleIn = rnd.Next (obstacleMin, obstacleMax + 1);
		} else if (nextSingleLaneIn < 0) {
			AddSingleLane ();
			nextSingleLaneLength = rnd.Next (singleLaneLengthMin, singleLaneLengthMax + 1);
			nextSingleLaneIn = rnd.Next (singleLaneMin, singleLaneMax + 1);
		} else if (nextCornerIn < 0) {
			if (wasLastCornerRight) {
				AddLeftCorner ();
				wasLastCornerRight = false;
			} else {
				AddRightCorner ();
				wasLastCornerRight = true;
			}
			nextCornerIn = rnd.Next (cornerMin, cornerMax + 1);
		} else {
			AddThreeLanes ();
		}


		// Decrement event counters
		nextObstacleIn -= 1;
		nextCornerIn -= 1;
		nextSingleLaneIn -= 1;
	}

	void AddSingleLane() {
		int left_mid_right = rnd.Next (0, 3);
		if(left_mid_right == 0) {
			AddLeftLane (nextSingleLaneLength);
		} else if(left_mid_right == 1) {
			AddMidLane (nextSingleLaneLength);
		} else if(left_mid_right == 2) {
			AddRightLane (nextSingleLaneLength);
		}
	}

	void AddObstacle() {
		Vector3 _pos = pos;
		_pos.y += 1;
		lane.Add(Instantiate(obstacle, _pos, quaternion));
	}

	void SetPosition() {
		GameObject go = lane [lane.Count - 1];
		pos = go.transform.position;

		if (go.tag == "Obstacle") {
			pos.y -= 1;
		}

		switch (rotation) {

		case 90:
			pos.x += 1; 
			if (go.tag == "LeftLane") {
				pos.z -= 1;
			} else if (go.tag == "RightLane") {
				pos.z += 1;
			} else if (go.tag == "LeftCorner") {
				pos.x += 1;
				pos.z -= 1;
			} else if (go.tag == "RightCorner") {
				pos.x += 1;
				pos.z += 1;
			}
			break;

		case 180:
			pos.z -= 1; 
			if (go.tag == "LeftLane") {
				pos.x -= 1;
			} else if (go.tag == "RightLane") {
				pos.x += 1;
			} else if (go.tag == "LeftCorner") {
				pos.x -= 1;
				pos.z -= 1;
			} else if (go.tag == "RightCorner") {
				pos.x += 1;
				pos.z -= 1;
			}
			break;

		case 270:
			pos.x -= 1; 
			if (go.tag == "LeftLane") {
				pos.z += 1;
			} else if (go.tag == "RightLane") {
				pos.z -= 1;
			} else if (go.tag == "LeftCorner") {
				pos.x -= 1;
				pos.z += 1;
			} else if (go.tag == "RightCorner") {
				pos.x -= 1;
				pos.z -= 1;
			}
			break;

		default:
			pos.z += 1; 
			if (go.tag == "LeftLane") {
				pos.x += 1;
			} else if (go.tag == "RightLane") {
				pos.x -= 1;
			} else if (go.tag == "LeftCorner") {
				pos.x += 1;
				pos.z += 1;
			} else if (go.tag == "RightCorner") {
				pos.x -= 1;
				pos.z += 1;
			}
			break;
		}




	}

	void AddThreeLanes(int blockCount=1) {
		for (int i = 0; i < blockCount; i++) {
			if (i != 0)
				SetPosition();
			AddLeftLane ();
			AddMidLane ();
			AddRightLane ();
		}
	}

	void AddLeftLane(int blockCount=1) {
		for (int i = 0; i < blockCount; i++) {
			if (i != 0) {
				SetPosition ();
			}
			Vector3 _pos = pos;
			switch (rotation) {
			case 90:
				_pos.z += 1;
				break;
			case 180:
				_pos.x += 1;
				break;
			case 270:
				_pos.z -= 1;
				break;
			default:
				_pos.x -= 1;
				break;
			}
			lane.Add (Instantiate (leftLane, _pos, quaternion));
		}
	}

	void AddMidLane(int blockCount=1) {
		for (int i = 0; i < blockCount; i++) {
			if (i != 0) {
				SetPosition ();
			}
			lane.Add (Instantiate (midLane, pos, quaternion));
		}
	}

	void AddRightLane(int blockCount=1) {
		for (int i = 0; i < blockCount; i++) {
			if (i != 0) {
				SetPosition ();
			}
			Vector3 _pos = pos;
			switch (rotation) {
			case 90:
				_pos.z -= 1;
				break;
			case 180:
				_pos.x -= 1;
				break;
			case 270:
				_pos.z += 1;
				break;
			default:
				_pos.x += 1;
				break;
			}
			lane.Add (Instantiate (rightLane, _pos, quaternion));
		}
	}

	void AddLeftCorner() {
		lane.Add (Instantiate(leftCorner, pos, quaternion));
		rotation = (270 + rotation) % 360;
		quaternion = Quaternion.Euler (new Vector3 (0, rotation, 0));
	}

	void AddRightCorner() {
		lane.Add (Instantiate(rightCorner, pos, quaternion));
		rotation = (90 + rotation) % 360;
		quaternion = Quaternion.Euler (new Vector3 (0, rotation, 0));
	}





	// Load Prefabs which are inside Assets/Resources/Prefabs
	GameObject LoadPrefab(string name) {
		if ((GameObject) Resources.Load ("Prefabs/" + name, typeof(GameObject)) == null) {
			Debug.Log ("Prefab '" + name + "' couldn't be found");
		}
		return (GameObject) Resources.Load ("Prefabs/" + name, typeof(GameObject));
	}

}
