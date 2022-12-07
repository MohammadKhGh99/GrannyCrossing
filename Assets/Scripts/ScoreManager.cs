// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
//
// public class ScoreManager : MonoBehaviour
// {
//        [SerializeField] private Text blueLivesText;
//        [SerializeField] private Text redLivesText;
//        // [SerializeField] private Text highScoreText;
//    
//        private const string InitialText = "Lives: ";
//
//        public static int Blue = 1;
//        public static int Red = 1;
//        // private const string InitialHighText = "HIGHSCORE: ";
//        public static ScoreManager Instance;
//    
//        private void Awake()
//        {
//            Instance = this;
//        }
//    
//        // Start is called before the first frame update
//        void Start()
//        {
//            blueLivesText.text = InitialText + "10";
//            redLivesText.text = InitialText + "10";
//            // scoreText.text = InitialText + GameManager.Score;
//            // highScoreText.text = InitialHighText + PlayerPrefs.GetInt("HighScore", 0);
//        }
//    
//        // Update is called once per frame
//        public void UpdateScore(int lives)
//        {
//            // GameManager.Score += points;
//            blueLivesText.text = InitialText + GameManager.Score;
//            curScoreText.color = points < 0 ? Color.red : Color.green;
//            curScoreText.text = points < 0 ? points.ToString() : "+" + points;
//            if (GameManager.Score > PlayerPrefs.GetInt("HighScore", 0))
//            {
//                PlayerPrefs.SetInt("HighScore", GameManager.Score);    
//            }
//        }
//    
//        public void ActiveCurScore(bool activeOrNot)
//        {
//            curScoreText.gameObject.SetActive(activeOrNot);
//        }
// }
