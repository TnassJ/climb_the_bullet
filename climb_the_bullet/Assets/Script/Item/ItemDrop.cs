using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    public float MagnetDistance = 2.0f; // アイテムを引きつける距離
    public int m_exp; // 取得できる経験値
    public float m_brake = 0.9f; // 散らばる時の減速量、数値が小さいほどすぐ減速する

    private Vector3 m_direction; // 散らばる時の進行方向
    private float m_speed; // 散らばる時の速さ

    // プレイヤーを追尾する時の加速度、数値が大きいほどすぐ加速する
    public float m_followAccel = 0.01f;

    private bool m_isFollow; // プレイヤーを追尾するモードに入った場合 true
    private float m_followSpeed = 0.1f; // プレイヤーを追尾する速さ

    public AudioClip ItemGetClip; // ショット時再生する SE

    // 毎フレーム呼び出される関数
    private void Update()
    {
                // プレイヤーの現在地を取得する
        var playerPos = ShootingPlayer.m_instance.transform.position;
        //Debug.Log( playerPos );

        // プレイヤーと宝石の距離を計算する
        var distance = Vector3.Distance( playerPos, transform.position );

        // プレイヤーと宝石の距離が近づいた場合
        if ( distance < MagnetDistance )
        {
            // プレイヤーを追尾するモードに入る
            m_isFollow = true;
        }

        // プレイヤーを追尾するモードに入っている場合かつ
        // プレイヤーがまだ死亡していない場合
        //if ( m_isFollow && ShootingPlayer.m_instance.gameObject.activeSelf )
        if ( m_isFollow )
        {
            // プレイヤーの現在位置へ向かうベクトルを作成する
            var direction = playerPos - transform.position;
            direction.Normalize();
            //Debug.Log( direction );

            // 宝石をプレイヤーが存在する方向に移動する
            transform.position += direction * m_followSpeed;

            // 加速しながら近づく
            //m_followSpeed += m_followAccel;
            return;
        }

        // 散らばる速度を計算する
        var velocity = m_direction * m_speed;

        // 散らばる
        transform.localPosition += velocity;

        // だんだん減速する
        m_speed *= m_brake;

        // 宝石が画面外に出ないように位置を制限する
        transform.localPosition = 
            Utils.ClampPosition( transform.localPosition );
        

    }

    // 宝石が出現する時に初期化する関数
    public void ItemInit( float speedMin, float speedMax )
    {
        // 宝石がどの方向に散らばるかランダムに決定する
        var angle = Random.Range( 0, 360 );

        // 進行方向をラジアン値に変換する
        var f = angle * Mathf.Deg2Rad;

        // 進行方向のベクトルを作成する
        m_direction = new Vector3( Mathf.Cos( f ), Mathf.Sin( f ), 0 );

        // 宝石の散らばる速さをランダムに決定する
        m_speed = Mathf.Lerp( speedMin, speedMax, Random.value );

        // 8 秒後に宝石を削除する
        Destroy( gameObject, 8 );
    }

    // 他のオブジェクトと衝突した時に呼び出される関数
    private void OnTriggerEnter2D( Collider2D collision )
    {
        // 衝突したオブジェクトがプレイヤーではない場合は無視する
        if ( !collision.gameObject.CompareTag("Player") ) return;

        // 宝石を削除する
        Destroy( gameObject );

        // SE を再生する
        var audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>();
        audioSource.PlayOneShot( ItemGetClip , 0.05f);
        
        // プレイヤーの経験値を増やす
        var player = collision.GetComponent<ShootingPlayer>();
        Debug.Log("tagName:" + collision.gameObject.name);
        player.AddExp( m_exp );
    }
}
