using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Button_Retry : MonoBehaviour
{
    public void Retry()
    {   
        MenuPause.ResumeON = true;
        ShootingPlayer.PlayerExpSum = ShootingPlayer.PlayerExpSum_first;
        ShootingPlayer.PlayerHP = 0;
        ShootingPlayer.PlayerBomb = 0;
        SceneManager.LoadScene (SceneManager.GetActiveScene().name);
    }
	
}
