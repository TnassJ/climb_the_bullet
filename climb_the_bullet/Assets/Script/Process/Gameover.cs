using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//namespace Gameover
//{
    public class Gameover : MonoBehaviour
    {
        bool PauseON = false;
        public GameObject gameoverMenu;
        public GameObject PlayerObject;
        public GameObject DarkerObject; //暗転用オブジェクト

        public bool GameoverJudge = false;

        // Start is called before the first frame update
        void Start()
        {
            gameoverMenu = GameObject.Find("GameoverMenuUI");
            gameoverMenu.SetActive(false);
            PlayerObject = GameObject.Find("Player");
        }

        // Update is called once per frame
        void Update()
        {
            if (GameoverJudge == true)
            {
                if (!PauseON)
                {
                    GameoverPause();
                    PauseON = true;
                }
            }

        }

        public void GameoverPause()
        {
            //EnemyMove.EnemyPouse = true;
            //TargetPauser.Pause();

            //var pauseMenu = GameObject.Find("PauseMenuUI");
            StartCoroutine("GameoverWait");
            

        }

        public void CallGameover(bool gameover)
        {
            GameoverJudge = gameover;
        }

        IEnumerator GameoverWait()
        {
            DarkerObject.SetActive(true);
            yield return new WaitForSeconds(1.0f);
            gameoverMenu.SetActive(true);
            Time.timeScale = 0;

        }

    }
//}
