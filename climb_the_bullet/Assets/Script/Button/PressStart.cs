using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Drop()
    {
        SceneManager.LoadScene("STAGE0Prolog");
        Debug.Log("Button Pressed");
    }
    public void OnClick()
    {
        SceneManager.LoadScene("STAGE0Prolog");
        Debug.Log("Button Clicked");
    }
    //public void StartZ()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        SceneManager.LoadScene("SampleScene 1");
    //        Debug.Log("Button Pressed");
    //    }
    //}
}
