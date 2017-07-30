using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class SessionController : Singleton<SessionController> {
	
	public AudioClip click;

	public int currentLevel = 1;
	public bool firstGame = true;

	// Use this for initialization
	void Awake () {

	
		DontDestroyOnLoad(transform.gameObject);

		if (FindObjectsOfType (GetType()).Length > 1) {
			Destroy (gameObject);
		}

		if (PlayerPrefs.HasKey("HighScore") == false) {

			PlayerPrefs.SetInt ("HighScore", 0);
		}

		if (PlayerPrefs.HasKey ("Level") == false) {

			System.DateTime start = new System.DateTime (2017, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			int use_time = (int)(System.DateTime.UtcNow - start).TotalSeconds;

			PlayerPrefs.SetInt ("Level", 1);
			PlayerPrefs.SetInt ("Score", 0);
			UnityEngine.Random.InitState (use_time);
			PlayerPrefs.SetInt("BaseSeed", use_time);
		}

	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKey ("escape")) {
			Application.Quit ();
		}
	}


	public void restartScene() {


		SceneManager.LoadScene ("Power");

	}


}
