using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SpyHunter.Game
{
    public class GameManager : MonoBehaviour
    {
        public KeyCode restartInput = KeyCode.R;

        int score = 0;
        int scoreKills = 0;

        public int Score { get { return score + scoreKills; } }

        public static GameManager instance;

        GUIStyle simpleScoreStyle;

        private void Awake()
        {
            instance = this; 

            simpleScoreStyle = new GUIStyle();
            simpleScoreStyle.fontSize = Screen.height / 20;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            RestartScene();
        }

        void RestartScene()
        {
            if (Input.GetKeyDown(restartInput) 
                //|| phoneScript.TapRestart(alive)
                )
            {
                //Application.LoadLevel ("Scene1");
                //Application.LoadLevel(Application.loadedLevelName);
                SceneManager.LoadScene(Application.loadedLevelName);
            }
        }

        public void AddToScore(int value)
        {
            scoreKills += value;
        }

        void OnGUI()
        {
            GUI.Box(
                new Rect(
                    0, 
                    0,
                    Screen.width / 4, 
                    Screen.height / 20), 
                "Score: " + Score, 
                simpleScoreStyle);
        }
    }
}