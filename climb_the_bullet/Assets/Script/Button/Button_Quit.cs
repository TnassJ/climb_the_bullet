using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Quit : MonoBehaviour
{
    public void Quit()
    {
        MenuPause.ResumeON = true;
        ShootingPlayer.PlayerHP = 0;
        ShootingPlayer.PlayerBomb = 0;
        SceneManager.LoadScene("TitleScene");
    }
}
