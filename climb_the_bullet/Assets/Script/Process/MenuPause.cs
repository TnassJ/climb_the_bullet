using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuPause : MonoBehaviour
{
    public static bool ResumeON = false;
    bool PauseON = false;
    public GameObject pauseMenu;

    private GameInputs gameInputs;

    // Start is called before the first frame update
    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenuUI");
        pauseMenu.SetActive(false);

        gameInputs = new GameInputs();
        // ポーズ操作の登録、有効化
        gameInputs.Player.Pause.started += PauseAction;
        gameInputs.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // 再開ボタン押される待ち（button_Resume.csでResumeON = trueを行う）
        if (ResumeON)
        {
            ResumeGame();
            ResumeON = false;
            PauseON = false;
        }

    }

    private void OnDestroy()
    {
        gameInputs?.Dispose();
    }

    private void PauseAction(InputAction.CallbackContext context)
    {
        if (!PauseON)
        {
            PauseGame();
            PauseON = true;
        }
        else if (PauseON)
        {
            ResumeGame();
            PauseON = false;
        }
    }

    public void PauseGame()
    {
        //EnemyMove.EnemyPouse = true;
        //TargetPauser.Pause();
        
        //var pauseMenu = GameObject.Find("PauseMenuUI");
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        
    }

    public void ResumeGame()
    {
        //var pauseMenu = GameObject.Find("PauseMenuUI");
        //TargetPauser.Resume();
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
        //EnemyMove.EnemyPouse = false;
    }
}
