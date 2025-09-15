using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class IconUIManager1 : MonoBehaviour
{

    //[SerializeField] GameObject LifeIcon; // 残機アイコン
    [SerializeField] GameObject[] icon; 
    //[SerializeField] GameObject BombIcon; // ボムドロップ
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IconNumChange (int iconNum)
    {
        // プレイヤーのインスタンスを取得する
        //var player = ShootingPlayer.m_instance;
        //var PlayerLife = player.PlayerHP; //プレイヤー残機取得
        for (int i = 0; i < icon.Length; i++)
        {
            icon[i].SetActive(false);
        }

        for (int i = 0; i < iconNum; i++)
        {
            icon[i].SetActive(true);
        }
    }
}
