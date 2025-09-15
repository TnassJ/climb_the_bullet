using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 式神オプションが発射する弾を制御するコンポーネント
public class Shot_Shikigami : MonoBehaviour
{
    private Vector3 m_velocity; // 速度
    private Vector3 normal; // 反射壁の法線ベクトル
    private Rigidbody rb; // 弾のRigidbody
    public int shotDamage = 1; // 弾のダメージ
    public bool ShotReflection = false; // 弾を反射させるかどうか
    public bool ShotPiercing = false; // 弾を貫通させるかどうか

    // ☆追加　エネミーのインスタンスを管理する static 変数
    public static Shot_Shikigami m_instance;

    //public AudioClip PlayerBulletClip; // ショット時再生する SE

    void start()
    {
        m_instance = this;
        rb = GetComponent<Rigidbody>();
    }

    // 毎フレーム呼び出される関数
    private void Update()
    {
        //m_velocity = rb.velocity;
        // 移動する
        transform.localPosition += m_velocity * Time.deltaTime;
        // SE を再生する
        //var audioSource = FindObjectOfType<AudioSource>();
        //audioSource.PlayOneShot( PlayerBulletClip );
        DeleteBullet_Shikigami();
    }

    // 弾を発射する時に初期化するための関数
    public void Init_Shikigami(float angle, float speed)
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

    public void Updatevelo(float angle, float speed)
    {
        // 弾の発射角度をベクトルに変換する
        var direction = Utils.GetDirection(angle);

        // 発射角度と速さから速度を求める
        m_velocity = direction * speed;

    }

    public void DeleteBullet_Shikigami()
    {
        float xp = this.transform.position.x;
        float yp = this.transform.position.y;
        if (xp < -6.0f | xp > 6.0f | yp < -6.0f | yp > 6.0f)
        {
            Destroy(gameObject);
        }

    }

    // 壁にぶつかったら反射
    void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.gameObject.CompareTag("Border")) && (ShotReflection))
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
            this.transform.rotation = Quaternion.FromToRotation (Vector3.up, resultVel);
            // m_velocityの更新
            m_velocity = resultVel;
        }
        
    }

}
