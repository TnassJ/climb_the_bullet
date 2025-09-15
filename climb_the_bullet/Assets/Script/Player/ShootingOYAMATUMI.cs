using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingOYAMATUMI : MonoBehaviour
{
    //[SerializeField]
    //float PlayerSpeed = 0.1f;

    float timeCount = 0; // 経過時間
    float shotAngle = 90; // 発射角度

    public Shot_OYAMATUMI ShotPrefab_OYAMATUMI; // 弾のプレハブ
    public float ShotSpeed; // 弾の移動の速さ
    public float ShotAngleRange; // 複数の弾を発射する時の角度
    float ShotTimer; // 弾の発射タイミングを管理するタイマー
    public int ShotCount; // 弾の発射数
    public float ShotInterval; // 弾の発射間隔（秒

    public AudioClip ShotClip_OYAMATUMI; // 弾のSE
    AudioSource audioSource;
    // プレイヤーのインスタンスを管理する static 変数
    //public static ShootingShikigami m_instance;

    void Start()
    {
        audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>(); // SE読み込み
        //audioSource.volume = 0.1f;
    }

    void Update()
    {
        // 弾の発射タイミングを管理するタイマーを更新する
        ShotTimer += Time.deltaTime;

        // まだ弾の発射タイミングではない場合は、ここで処理を終える
        if (ShotTimer < ShotInterval) return;

        // 弾の発射タイミングを管理するタイマーをリセットする
        ShotTimer = 0;
        var angle = 90;

        // 弾を発射する
        ShootNWay(angle, ShotAngleRange, ShotSpeed, ShotCount);
        audioSource.PlayOneShot(ShotClip_OYAMATUMI, 0.05f); // SE再生
        var movex = Random.Range(-5.0f, 5.0f);
        var movey = Random.Range(-5.0f, 2.0f);
        this.transform.position = new Vector3(movex, movey, 0);
    }

    // 弾を発射する関数
    private void ShootNWay(float angleBase, float angleRange, float speed, int count)
    {
        //ShootingPlayerスクリプトの変数を呼び出すために必要
        //var PlayerObject = GameObject.Find("Player");
        //var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();

        //追記　ステートがダメージならリターン
        //if (shootingPlayerObject.state == STATE.DAMAGED) return;

        var pos = transform.position; // プレイヤーの位置
        var rot = transform.rotation; // プレイヤーの向き

        // 弾を複数発射する場合
        if (1 < count)
        {
            // 発射する回数分ループする
            for (int i = 0; i < count; ++i)
            {
                // 弾の発射角度を計算する
                var angle = angleBase +
                    angleRange * ((float)i / (count - 1) - 0.5f);

                // 発射する弾を生成する
                var shot = Instantiate(ShotPrefab_OYAMATUMI, pos, rot);

                // 弾を発射する方向と速さを設定する
                shot.Init_Shikigami(angle, speed);
            }
        }
        // 弾を 1 つだけ発射する場合
        else if (count == 1)
        {
            // 発射する弾を生成する
            var shot = Instantiate(ShotPrefab_OYAMATUMI, pos, rot);

            // 弾を発射する方向と速さを設定する
            shot.Init_Shikigami(angleBase, speed);
        }
    }

}
