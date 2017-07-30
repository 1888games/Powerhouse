using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge  {
	
	public float direction;		// the direction this edge points in
	public bool hasPower;		// whether this edge is connected to the grid

	public Edge (int direction, bool hasPower = false) {


		this.direction = (float)direction;
		this.hasPower = hasPower;


	}

	public float convertDirection (float rotation) {

		float newDirection = direction + rotation + 0f;

		//sDebug.Log (direction + " " + rotation);

		if (newDirection > 359f) {

			newDirection = newDirection - 360f;
		}

		if (newDirection < 0f) {
			newDirection= newDirection + 360f;
		}



		return newDirection;

	}
}
