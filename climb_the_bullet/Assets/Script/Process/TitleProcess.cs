using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleProcess : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject titleEffect; // 道中2
    public GameObject FogEffect; // 道中2
    public GameObject enterButton; // 道中2
    public static bool enterCheck;
    public GameObject menuButtons; // 道中2

    void Start()
    {
        StartCoroutine("OPWait");
    }

    // Update is called once per frame
    void Update()
    {
        if (enterCheck)
        {
            StartCoroutine("MenuWait");
            enterCheck = false;
        }
        
    }
    IEnumerator OPWait()
    {
        //FogEffect.SetActive(true);
        GameObject effectObj = Instantiate (FogEffect, new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);
        Destroy(effectObj, 4);

        yield return new WaitForSeconds(2.0f);
        titleEffect.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        enterButton.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
    }

    IEnumerator MenuWait()
    {
        yield return new WaitForSeconds(1.0f);
        enterButton.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        menuButtons.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        
    }
}
