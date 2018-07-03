using UnityEngine;
using System.Collections.Generic;

public class WayPoint : MonoBehaviour {

	public WayPoint upWaypoint;
	public WayPoint leftWaypoint;
	public WayPoint downWaypoint;
	public WayPoint rightWaypoint;

	public List<WayPoint> getConnectedWaypoints(){
		List<WayPoint> connectedWaypoints = new List<WayPoint>();
		
		if (upWaypoint != null) {
			connectedWaypoints.Add(upWaypoint);
		}
		if (downWaypoint != null) {
			connectedWaypoints.Add(downWaypoint);
		}
		if (leftWaypoint != null) {
			connectedWaypoints.Add(leftWaypoint);
		}
		if (rightWaypoint != null) {
			connectedWaypoints.Add(rightWaypoint);
		}
		
		return connectedWaypoints;
	}
}
