using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
public class Button_LangChange : MonoBehaviour
{
    /* LangNum
       0 jp
       1 en
    */
    public static string LangCode = "jp";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Drop(string code)
    {
        LangCode = code;
        //Debug.Log("activeLanguage:" + LangCode);
        Localization.LangCode = LangCode;
    }
}
