using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrazeUIManager : MonoBehaviour
{
    public TextMeshProUGUI Text3_Graze;// スコアのテキスト

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Text3_Graze.text = Graze.GrazeCount.ToString("D1");
    }
}
