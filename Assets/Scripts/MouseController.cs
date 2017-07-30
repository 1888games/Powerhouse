using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

	float turnspeed = 60f;
	float moveSpeed = 10f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		checkKeyboard ();
	}


	void checkKeyboard () {

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Transform t = GameController.Instance.board.transform;

			t.eulerAngles = new Vector3 (t.eulerAngles.x, t.eulerAngles.y - (turnspeed * Time.deltaTime), t.eulerAngles.z);



		}

		if (Input.GetKey (KeyCode.RightArrow)) {

			Transform t = GameController.Instance.board.transform;

			t.eulerAngles = new Vector3 (t.eulerAngles.x, t.eulerAngles.y + (turnspeed * Time.deltaTime), t.eulerAngles.z);



		}

		if (Input.GetKey (KeyCode.UpArrow)) {

			Transform t = GameController.Instance.board.transform;

			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z + moveSpeed * Time.deltaTime);




		}


		if (Input.GetKey (KeyCode.DownArrow)) {

			Transform t = GameController.Instance.board.transform;

			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - moveSpeed * Time.deltaTime);




		}


	}

	public void onRestartClick () {

		GameController.Instance.source.PlayOneShot (GameController.Instance.click);
		PlayerPrefs.DeleteKey ("Level");
		Debug.Log ("Clicked button.,..");
		SessionController.Instance.restartScene ();



	}


	public void onQuitClick () {

		GameController.Instance.source.PlayOneShot (GameController.Instance.click);
		Application.Quit ();
		PlayerPrefs.DeleteKey ("FirstGame");


	}
}
