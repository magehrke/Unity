using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class PathStruct{
	
	public int cost;
	
	public WayPoint wayPoint;
	public PathStruct predecessor;
	
	public PathStruct(int cost, WayPoint wayPoint, PathStruct predecessor){
		this.cost = cost;
		this.wayPoint = wayPoint;
		this.predecessor = predecessor;
	}
	
}

public class PathfindingScript {

	static int pathCost = 1;
	static List<Vector3> visited = new List<Vector3>();
	static SortedDictionary<int, List<PathStruct>> openList = new SortedDictionary<int, List<PathStruct>>();


	//Die Geister nutzen diese Funktion um zufällig herumzulaufen.
	//Der berechnet Weg soll fünf Felder enthalten.
	//pacmanWayPoint: die Position von Pacman
	//Die zurückgegebene Liste soll fünf Wegpunkte beinhalten.
	//Falls benötigt, darf aber auch der aktuelle Wegpunkt als erster Punkt enthalten sein.
	public static List<WayPoint> getRandomPath (WayPoint pacmanWayPoint){
		
		List<WayPoint> path = new List<WayPoint>();
		
		WayPoint wp = pacmanWayPoint;
		
		//+1 because the current node will be in the path.
		for (int i = 0; i < 4 + 1; i++) {
			int numberOfOptions = wp.getConnectedWaypoints().Count;
			int randomDirection = UnityEngine.Random.Range(0, numberOfOptions);
			
			path.Add(wp);
			wp = wp.getConnectedWaypoints()[randomDirection];
		}
		
		return path;
	}

	//Die Geister nutzen diese Funktion um vor Pacman zu fliehen.
	//Es soll ein Fluchtweg der Länge 1 berechnet werden.
	//pacmanWayPoint: die Position von Pacman
	//currentWayPoint: die Position des Geistes
	//Die zurückgegebene Liste soll als letzten Eintrag den gewollten Fluchtwegpunkt
	//enthalten.
	//Falls benötigt, darf aber auch der aktuelle Wegpunkt als erster Punkt enthalten sein.
	public static List<WayPoint> getEscapePath (WayPoint pacmanWayPoint, WayPoint currentWaypoint){

		List<WayPoint> path = new List<WayPoint>();
		
		path.Add(currentWaypoint);
		WayPoint wp = currentWaypoint;
		
		
		WayPoint tmpWP = wp;
		if (currentWaypoint.getConnectedWaypoints().Count > 0) {
			tmpWP = currentWaypoint.getConnectedWaypoints()[0];
		}
		float maxdistance = manhattenDistance(tmpWP.transform.position, pacmanWayPoint.transform.position);
		
		foreach (WayPoint waypoint in wp.getConnectedWaypoints()) {
			float distance = manhattenDistance(waypoint.transform.position, pacmanWayPoint.transform.position);
			if (distance > maxdistance) {
				maxdistance = distance;
				tmpWP = waypoint;
			}
		}
		
		path.Add(tmpWP);
		
		return path;
	}

	//destination: der ZielPunkt (Pacmans Position)
	//start: aktuelle Positionn des Geistes
	//die zurückgegebene Liste soll als ersten Wegpunkt den Startpunkt und als letzten Wegpunkt den
	//Zielpunkt enthalten.
	public static List<WayPoint> getPath(WayPoint destination, WayPoint start){

		List<WayPoint> path = new List<WayPoint>();
		
		
		//initialize
		int cost = 0;
		int heuristicValue = manhattenDistance(destination.transform.position, start.transform.position);
		
		//List of visited positions. Needed to ensure we don't revisit places.
		visited = new List<Vector3>();
		//using the key as the cost + heuristic value. Value is a struct with the waypoint and the path to this one.
		openList = new SortedDictionary<int, List<PathStruct>>();
		//insert the starting point
		
		PathStruct startPath = new PathStruct(cost, start, null);
		List<PathStruct> startList = new List<PathStruct>();
		startList.Add(startPath);
		openList.Add((cost + heuristicValue), startList);
		bool finished = false;
		PathStruct currentPoint = startPath;

		//the loop
		while (!finished && openList.Count > 0) {

			//get all waypoints with lowest total cost
			List<PathStruct> lowestCostList = new List<PathStruct>(openList.First().Value);
			openList.Remove(openList.First().Key);

			//loop over these points
			for (int i = 0; i < lowestCostList.Count; i++) {
				//get the first one
				currentPoint = lowestCostList[i];

				//did we reach the goal?
				if (currentPoint.wayPoint.transform.position.Equals(destination.transform.position)) {
					finished = true;
					break;
				} else {
					
					//try to expand it in all four directions:
          					//up:

					WayPoint nextWayPoint = currentPoint.wayPoint.upWaypoint;
					expandNode(nextWayPoint, currentPoint, destination);
					//down
					nextWayPoint = currentPoint.wayPoint.downWaypoint;
					expandNode(nextWayPoint, currentPoint, destination);
					//left:
					nextWayPoint = currentPoint.wayPoint.leftWaypoint;
					expandNode(nextWayPoint, currentPoint, destination);
					//right:
					nextWayPoint = currentPoint.wayPoint.rightWaypoint;
					expandNode(nextWayPoint, currentPoint, destination);
				}
				
			}
			
		}
		
		if (finished) {
			while (currentPoint.predecessor != null)  {
				path.Insert(0, currentPoint.wayPoint);
				currentPoint = currentPoint.predecessor;
			}
			path.Insert(0, currentPoint.wayPoint);
		}
		return path;
	}
	
	private static void expandNode(WayPoint nextWayPoint, PathStruct currentPoint, WayPoint destination) {

		if (nextWayPoint != null) {
			//check if thie new waypoint has already been visited
			if (!visited.Contains(nextWayPoint.transform.position)) {
				visited.Add(nextWayPoint.transform.position);
				
				//calculate the total costs
				int totalCosts = currentPoint.cost + pathCost +
					manhattenDistance(destination.transform.position, nextWayPoint.transform.position);
				
				//create PathStruct
				PathStruct newPathStruct = new PathStruct(currentPoint.cost + pathCost, nextWayPoint, currentPoint);
				
				//add the struct to the openlist
				if (openList.ContainsKey(totalCosts)) {
					openList[totalCosts].Add(newPathStruct);
				} else {
					openList.Add(totalCosts, new List<PathStruct>(){newPathStruct});
				}
			} else {
			}
			
		} else {
		}// else there is no waypoint upwards. 
	}
	
	private static int manhattenDistance (Vector3 a, Vector3 b) {
		return (int)Math.Abs(a.x - b.x) + (int)Math.Abs(a.z - b.z);
	}
}
