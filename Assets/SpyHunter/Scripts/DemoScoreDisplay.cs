using UnityEngine;
using System.Collections;

public class DemoScoreDisplay : MonoBehaviour {

	public GameObject player;
	CarMoveBasic playerScript;

	// Use this for initialization
	void Start () {
		playerScript = player.GetComponent<CarMoveBasic>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI()
	{
		float width = Screen.width;
		int score = playerScript.Score;

		GUIStyle style = new GUIStyle();
		style.fontSize = Screen.height/20;

		GUI.Box (new Rect (0,0,width/4,Screen.height/20), "Score: " + score, style);
	}
}
