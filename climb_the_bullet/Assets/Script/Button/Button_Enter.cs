using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button_Enter : MonoBehaviour
{
    Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter()
    {
        anim = GetComponent<Animation>();
        //anim.SetBool ( "Selected", false );
        TitleProcess.enterCheck = true;
    }
}
