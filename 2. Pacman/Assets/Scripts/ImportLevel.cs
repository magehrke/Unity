using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using UnityEngine.UI;

public class ImportLevel : MonoBehaviour {

	string filename;

	// Level elements
	GameObject corner;
	GameObject corridor;
	GameObject cross;
	GameObject deadend;
	GameObject tcross;

	// Persons
	GameObject pacman;
	GameObject blinky;
	GameObject clyde;
	GameObject inky;
	GameObject pinky;

	// Error
	GameObject errorGo;
	Text errorText;
	bool error = false;
	string error_msg = "";
	GameObject textPanel;

	// Use this for initialization
	void Start () {
		corner = LoadPrefab("Levelelements/Corner");
		corridor = LoadPrefab("Levelelements/Corridor");
		cross = LoadPrefab("Levelelements/Cross");
		deadend = LoadPrefab("Levelelements/DeadEnd");
		tcross = LoadPrefab("Levelelements/TCross");

		pacman = LoadPrefab("Pacman");
		blinky = LoadPrefab("Blinky");
		clyde = LoadPrefab("Clyde");
		inky = LoadPrefab("Inky");
		pinky = LoadPrefab("Pinky");

		errorGo = GameObject.FindGameObjectWithTag ("ErrorText");
		errorText = errorGo.GetComponent<Text> ();
		textPanel = GameObject.FindGameObjectWithTag ("Panel");
	}

	GameObject LoadPrefab(string name) {
		if ((GameObject) Resources.Load ("Prefabs/" + name, typeof(GameObject)) == null) {
			Debug.Log ("Prefab '" + name + "' couldn't be found");
		}

		return (GameObject) Resources.Load ("Prefabs/" + name, typeof(GameObject));
	}
	
	// Update is called once per frame
	void Update () {
		if (filename != null) {
			gameObject.SetActive (false);
			errorGo.SetActive (false);
			textPanel.SetActive (false);
			ReadFile ();
		}

		if (error) {
			errorGo.SetActive (true);
			errorText.text = error_msg;
			textPanel.SetActive (true);
		}

		if (Input.GetKey("escape")) {
			Application.Quit();
		}
	}

	public void Text_Changed(string new_file) {
		filename = new_file;
	}

