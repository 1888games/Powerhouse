using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : Singleton<GameController> {

	[System.NonSerialized]
	public bool canClick = true;

	public GameObject tilePrefab;
	public GameObject board;
	public GameObject house;
	public GameObject road;
	GameObject gameOverPanel;
	public Tile[,] tiles;
	public Dictionary<Tile, GameObject> tileGameObjectMap;
	public int size;


	public bool populating = false;
	public int currentLevel;
	public int baseSeed;
	public int secondsLeft;
	public int score = 0;
	public int highScore = 0;
	public bool paused = false;
	public float waiting = 0f;
	public bool levelComplete = false;
	public bool gameOver = false;

	public AudioClip won;
	public AudioClip lost;
	public AudioClip rotate;
	public AudioClip powerUp;
	public AudioClip powerDown;
	public AudioClip click;


	public AudioSource source;


	// Use this for initialization
	void Start () {

		gameOverPanel = GameObject.FindGameObjectWithTag ("GameOver");
		gameOverPanel.SetActive (false);

		score = PlayerPrefs.GetInt ("Score");
		highScore = PlayerPrefs.GetInt ("HighScore");

		currentLevel = PlayerPrefs.GetInt ("Level");
		baseSeed = PlayerPrefs.GetInt ("BaseSeed");

		tileGameObjectMap = new Dictionary<Tile, GameObject> ();
		createLevel ();

		//source = GetComponent<AudioSource> ();

	}


	public void completeLevel() {

		paused = true;
		levelComplete = true;
		canClick = false;

		PlayerPrefs.SetInt ("Level", currentLevel + 1);
		score = score + secondsLeft * 5 + (currentLevel * 5);

		PlayerPrefs.SetInt ("Score", score);

		if (score > highScore) {
			PlayerPrefs.SetInt ("HighScore", score);
		}

		waiting = 3f;

		source.PlayOneShot (won,1f);

	}



	void createLevel () {

		//currentLevel = 30;

		Random.InitState (currentLevel + baseSeed);


		if (currentLevel < 20) {
			
			size = Mathf.RoundToInt (3 + (currentLevel / 3));
		} else {

			size = Mathf.RoundToInt (10 + currentLevel/10);
		}

		Camera.main.transform.position = new Vector3 ((float)size - 2f, 13f, -8f);

		Text levelText = GameObject.FindGameObjectWithTag ("Level").GetComponent<Text>();
		levelText.text = "Level: " + currentLevel;

		Text scoreText = GameObject.FindGameObjectWithTag ("Score").GetComponent<Text> ();
		scoreText.text = "Score - " + score;

		Text bestText = GameObject.FindGameObjectWithTag ("High").GetComponent<Text> ();
		bestText.text = "High - " + highScore;


		createTiles ();
		resizePlane ();
		drawRoads ();
		createSourceNode ();





	}

	public void gameOverStuff () {

		waiting = 2f;
		gameOver = true;
		paused = true;
		gameOverPanel.SetActive (true);

		source.PlayOneShot (lost);



	}

	public void updateTime () {

		if (paused == false) {
			//Debug.Log ("time...");

			secondsLeft = secondsLeft - 1;

			Text levelText = GameObject.FindGameObjectWithTag ("Time").GetComponent<Text> ();
			levelText.text = "Time: " + secondsLeft;

			if (secondsLeft == 0 ) {

				gameOverStuff();

			}
		}


	}

	public void startClock () {
		InvokeRepeating ("updateTime", 1f, 1f);
	}

	void flashLights () {

		if (Random.Range (0, 2) == 0) {
			int choose = Random.Range (0, NodeController.Instance.nodes.Count);

			Node n = NodeController.Instance.nodes [choose];

			if (n.light.gameObject.activeInHierarchy) {
				n.light.gameObject.SetActive (false);
			} else {
				n.light.gameObject.SetActive (true);
			}

		}


	}

	void checkWaiting() {

		if (waiting > 0f) {
			waiting = waiting - Time.deltaTime;

			if (levelComplete) {
				flashLights ();
			}

			if (waiting <= 0f) {

				if (levelComplete) {
					SessionController.Instance.restartScene ();



				}
			}
		}

	}
	
	// Update is called once per frame
	void Update () {

		//bug.Log (canClick);
		checkWaiting ();

		if ( gameOver && waiting <= 0f && Input.GetMouseButton(0)) {

			source.PlayOneShot (click);
			PlayerPrefs.DeleteKey ("Level");
			SessionController.Instance.restartScene ();

		}

		if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && canClick == true && levelComplete == false) {

			//Debug.Log (canClick);

			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			float mult = 1f;

			if (Input.GetMouseButtonDown (1)) {
				mult = -1f;
			}

			if (Physics.Raycast (ray, out hit, 100)) {

				GameObject clicked = Helpers.FindParentWithTag (hit.transform.gameObject, "Clickable");

				Node n= null;

				if (clicked != null) {

					if (clicked.name == "House") {
						n = NodeController.Instance.houseNodeMap [clicked];
					}

					if (clicked.name == "Node") {
						n = NodeController.Instance.gameObjectNodeMap [clicked];
					}

					if (n != null) {

						source.PlayOneShot (click);
						//Debug.Log (n.rotation + " going to rotate " + mult);
						canClick = false;
						n.rotateNode (90f * mult);
						source.PlayOneShot (rotate);

					} 

				} 


			
			}



		}

	}


	void drawRoads() {



		for (int y= 0; y < size + 1; y++) {

			GameObject go = Instantiate (road);
			go.transform.position = new Vector3 (0.5f * (size*3f), 0.02f,  (y * 3f));

			go.transform.localScale = new Vector3 ((size) * 3f +1f, 1f, 1f);
			go.transform.eulerAngles = new Vector3 (90f, 0f, 0f);
			go.GetComponent<Renderer> ().material.mainTextureScale = new Vector2 (size * 3f, 1f);

			go.transform.SetParent (board.transform);
			//go.set
		}

		for (int x = 0; x < size + 1; x++) {

			GameObject go = Instantiate (road);
			go.transform.position = new Vector3 (-0f + (x * 3f), 0.01f, 0.5f * (size*3f));

			go.transform.localScale = new Vector3 ((size) * 3f, 1f, 1f);
			go.GetComponent<Renderer> ().material.mainTextureScale = new Vector2 (size * 3f, 1f);

			go.transform.SetParent (board.transform);
		}




	}

	void resizePlane() {

		//Transform board = GameObject.FindGameObjectWithTag ("Board").transform;

		board.transform.position = new Vector3 (size * 3f / 2f, 0f, size * 3f / 2f);

		Transform plane = GameObject.FindGameObjectWithTag ("Plane").transform;

		plane.localScale = new Vector3 ((float)size * 3f / 10f, 1f, (float)size * 3f / 10f);
		plane.position = new Vector3 (size / 2f * 3f, 0f, size / 2f * 3f);

		plane.transform.SetParent (board.transform);


	}

	void createSourceNode () {

		populating = true;
		int half = Mathf.RoundToInt (size / 2);

		Tile t = getTileAt (half, 0);

		string[] startNodes = { "T", "Corner", "Straight", "Y", "BirdFoot" };

		string startName = startNodes[Random.Range(0, 5)];

		NodeController.Instance.newNode (t, 0f);
	

		GameObject node = Instantiate(NodeController.Instance.nodePrefabs ["Straight"]);
		node.transform.position = t.getNodePosition ();
		node.transform.position = new Vector3 (node.transform.position.x, node.transform.position.y, node.transform.position.z - 3f);
		node.transform.SetParent (board.transform);
		//startNode.rotateNode (45f);

	}


	void createTiles () {

		tiles = new Tile[size, size]; 

		for (int x = 0; x < size; x++) {
			
			for (int y = 0; y < size; y++) {

				tiles [x, y] = new Tile (x, y);
				createTileGameObject (tiles [x, y]);
			}
			
		}
			
	}


	public Tile getTileAt(int x, int y) {

		if (x < 0 || x > size - 1 || y < 0 || y > size - 1) {
			return null;
		}

		return tiles [x, y];

	}

	void createTileGameObject (Tile tile) {

		GameObject go = Instantiate (tilePrefab);
		go.transform.position = tile.getWorldPosition ();

		tileGameObjectMap.Add (tile, go);
		go.transform.SetParent (GameObject.Find ("DebugTiles").transform);

		Text txt = go.transform.GetComponentInChildren<Text> ();

		txt.text = tile.x + "," + tile.y;

		go.SetActive (false);

	//

	}


}
