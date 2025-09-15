using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ボム時に発射する弾を制御するコンポーネント（ボム弾のプレハブにアタッチ）
public class Shot_Bunshin : MonoBehaviour
{
    float angle_Bunshin; // 角度
    float speed_Bunshin; // 速さ
    private Vector3 m_velocity; // 速度ベクトル
    public AudioClip PlayerBulletClip; // ショット時再生する SE
    public bool BunshinReflection = false; // 弾を反射させるかどうか

    private void Start()
    {

    }

    // 毎フレーム呼び出される関数
    private void Update()
    {
        //float xp = this.transform.position.x;
        //float yp = this.transform.position.y;



        // 移動する
        transform.localPosition += m_velocity * Time.deltaTime;
    }


    // 弾を発射する時に初期化するための関数
    public void Init_Bomb(float angle, float speed)
    {
        angle_Bunshin = angle;
        speed_Bunshin = speed;
        // 弾の発射角度をベクトルに変換する
        var direction = Utils.GetDirection(angle_Bunshin);
        // 発射角度と速さから速度を求める
        m_velocity = direction * speed_Bunshin;
        // 4 秒後に削除する
        Destroy(gameObject, 4);
    }

    // 壁にぶつかったら反射
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Border")) && (BunshinReflection))
        {
            //Debug.Log("ReflectionBorder");
            // m_velocityの単位ベクトル取得
            var distance = m_velocity.magnitude;
            var direction = m_velocity / distance;
            var RayDistance = 5;
            Ray ray = new Ray(transform.position, direction * RayDistance);
            //Ray可視化
            //Debug.DrawRay(ray.origin, ray.direction * RayDistance, Color.red, Time.deltaTime);
            // Raycast対象レイヤー(RaycastSubjectレイヤー)にのみ反応するようにする
            int layerMask = 1 <<  LayerMask.NameToLayer ("RaycastSubject");
            // RayCastして衝突相手の垂線ベクトルを取得normalVec
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, RayDistance, layerMask);
            var normalVec = hit.normal;
            //Debug.Log(normal);
            //Debug.Log("Hit object: " + hit.collider.gameObject.name);
            // 反射後のベクトル算出
            Vector3 resultVel = Vector3.Reflect(m_velocity, normalVec);
            // 進行方向にオブジェクトを回転
            //this.transform.rotation = Quaternion.FromToRotation (Vector3.up, resultVel);
            // m_velocityの更新
            m_velocity = resultVel;
        }
    }
}
