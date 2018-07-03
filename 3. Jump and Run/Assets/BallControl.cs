using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BallControl : MonoBehaviour {


	/*
	 * Length of the Levelelements: 2-10
	 * Difference in Height: 		-2-2
	 * Gap between elements:		1-6
     * Difficulties: 				0-2
     * Diff. in/decrease:           each powerup
     * Powerup: 					20-60
     * Speed in/decrease: 			each powerup
	 */

	public int difficulty = 0;
	int maxDifficulty = 2;

	static System.Random rnd = new System.Random();

	// Speed / Jump
	public float jumpSpeed = 400;
	public float speed = 400;
	float speedIncrease = 150;
	bool doubleJump = true;

	public bool moveLeft = true;
	public bool moveRight = true;
	public bool collision = true;

	// Speeder: counting until next speeder comes
	public bool enableSpeeder = true;
	public int speederCounter = 10;
	GameObject speeder;

	Vector3 pos;
	float currGamePoints = 0;

	// We need this variable to stop movement
	// We need to remember a key release, when it was released mid air
	bool keyUp = false;

	// Camera/Screen/Background obejcts
	GameObject camera;
	Camera cam;
	GameObject background;
	float screenWidth = Screen.width;

	// Level Elements
	GameObject lvlElement; // Prefab
	List<GameObject> lvlElements = new List<GameObject> ();
	float lastElEnd;
	float lastElHeight;
	GameObject currLvlElement;
	int currLvlIndex;
	float bottomLine = -3F;

	// Power Ups
	GameObject powerUp; // Prefab
	List<GameObject> powerUps = new List<GameObject> ();
	float lastPowerUp_x;
	float nextPowerUp_x;
	float minDistToGaps = 0.5F;

	// Test fields
	GameObject pointsTxtGO;
	GameObject levelTxtGO;

	// Use this for initialization
	void Start () {
		// Load the Prefabs
		powerUp = LoadPrefab ("PowerUp");
		lvlElement = LoadPrefab ("LevelElement");
		speeder = LoadPrefab ("Speeder");

		// Load camera, camera component, screen width and background
		camera = GameObject.FindGameObjectWithTag ("MainCamera");
		cam = camera.GetComponent<Camera> ();
		screenWidth = Screen.width;
		background = GameObject.FindGameObjectWithTag ("BackgroundImage");

		// Get the first lvl element which is already in the scene
		currLvlElement = GameObject.FindGameObjectWithTag ("LevelElement");
		currLvlIndex = 0;
		lvlElements.Add(currLvlElement);

		// Get the power up which is already in the scene
		powerUps.Add (GameObject.FindGameObjectWithTag ("PowerUp"));
		lastPowerUp_x = 5;
		nextPowerUp_x = lastPowerUp_x + rnd.Next (20, 61);

		// Load Text Gameobjects
		levelTxtGO = GameObject.FindGameObjectWithTag("Level");
		pointsTxtGO = GameObject.FindGameObjectWithTag ("Points");
	}

	// Load Prefabs which are inside Assets/Resources/Prefabs
	GameObject LoadPrefab(string name) {
		if ((GameObject) Resources.Load ("Prefabs/" + name, typeof(GameObject)) == null) {
			Debug.Log ("Prefab '" + name + "' couldn't be found");
		}
		return (GameObject) Resources.Load ("Prefabs/" + name, typeof(GameObject));
	}
	
	// Update is called once per frame
	void Update () {
		// Get current position in coordinates
		pos = this.transform.position;

		// Update Game Points (incl. textfield)
		if (pos.x > currGamePoints) {
			currGamePoints = pos.x;
			pointsTxtGO.GetComponent<Text> ().text = "Points: " + ((int)currGamePoints).ToString ();
		}

		// Set Level Text
		setLevelText();

		if(currLvlElement.transform.position.x + (currLvlElement.transform.localScale.x/2) < pos.x) {
			currLvlIndex += 1;
			currLvlElement = lvlElements [currLvlIndex];
			bottomLine = currLvlElement.transform.position.y - 5.0F;
		}

		// Last level Element
		GameObject lastEl = lvlElements [lvlElements.Count - 1];
		Renderer lastRen = lastEl.GetComponent<Renderer> ();
		Vector3 lastElPos = lastEl.transform.position;
		lastElEnd = lastEl.transform.position [0] + (lastRen.bounds.size [0] / 2);
		lastElHeight = lastElPos [1];
		Vector3 lastElPixel = cam.WorldToScreenPoint (lastElPos);

		// Build next element (500px in advance)
		if (lastElPixel.x < cam.WorldToScreenPoint(pos).x + (screenWidth/2) + 250) {
			BuildNextElement ();
		}

		// Reset to start position if falling down
		if (pos [1] < bottomLine) {
			ResetBallToStart ();
		}

		// Up Arrow / Space
		if ((Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.Space)) && collision) {
			this.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, jumpSpeed, 0));
		} else if ((Input.GetKeyDown (KeyCode.UpArrow) || Input.GetKeyDown (KeyCode.Space)) && doubleJump) {
			this.GetComponent<Rigidbody> ().AddForce (new Vector3 (0, jumpSpeed/3, 0));
			doubleJump = false;
		}

		// Left Arrow
		if (Input.GetKeyDown (KeyCode.RightArrow)) {
			if (collision) {
				this.GetComponent<Rigidbody> ().AddForce (new Vector3 (speed, 0, 0));
			} else if (moveLeft) {
				if (!moveRight) {
					this.GetComponent<Rigidbody> ().isKinematic = true;
					this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
					this.GetComponent<Rigidbody> ().isKinematic = false;
					this.GetComponent<Rigidbody> ().AddForce (new Vector3 (speed, 0, 0));
				} else {
					this.GetComponent<Rigidbody> ().AddForce (new Vector3 (speed, 0, 0));
				}
			}
			moveLeft = false;
			moveRight = true;
		} else if (Input.GetKeyUp (KeyCode.RightArrow)) {
			keyUp = true;
		}

		// Right Arrow
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			if (collision) {
				this.GetComponent<Rigidbody> ().AddForce (new Vector3 (-speed, 0, 0));
			}
			else if (moveRight) {
				if (!moveLeft) {
					this.GetComponent<Rigidbody> ().isKinematic = true;
					this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
					this.GetComponent<Rigidbody> ().isKinematic = false;
					this.GetComponent<Rigidbody> ().AddForce (new Vector3 (-speed, 0, 0));
				} else {
					this.GetComponent<Rigidbody> ().AddForce (new Vector3 (-speed, 0, 0));
				}
			}
			moveRight = false;
			moveLeft = true;
		} else if (Input.GetKeyUp (KeyCode.LeftArrow)) {
			keyUp = true;
		}

		// We stop the ball as soon as he hits the ground
		if (keyUp && collision) {
			this.GetComponent<Rigidbody>().isKinematic = true;
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.GetComponent<Rigidbody>().isKinematic = false;
			keyUp = false;
		}

		// Esc / Reset the game
		if (Input.GetKeyDown (KeyCode.Escape)) {
			ResetBallToStart ();
			DestroyLvl ();
			currGamePoints = 0;
			lastPowerUp_x = 5;
			nextPowerUp_x = lastPowerUp_x + rnd.Next (20, 61);
		}

		// Set Camera
		camera.transform.position = new Vector3(pos[0], pos[1], camera.transform.position [2]);
		// Set Background
		background.transform.position = new Vector3(pos[0], pos[1], background.transform.position [2]);
	}

	void OnCollisionEnter(Collision target) {
		if (target.gameObject.tag.Equals("LevelElement") == true) {
			collision = true;
			doubleJump = true;
			moveRight = true;
			moveLeft = true;
		}

		if (target.gameObject.tag.Equals ("PowerUp") == true) {
			Destroy (target.gameObject);

			if(difficulty != maxDifficulty) {
				difficulty += 1;
				speed += speedIncrease;
				jumpSpeed += speedIncrease/3;
			}
		}
		if (enableSpeeder) {
			if (target.gameObject.tag.Equals ("Speeder") == true) {
				this.GetComponent<Rigidbody> ().isKinematic = true;
				this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				this.GetComponent<Rigidbody> ().isKinematic = false;
				this.GetComponent<Rigidbody> ().AddForce (new Vector3 (speed * 3, speed * 1, 0));
				Destroy (target.gameObject);
			}
		}

	}

	void OnCollisionExit(Collision target) {
		if (target.gameObject.tag.Equals("LevelElement") == true) {
			collision = false;
		}
	}

	void BuildNextElement() {
		// The higher difficulty, the less variance in height
		// The higher difficulty, the bigger the gap
		float height; 
		float gap;
		if (difficulty == 0) {
			height = rnd.Next (-2, 3);
			gap = rnd.Next (1, 3);
		} else if (difficulty == 1) {
			height = rnd.Next (-1, 2);
			gap = rnd.Next (3, 5);
		} else { // if diff. == 2
			height = 0;
			gap = rnd.Next (5, 7);
		}

		// Length is always random indipendent of diff.
		float length = rnd.Next (2, 11);


		GameObject go = Instantiate(lvlElement, new Vector3(lastElEnd + (length/2) + gap, lastElHeight + height, 0), Quaternion.identity);
		go.transform.localScale = new Vector3 (length, 1, 1);
		lvlElements.Add (go);

		// Insert power up if it is at the level element
		float rnd_height_add = (float) rnd.Next (1, 5);
		float pu_height = go.transform.position.y + rnd_height_add;
		if(nextPowerUp_x < go.transform.position.x + (go.transform.localScale.x/2)) {

			if (nextPowerUp_x < go.transform.position.x - (go.transform.localScale.x / 2)) {
				nextPowerUp_x = lastElEnd + gap / 2;
				if (gap >= 5) {
					rnd_height_add = (float)rnd.Next (3, 4);
				} else if (gap >= 3) {
					rnd_height_add = (float)rnd.Next (2, 4);
				} else {
					rnd_height_add = (float)rnd.Next (1, 4);
				}
				if (lvlElements [lvlElements.Count - 2].transform.position.y > go.transform.position.y) {
					pu_height = lvlElements [lvlElements.Count - 2].transform.position.y + rnd_height_add;
				} else {
					pu_height = go.transform.position.y + rnd_height_add;
				}
			} else if (nextPowerUp_x < go.transform.position.x - (go.transform.localScale.x / 2) + minDistToGaps) {
				nextPowerUp_x = go.transform.position.x - (go.transform.localScale.x / 2) + minDistToGaps;
			} else if (nextPowerUp_x > go.transform.position.x + (go.transform.localScale.x / 2) - minDistToGaps) {
				nextPowerUp_x = go.transform.position.x + (go.transform.localScale.x / 2) - minDistToGaps;
			}


			GameObject pu = Instantiate (powerUp, new Vector3 (nextPowerUp_x, pu_height, 0), Quaternion.identity);
			powerUps.Add (pu);

			float npu = nextPowerUp_x;
			nextPowerUp_x += rnd.Next (20, 60);
			lastPowerUp_x = npu;
		}

		// Insert speeder
		if (enableSpeeder) {
			speederCounter -= 1;
			if (speederCounter == 0) {
				if (go.transform.localScale.x > 4) {
					speederCounter = rnd.Next (15, 20);
					Instantiate (speeder, new Vector3 (go.transform.position.x, go.transform.position.y + 0.55F, 0), Quaternion.identity);
				} else {
					speederCounter = 1;
				}
			}
		}
	}

	public void DecreasePowerUpCount() {
		if(difficulty != 0) {
			difficulty -= 1;
			speed -= speedIncrease;
			jumpSpeed -= speedIncrease/3;
		}
	}

	public Vector3 GetBallPosition() {
		return pos;
	}

	void DestroyLvl() {
		List<GameObject> newLvlElements = new List<GameObject> {lvlElements [0]};
		for (int i = 1; i < lvlElements.Count; i++) {
			Destroy (lvlElements [i]);
		}
		lvlElements = newLvlElements;

		for (int i = 0; i < powerUps.Count; i++) {
			Destroy (powerUps [i]);
		}
		GameObject first_PU = Instantiate (powerUp, new Vector3 (5, 3.15F, 0), Quaternion.identity);
		powerUps = new List<GameObject> { first_PU };
	}

	void ResetBallToStart() {
		this.GetComponent<Rigidbody>().isKinematic = true;
		this.transform.position = new Vector3 (1, 1, 0);
		this.GetComponent<Rigidbody>().velocity = Vector3.zero;
		this.GetComponent<Rigidbody>().velocity = Vector3.zero;
		this.GetComponent<Rigidbody>().isKinematic = false;
		currLvlIndex = 0;
		currLvlElement = lvlElements [0];
		difficulty = 0;
		speed = 400;
		jumpSpeed = 400;
	}

	void setLevelText() {
		if (difficulty == 0) {
			levelTxtGO.GetComponent<Text> ().text = "Power-Ups: 0, Gaps: 1-2, Height: (-2)-2, Speed: 400";
		} else if (difficulty == 1) {
			levelTxtGO.GetComponent<Text> ().text = "Power-Ups: 1, Gaps: 3-4, Height: (-1)-1, Speed: 550";
		} else if (difficulty == 2) {
			levelTxtGO.GetComponent<Text> ().text = "Power-Ups: 2, Gaps: 5-6, Height: 0, Speed: 700";
		}
	
	}
}
