using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

	public List <Edge> edges;	// a list of edges that point in different directions
	public string name;	// the name of this node (i.e T, Cross, Obtuse)
	public Tile tile;	// the tile the node sits in
	public float rotation = 0f;
	public float leftToRotate = 0f;
	public float targetRotation = 0f;
	public float speed = 2.5f;
	public float baseSpeed = 4.5f;
	public bool hasPower = false;
	public GameObject house;
	public Transform light;
	public int id;
	public Renderer[] renderers;

	public Node (string name, List<Edge> edgesIn) {

		edges = new List<Edge> ();

		foreach (Edge e in edgesIn) {

			edges.Add (e);

		}

		this.name = name;





	}

	public void distributePower () {



	}

	public void UpdateRotation () {



		if (rotation != targetRotation) {

			//Debug.Log (rotation + " " + targetRotation);

			if (targetRotation > rotation) {
				rotation = rotation + speed;
			} else {
				rotation = rotation - speed;
			}



			if (Mathf.Abs(rotation - targetRotation) <= speed) {
				rotation = targetRotation;
				if (rotation == 360f) {
					rotation = 0f;
					targetRotation = 0f;
				}

				if (rotation == -90f) {
					rotation = 270f;
					targetRotation = 270f;
				}

				NodeController.Instance.calculatePower ();
				GameController.Instance.canClick = true;

			}

			NodeController.Instance.rotateNode (this);

			//Debug.Log (rotation);

		}





	}

	private Node (Node proto) {

		name = proto.name;

		edges = new List<Edge> ();
	

		foreach (Edge e in proto.edges) {

			edges.Add (e);

		}


	}

	public Node Clone () {

		return new Node (this);


	}

	public void setTile (Tile t) {

		tile = t;

		NodeController.Instance.displayNode (this);


	}


	public void rotateNode (float amount = 45) {

		GameController.Instance.canClick = false;



		speed = baseSpeed / 45f * Mathf.Abs(amount);

		targetRotation = rotation + amount;
		//Debug.Log (rotation + " plus " + amount + " = " + targetRotation);

		if (targetRotation > 360f) {

			targetRotation = targetRotation - 360f;
		}

		if (targetRotation < -1f) {
			//targetRotation= targetRotation + 360f;
		}

	//	Debug.Log (rotation + " plus " + amount + " = " + targetRotation);

	

	}

	public void forceRotation (float amount) {

		if (amount > 360f) {

			amount = amount - 360f;
		}

		if (amount < 0f) {
			amount = amount + 360f;
		}

		rotation = amount;
		targetRotation = amount;
		NodeController.Instance.rotateNode (this);

	}
}
