using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile {

	public int x { get; protected set; }
	public int y { get; protected set; }
	public Node installedNode;
	public bool isEdge = false;

	public Tile (int x, int y) {

		this.x = x;
		this.y = y;

		if (x == 0 || x == GameController.Instance.size - 1 || y == 0 || y == GameController.Instance.size - 1) {
			isEdge = true;
		}

	}


	public Vector3 getWorldPosition () {

		return new Vector3 (1.5f + 3f * (x), 0.01f, 1.5f + 3f* (y));

	}

	public Vector3 getHousePosition ( ) {

		Vector3 tilePos = getWorldPosition ();

		Vector3 housePos = new Vector3 (tilePos.x, tilePos.y + 0.53f, tilePos.z);

		return housePos;
	}

	public Vector3 getNodePosition () {

		Vector3 tilePos = getWorldPosition ();

		Vector3 nodePos = new Vector3 (tilePos.x, 1.52f, tilePos.z);

		return nodePos;

	}

	public Tile getNeighbour(int angle) {

		switch (angle) {

		case 0:

			return GameController.Instance.getTileAt (x, y + 1);

		case 45:

			return GameController.Instance.getTileAt (x + 1, y + 1);

		case 90:

			return GameController.Instance.getTileAt (x + 1, y);


		case 135:

			return GameController.Instance.getTileAt (x + 1, y - 1);

		case 180:

			return GameController.Instance.getTileAt (x, y - 1);

		case 225:

			return GameController.Instance.getTileAt (x - 1, y -1);

		case 270:

			return GameController.Instance.getTileAt (x - 1, y);

		case 315:

			return GameController.Instance.getTileAt (x - 1, y + 1);

		

		}

		return null;



	}
}
