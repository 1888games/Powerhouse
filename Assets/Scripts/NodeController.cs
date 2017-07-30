using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class NodeController : Singleton<NodeController> {

	public Dictionary<string, Node> nodePrototypes;
	public Dictionary<string, GameObject> nodePrefabs;
	public Dictionary<Node, GameObject> nodeGameObjectMap;
	public Dictionary<GameObject, Node> houseNodeMap;
	public Dictionary<GameObject, Node> gameObjectNodeMap;
	public List<Node> nodes;
	public List<Node> nodesToProcess;
//
	int lastTimeLighted = 0;

//	string[] startNodes = { "T", "Corner", "Straight", "Y", "BirdFoot" };
//	string[] allNodes = { "Straight", "T", "Y", "Corner", "Acute", "Cross", "Arrow", "BirdFoot", "Obtuse" };
//	string[] edgeNodes = { "Straight", "T", "Y", "Corner", "Acute", "Arrow", "Obtuse" };
//	float[] rotations = { 0f, 45f, 90f, 135f, 180f, 225f, 270f, 315f };

	string[] startNodes = { "T", "Corner", "Straight" };
	string[] allNodes = { "T", "Corner", "Straight", "Cross" };
	string[] edgeNodes = { "T", "Corner", "Straight" };
	float[] rotations = { 0f,  90f,  180f, 270f };


	// Use this for initialization
	void Start () {

		nodeGameObjectMap = new Dictionary<Node, GameObject> ();
		houseNodeMap = new Dictionary<GameObject, Node> ();
		gameObjectNodeMap = new Dictionary<GameObject, Node> ();
		nodes = new List<Node> ();
		nodesToProcess = new List<Node> ();
		createNodePrototypes ();
		loadNodePrefabs ();

	}
	
	// Update is called once per frame
	void Update () {

		foreach (Node n in nodes) {

			n.UpdateRotation ();

		}
	}



	public void populateGrid () {







	}



	public void turnToFaceSource(Node n, float fromRotation ) {

		bool okay = false;


		List<float> useRotations = new List<float>(rotations);
		Helpers.Shuffle (useRotations);

		//Debug.Log (useRotations.Count + " looking for rotation for " + n.name + " in tile " + n.tile.x + " - " + n.tile.y);


		while (okay == false && useRotations.Count > 0) {

			bool abandon = false;

			foreach (Edge e in n.edges) {
				float difference = Mathf.Abs (e.convertDirection (useRotations [0]) - fromRotation);

				if (difference == 180f && abandon == false && okay == false) {
					okay = true;
					//Debug.Log ("Can get to source tile using rotation " + useRotations [0]);

					if (n.tile.isEdge && nodes.Count > 1) {

						foreach (Edge e2 in n.edges) {

							float direction = e2.convertDirection (useRotations [0]);
							Tile t = n.tile.getNeighbour ((int)direction);


							if (t == null) {
								//Debug.Log (n.tile.x + " / " + n.tile.y + " " + direction + " + points out of bounds");
								abandon = true;
								okay = false;
								break;
							} else {
							//	Debug.Log (direction + " + points to tile " + t.x + " - " + t.y);
							}


						}


					}



				}

				if (okay) {
					n.forceRotation (useRotations [0]);
				}

			

			}

			if (okay == false) {
				useRotations.RemoveAt (0);
			} else {
				n.forceRotation (useRotations [0]);
			}
		}

		if (useRotations.Count == 0) {
			Debug.Log ("Couldn't find a valid node...");
		}

	}



	public void exploreEdges(Node n) {

		bool ended = true;

		foreach (Edge e in n.edges) {

			Tile t = n.tile.getNeighbour ((int)e.convertDirection (n.rotation));

			if (t != null && t.installedNode == null) {

				ended = false;
				newNode (t, e.convertDirection (n.rotation) );
			

				//Debug.Log (n.name + " has an edge pointing in " + e.convertDirection (n.rotation) + " to tile " + t.x + " - " + t.y);

			}

		}

		nodesToProcess.Remove (n);

		//Debug.Log ("Processing: " + nodesToProcess.Count);

		if (nodesToProcess.Count == 0) {
		//if (ended) {

			randomizeNodes ();
			GameController.Instance.secondsLeft = Mathf.RoundToInt(nodes.Count * 1.5f / 5) * 5 + 10;


			Text levelText = GameObject.FindGameObjectWithTag ("Time").GetComponent<Text>();
			levelText.text = "Time: " + GameController.Instance.secondsLeft;

			GameController.Instance.startClock ();
		


		//}
		}




	}

	public void randomizeNodes () {

		foreach (Node n in nodes) {

			float origRotation = n.rotation;

			int choose = Random.Range (0, 4);

			if (rotations [choose] == n.rotation) {
				choose = Random.Range (0, 4);
			}
				
			if (n.id == 0 && n.name == "Straight" && (rotations[choose] == 0 || rotations[choose] == 180)) {
				choose = 3;
			}
				
			if (n.id == 0 && n.name == "Corner") {

				if (n.rotation == 90) {
					choose = 0;
				}

				if (n.rotation == 180) {
					choose = 3;
				}

			}

			if (n.id == 0 && n.name == "T") {

				choose = 3;

			}

		

			n.forceRotation ( rotations [choose]);


		}

		calculatePower ();


	}

	public void calculatePower () {

		Debug.Log ("Calc power...");
		Node startNode = nodes [0];

		foreach (Node n in nodes) {
			n.hasPower = false;
		}

	
		foreach (Edge e in startNode.edges) {

			if (e.convertDirection (startNode.rotation) == 180f) {
				
				startNode.hasPower = true;

			}
		}

		if (startNode.hasPower) {
			distributePower (startNode, 0f);
			doLighting ();

		} else {
			Debug.Log ("Start node has NO power...");
			doLighting ();

		}

	}

	public void distributePower (Node n, float fromRotation ) {

		foreach (Edge e in n.edges) {

			float diffToIncoming = Mathf.Abs ((e.convertDirection (n.rotation) - fromRotation));

			//Debug.Log (n.tile.x + " / " + n.tile.y + " Exploring edge of " + e.convertDirection (n.rotation) + " came from " + fromRotation + " gives diff of " + diffToIncoming);

			if (diffToIncoming != 180f) {

				Tile t = n.tile.getNeighbour ((int)e.convertDirection (n.rotation));

				if (t != null && t.installedNode != null && t.installedNode.hasPower == false) {

					foreach (Edge e2 in t.installedNode.edges) {

						float difference = Mathf.Abs (e2.convertDirection (t.installedNode.rotation) - e.convertDirection (n.rotation));

						//Debug.Log ("Compare to " + t.x + " / " + t.y + " with edge of " + e2.convertDirection (t.installedNode.rotation) + " gives diff of " + difference);

						if (difference == 180f) {

							t.installedNode.hasPower = true;
							distributePower (t.installedNode, e.convertDirection (n.rotation)); 


						}
					}



				}

			}


				
		

		}


		//Debug.Log ("Done with power...");

	}



	void doLighting () {

		bool completedLevel = true;
		int haveLight = 0;

		foreach (Node n in nodes) {

			if (n.light != null) {

				//n.light.gameObject.SetActive (false);
				if (n.hasPower) {

					n.light.gameObject.SetActive (true);
					haveLight = haveLight + 1;

				} else {
					completedLevel = false;
					n.light.gameObject.SetActive (false);
				}


				foreach (Renderer r in n.renderers) {

					//Debug.Log ("Got a renderer...");

					if (n.hasPower) {
						r.material.SetColor ("_Color", Color.yellow);
					} else {
						r.material.SetColor ("_Color", Color.red);
					}

				}

			}
		}

		Text litText = GameObject.FindGameObjectWithTag ("Lit").GetComponent<Text>();
		litText.text = "Houses Lit: " + haveLight + " / " + nodes.Count;

		if (haveLight > lastTimeLighted) {

			GameController.Instance.source.PlayOneShot (GameController.Instance.powerUp);

		}

		if (haveLight < lastTimeLighted) {

			GameController.Instance.source.PlayOneShot (GameController.Instance.powerDown);

		}

		lastTimeLighted = haveLight;

		if (completedLevel) {
			GameController.Instance.completeLevel ();
		}




	}



	public void newNode (Tile t, float fromRotation ) {

		if (t.installedNode == null) {
			
			string[] nodeNames = allNodes;

			if (nodes.Count == 0) {
				nodeNames = startNodes;
			} else {
				if (t.isEdge) {
					nodeNames = edgeNodes;
				}

			}

			int lengthUse = nodeNames.Length;

			if (t.y == 0 && fromRotation == 180f) {
				lengthUse = 2;
			}

			if (t.y == GameController.Instance.size - 1 && fromRotation == 0f) {
				lengthUse = 2;
			}

			if (t.x == 0 && fromRotation == 270f) {
				lengthUse = 2;
			}

			if (t.x == GameController.Instance.size - 1 && fromRotation == 90f) {
				lengthUse = 2;
			}

	
			
			string nodeName = nodeNames [Random.Range (0, lengthUse)];

			Node newNode = NodeController.Instance.nodePrototypes [nodeName].Clone ();
			newNode.setTile (t);
			t.installedNode = newNode;
			newNode.id = nodes.Count;

			GameObject go = Instantiate (GameController.Instance.house);
			go.transform.position = t.getHousePosition ();
			go.transform.localScale = new Vector3 (1.5f, 1f, 1.5f);
			go.transform.SetParent (GameController.Instance.board.transform);

			//go.transform.localScale.z = 1.5f;

			newNode.house = go;

			houseNodeMap.Add (go, newNode);

			go.name = "House";
			newNode.name = "Node";

	



			go.transform.eulerAngles = new Vector3 (0f, (float)rotations [Random.Range (0, 4)], 0f);

			newNode.light = go.transform.Find ("Area Light");

			GameObject nodeObj = Instantiate (NodeController.Instance.nodePrefabs [nodeName]);
			nodeObj.transform.position = t.getNodePosition ();
			nodeObj.transform.position = new Vector3 (nodeObj.transform.position.x, nodeObj.transform.position.y, nodeObj.transform.position.z);
			nodeObj.name = "Node";
			nodeObj.transform.SetParent (GameController.Instance.board.transform);

			newNode.renderers = nodeObj.transform.GetComponentsInChildren<Renderer>();

			NodeController.Instance.nodes.Add (newNode);
			NodeController.Instance.nodesToProcess.Add (newNode);
			NodeController.Instance.nodeGameObjectMap.Add (newNode, nodeObj);
			NodeController.Instance.gameObjectNodeMap.Add (nodeObj, newNode);

			//newNode.rotateNode (60);


			turnToFaceSource (newNode, fromRotation);
			exploreEdges (newNode);

		}
	

	}

	public void displayNode (Node n) {

		//Debug.Log("Display node in tile " + n.tile.x +  " + " + n.tile.y);



	}


	public void rotateNode (Node n) {

		Transform trans = nodeGameObjectMap [n].transform;

		//Debug.Log (trans.rotation.y + " " + n.rotation);
		trans.localEulerAngles = (new Vector3 (0, n.rotation, 0));



	}

	void loadNodePrefabs () {

		nodePrefabs = new Dictionary<string, GameObject> ();

		GameObject prefabs = GameObject.FindGameObjectWithTag ("Prefabs");	// find the prefabs 'folder'

		Transform nodes = prefabs.transform.Find ("Nodes");	// find the pieces folder within it

		nodes.gameObject.SetActive (true);		// set it active so we can iterate through children

		foreach (Transform child in nodes) {		// get the children of pieces (piece prefabs)

			nodePrefabs.Add (child.name, child.gameObject);		// add this piece to the prefabs dictionary so we can grab it by name

		}

		nodes.gameObject.SetActive (false);	// reset the pieces folder to inactive so these objects don't show


	}

	void createNodePrototypes() {

		nodePrototypes = new Dictionary<string, Node> ();


		List<Edge> edges;

		edges = new List<Edge> ();
		edges.Add (new Edge (0));
		edges.Add (new Edge (180));
		Node n = new Node ("Straight", edges);
		nodePrototypes.Add (n.name, n);

		edges = new List<Edge> ();
		edges.Add (new Edge (0));
		edges.Add (new Edge (90));
		n = new Node ("Corner", edges);
		nodePrototypes.Add (n.name, n);

//		edges = new List<Edge> ();
//		edges.Add (new Edge (0));
//		edges.Add (new Edge (45));
//		n = new Node ("Acute", edges);
//		nodePrototypes.Add (n.name, n);
//
//		edges = new List<Edge> ();
//		edges.Add (new Edge (0));
//		edges.Add (new Edge (135));
//		n = new Node ("Obtuse", edges);
//		nodePrototypes.Add (n.name, n);

		edges = new List<Edge> ();
		edges.Add (new Edge (0));
		edges.Add (new Edge (90));
		edges.Add (new Edge (180));
		n = new Node ("T", edges);
		nodePrototypes.Add (n.name, n);

//		edges = new List<Edge> ();
//		edges.Add (new Edge (0));
//		edges.Add (new Edge (135));
//		edges.Add (new Edge (225));
//		n = new Node ("Y", edges);
//		nodePrototypes.Add (n.name, n);
//
//		edges = new List<Edge> ();
//		edges.Add (new Edge (0));
//		edges.Add (new Edge (135));
//		edges.Add (new Edge (225));
//		n = new Node ("Arrow", edges);
//		nodePrototypes.Add (n.name, n);
//
//		edges = new List<Edge> ();
//		edges.Add (new Edge (0));
//		edges.Add (new Edge (135));
//		edges.Add (new Edge (180));
//		edges.Add (new Edge (225));
//		n = new Node ("BirdFoot", edges);
//		nodePrototypes.Add (n.name, n);

		edges = new List<Edge> ();
		edges.Add (new Edge (0));
		edges.Add (new Edge (90));
		edges.Add (new Edge (180));
		edges.Add (new Edge (270));
		n = new Node ("Cross", edges);
		nodePrototypes.Add (n.name, n);


	}
}
