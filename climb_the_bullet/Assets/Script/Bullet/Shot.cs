using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// プレイヤーが発射する弾を制御するコンポーネント
public class Shot : MonoBehaviour
{
    private Vector3 m_velocity; // 速度
    public AudioClip PlayerBulletClip; // ショット時再生する SE
    public int shotDamage = 1;

    // 毎フレーム呼び出される関数
    private void Update()
    {
        // 移動する
        transform.localPosition += m_velocity  * Time.deltaTime;
        // SE を再生する
        //var audioSource = FindObjectOfType<AudioSource>();
        //audioSource.PlayOneShot( PlayerBulletClip );
        DeleteBullet();
    }

    // 弾を発射する時に初期化するための関数
    public void Init(float angle, float speed)
    {
        // 弾の発射角度をベクトルに変換する
        var direction = Utils.GetDirection(angle);

        // 発射角度と速さから速度を求める
        m_velocity = direction * speed;

        // 弾が進行方向を向くようにする
        var angles = transform.localEulerAngles;
        angles.z = angle - 90;
        transform.localEulerAngles = angles;


        // 2 秒後に削除する
        //Destroy( gameObject, 2 );

    }

    public void DeleteBullet()
    {
        float xp = this.transform.position.x;
        float yp = this.transform.position.y;
        if (xp < -6.0f | xp > 6.0f | yp < -6.0f | yp > 6.0f)
        {
            Destroy(gameObject);
        }

    }
}
