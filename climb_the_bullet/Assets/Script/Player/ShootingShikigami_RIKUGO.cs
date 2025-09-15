using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingShikigami_RIKUGO : MonoBehaviour
{

    public Shot_Shikigami ShotPrefab_Shikigami_RIKUGO; // 弾のプレハブ
    public float ShotSpeed; // 弾の移動の速さ
    public float ShotArrayDistance; // 複数の弾を発射する時の角度
    float ShotTimer; // 弾の発射タイミングを管理するタイマー
    public int ShotCount; // 弾の発射数
    public float ShotInterval; // 弾の発射間隔（秒
    private GameObject[] targets;

    [System.Serializable]
    class RIKUGOShotData
    {
        public string level; // 弾のレベル
        public float speed; // 弾の移動の速さ
        public float arrayDistance; // 複数の弾を発射する時の角度
        public int count; // 弾の発射数
        public float interval; // 弾の発射間隔（秒
    }

    [SerializeField]
    private List<RIKUGOShotData> shotData;

    public AudioClip ShotClip_RIKUGO; // 弾のSE
    AudioSource audioSource;

    //private List<Shot_Shikigami> ShikigamiBullet = new List<Shot_Shikigami>();



    // プレイヤーのインスタンスを管理する static 変数
    //public static ShootingShikigami m_instance;

    void Start()
    {
        //audioSource = FindObjectOfType<AudioSource>(); // SE読み込み
        audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>(); // SE読み込み
        //audioSource.volume = 0.04f;
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
        HomingNWay(ShotArrayDistance, ShotSpeed, ShotCount);

    }

    // ショット内容を外部から変更するための関数
    public void ParameterUpdate(int level)
    {
        ShotArrayDistance = shotData[level].arrayDistance;
        ShotSpeed = shotData[level].speed;
        ShotCount = shotData[level].count;
        ShotInterval = shotData[level].interval;

    }

    // 弾を発射する関数
    private void HomingNWay(float arrayDistance, float speed, int count)
    {

        //ShikigamiBullet.RemoveAll(s => s == null);
        //foreach (var shot in ShikigamiBullet)
        //{
        //ShootingPlayerスクリプトの変数を呼び出すために必要
        //var PlayerObject = GameObject.Find("Player");
        //var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();

        // 追記 ステートがダメージならリターン
        //if (shootingPlayerObject.state == STATE.DAMAGED) return;

        var pos = transform.position; // プレイヤーの位置
        var rot = transform.rotation; // プレイヤーの向き

        // 手前の敵を追尾、消滅したら別の敵を追尾したい
        // タグを使って画面上の全ての敵の情報を取得
        targets = GameObject.FindGameObjectsWithTag("Enemy");
        // 要素数が0（敵がいない）なら早期リターン
        if (targets.Length == 0) return; 
        // 自機から
        var shotAngle = GetAngle2(transform.position, targets[0].transform.position);

        // 弾を複数発射する場合
        if (1 < count)
        {
            // 発射する回数分ループする
            for (int i = 0; i < count; ++i)
            {
                // 縦のarrayDistanceの間隔でcount数だけホーミング弾発射
                var pos_i = new Vector3(pos.x, pos.y + arrayDistance * ((float)i / (count - 1) - 0.5f), pos.z);

                // 発射する弾を生成する
                var shot = Instantiate(ShotPrefab_Shikigami_RIKUGO, pos_i, rot);
                //Debug.Log("tuibi");

                // 弾を発射する方向と速さを設定する
                shot.Updatevelo(shotAngle, speed);
            }
        }
        // 弾を 1 つだけ発射する場合
        else if (count == 1)
        {

            // 発射する弾を生成する
            var shot = Instantiate(ShotPrefab_Shikigami_RIKUGO, pos, rot);
            //Debug.Log("tuibi");

            // 弾を発射する方向と速さを設定する
            shot.Updatevelo(shotAngle, speed);

        }
        audioSource.PlayOneShot(ShotClip_RIKUGO, 0.03f); // SE再生
        //}
    }

    public float GetAngle2(Vector2 start, Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

}
