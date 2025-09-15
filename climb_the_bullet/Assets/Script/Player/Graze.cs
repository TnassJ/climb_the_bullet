using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graze : MonoBehaviour
{
    public AudioClip GrazeClip; // SE
    public static int GrazeCount = 0;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
//メモ：弾にもグレイズ用のcolliderつけて接触後消去するようにすれば一つの弾につき一回しかグレイズできなくなるが…
            GrazeCount ++; //残機減らす
            Debug.Log(GrazeCount);
            //グレイズ時SE再生
            var audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>();
            audioSource.PlayOneShot(GrazeClip, 0.1f);
            //オブジェクトにつけたパーティクルを再生
            GetComponent<ParticleSystem>().Play();
        }
    }
}
