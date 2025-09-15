using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Resume : MonoBehaviour
{
    public void Resume()
    {
        MenuPause.ResumeON = true;
    }
}