	void ReadFile() {
		filename = filename + ".pac";
		if (!System.IO.File.Exists (filename)) {
			Debug.Log ("FILE NOT FOUND!");
		}

		// ---------------- READ FILE --------------------- //
		List<string> allLines = new List<string>(System.IO.File.ReadAllLines (filename));
		List<string> levelLines = new List<string>();
		List<string> figureLines = new List<string>();

		foreach(string line in allLines) {
			string c_line = line.ToLower ().Trim ();

			if (Regex.IsMatch (c_line, @"^[|+-]+$")) {
				levelLines.Add (c_line);
			} else if(Regex.IsMatch(c_line, @"^(pacman|blinky|inky|pinky|clyde) (\d+) (\d+)$")) {
				figureLines.Add(c_line);
			} else if(c_line == "") {
				continue;
			} else {
				error_msg = "Syntax error: The level file contains invalid lines.";
				error = true;
			}
		}

		int levelLength = levelLines [0].Length;
		int levelHeight = levelLines.Count;

		for (int i = 1; i < levelLines.Count; i++) {
			if (levelLength != levelLines [i].Length) {
				if (error_msg.Equals ("")) {
					error_msg = "Syntax error: The level lines do not have the same length.";
				}
				error = true;
			}
		}



		// ---------------- CREATE LEVEL ------------------- //
		// For a better display in Unity, we build the level
		// in x and -z direction. So if Pacman shall start at
		// 1, 3 it has to start at 1, -3

		float pos_x = 0; // for each char we move 1 in x
		float pos_z = 0; // for each new line we move -1 in z

		// List with the gameobjects we create during building the level
		List<List<GameObject>> gos_container = new List<List<GameObject>> ();
			
		for(int i = 0; i < levelLines.Count; i++) {

			List<GameObject> gos = new List<GameObject> ();

			for(int j = 0; j < levelLines[i].Length; j++) {
				char c = levelLines [i] [j];

				List<bool> neighbours = getNeighbours (c, levelLines, i, j);

				bool up = neighbours [0];
				bool down = neighbours [1];
				bool right = neighbours [2];
				bool left = neighbours [3];

				switch(c) {
				// MINUS - HORIZONTAL CORRIDOR
				case '-':

					// DEADEND
					if (right && !left) {
						gos.Add(Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 270, 0))));
					} else if (!right && left) {
						gos.Add(Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 90, 0))));
					}

					// CORRIDOR
					else {
						gos.Add(Instantiate (corridor, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 90, 0))));
					}
					break;

				// BAR - VERTICAL CORRIDOR
				case '|':

					// DEADEND
					if (up && !down) {
						gos.Add(Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 180, 0))));
					} else if (!up && down) {
						gos.Add(Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 0, 0))));
					} 

					// CORRIDOR
					else {
						gos.Add(Instantiate (corridor, new Vector3 (pos_x, 0, pos_z), Quaternion.identity));
					}
					break;

				// PLUS - INTERSECTIONS
				case '+':
					// CORNER
					if (!up && down && right && !left) {
						gos.Add (Instantiate (corner, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 270, 0))));
					} else if (!up && down && !right && left) {
						gos.Add (Instantiate (corner, new Vector3 (pos_x, 0, pos_z), Quaternion.identity));
					} else if (up && !down && right && !left) {
						gos.Add (Instantiate (corner, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 180, 0))));
					} else if (up && !down && !right && left) {
						gos.Add (Instantiate (corner, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 90, 0))));
					} 

					// CROSS
					else if (up && down && right && left) {
						gos.Add (Instantiate (cross, new Vector3 (pos_x, 0, pos_z), Quaternion.identity));
					} 

					// TCROSS
					else if (!up && down && right && left) {
						gos.Add (Instantiate (tcross, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 90, 0))));
					} else if (up && !down && right && left) {
						gos.Add (Instantiate (tcross, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 270, 0))));
					} else if (up && down && !right && left) {
						gos.Add (Instantiate (tcross, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 180, 0))));
					} else if (up && down && right && !left) {
						gos.Add (Instantiate (tcross, new Vector3 (pos_x, 0, pos_z), Quaternion.identity));
					} 

					// DEADEND
					else if (up && !down && !right && !left) {
						gos.Add (Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 180, 0))));
					} else if (!up && down && !right && !left) {
						gos.Add (Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.identity));
					} else if (!up && !down && right && !left) {
						gos.Add (Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 270, 0))));
					} else if (!up && !down && !right && left) {
						gos.Add (Instantiate (deadend, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 90, 0))));
					}

					// CORRIDOR
					else if (!up && !down && right && left) {
						gos.Add (Instantiate (corridor, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 90, 0))));
					} else if (up && down && !right && !left) {
						gos.Add (Instantiate (corridor, new Vector3 (pos_x, 0, pos_z), Quaternion.Euler (new Vector3 (0, 0, 0))));
					}

					// Point
					else if (!up && !down && !right && !left) {
						gos.Add (Instantiate (cross, new Vector3 (pos_x, 0, pos_z), Quaternion.identity));
					}
						
					break;
				}


				if (j != 0) {
					// Set left waypoint of current game object to the waypoint of the game object of the last iteration
					if(left) gos [j].GetComponent<WayPoint> ().leftWaypoint = gos [j-1].GetComponent<WayPoint> ();
			
					// Set right waypoint of game object of the last iteration to the waypoint of current game object
					if(getNeighbours(levelLines[i][j-1], levelLines, i, j-1)[2]) gos [j-1].GetComponent<WayPoint> ().rightWaypoint = gos [j].GetComponent<WayPoint> ();
				}

				if (i != 0) {
					// Set up waypoint of current game object to the waypoint of the game object of the last level line
					if(up) gos[j].GetComponent<WayPoint> ().upWaypoint = gos_container[i-1][j].GetComponent<WayPoint> ();

					// Set down waypoint of the game object of the last level line to the waypoint of the current game object
					if (getNeighbours(levelLines[i-1][j], levelLines, i-1, j)[1]) gos_container [i - 1] [j].GetComponent<WayPoint> ().downWaypoint = gos [j].GetComponent<WayPoint> ();
				}

				pos_x += 1;
			}
			gos_container.Add (gos);

			pos_x = 0;
			pos_z -= 1;
		}

		// ---------------- CREATE PERSONS ------------------- //
		bool pacman_exists = false;
		bool blinky_exists = false;
		bool clyde_exists = false;
		bool inky_exists = false;
		bool pinky_exists = false;

		List<GameObject> persons = new List<GameObject> ();

		foreach (string line in figureLines) {
			var matches = Regex.Matches (line, @"^(pacman|blinky|inky|pinky|clyde) (\d+) (\d+)$");

			string personName = matches[0].Groups[1].Value.ToLower().Trim();
			int posX = Int32.Parse(matches[0].Groups[2].Value);
			int posZ = Int32.Parse(matches[0].Groups[3].Value);

			GameObject go = new GameObject ();

			// ERROR
			// Print out an error message, if the persons are situated outside the level
			if (ioor (levelLines, posZ, posX) == ' ') {
				if (error_msg.Equals ("")) {
					error_msg = "Semantik error: pacman or a ghost is not situated inside the level.";
				}
				error = true;
				continue;
			}

			if (personName == "pacman") {
				go = Instantiate (pacman, new Vector3 (posX, 0, -posZ), Quaternion.identity);
				go.GetComponent<PlayerControlScript> ().currentWaypoint = gos_container [posZ] [posX].GetComponent<WayPoint> ();

				if (pacman_exists) {
					if(error_msg.Equals("")) {
						error_msg = "Semantik error: There are two pacman in the game!";
					}
					error = true;
				}
				pacman_exists = true;
				persons.Add (go);
			} else {
				switch (personName) {
				case "blinky":
					go = Instantiate (blinky, new Vector3 (posX, 0.25F, -posZ), Quaternion.identity);
					blinky_exists = true;
					break;

				case "clyde":
					go = Instantiate (clyde, new Vector3 (posX, 0.25F, -posZ), Quaternion.identity);
					clyde_exists = true;
					break;

				case "inky":
					go = Instantiate (inky, new Vector3 (posX, 0.25F, -posZ), Quaternion.identity);
					inky_exists = true;
					break;

				case "pinky":
					go = Instantiate (pinky, new Vector3 (posX, 0.25F, -posZ), Quaternion.identity);
					pinky_exists = true;
					break;
				}

				go.GetComponent<EnemyBehaviourScript> ().currentWaypoint = gos_container [posZ] [posX].GetComponent<WayPoint> ();
				persons.Add (go);
			}
		}

		if (!pacman_exists || !blinky_exists || !clyde_exists || !inky_exists || !pinky_exists) {
			if(error_msg.Equals("")) {
				error_msg = "Semantik error: There are not at least one ghost each and/or pacman in the level.";
			}
			error = true;
		}
			
		if (error) {
			foreach (GameObject go_x in persons) {
				Destroy (go_x);
			}
		}

	}


	List<bool> getNeighbours(char position, List<string> levelLines, int i, int j) {
		bool up = false, down = false, right = false, left = false;

		switch (position) {
		case '-':
			right = Regex.IsMatch (ioor (levelLines, i, j + 1).ToString (), @"^[-+]$") ? true : false;
			left = Regex.IsMatch (ioor (levelLines, i, j - 1).ToString (), @"^[-+]$") ? true : false;
			break;
		case '|':
			up = Regex.IsMatch (ioor (levelLines, i - 1, j).ToString (), @"^[|+]$") ? true : false;
			down = Regex.IsMatch (ioor (levelLines, i + 1, j).ToString (), @"^[|+]$") ? true : false;
			break;
		case '+':
			up = Regex.IsMatch (ioor (levelLines, i - 1, j).ToString (), @"^[|+]$") ? true : false;
			down = Regex.IsMatch (ioor (levelLines, i + 1, j).ToString (), @"^[|+]$") ? true : false;
			right = Regex.IsMatch (ioor (levelLines, i, j + 1).ToString (), @"^[-+]$") ? true : false;
			left = Regex.IsMatch (ioor (levelLines, i, j - 1).ToString (), @"^[-+]$") ? true : false;
			break;
		}

		return new List<bool> {up, down, right, left};

	}



	// Return char at the desired postion
	// If the index is out of range return ' '
	char ioor(List<string> strings, int stringIndex, int charIndex) {
		try {
			return strings[stringIndex][charIndex];
		} catch(Exception e) {
			if (e is IndexOutOfRangeException || e is ArgumentOutOfRangeException) {
				return ' ';
			}
		}
		return ' ';
	}
}
