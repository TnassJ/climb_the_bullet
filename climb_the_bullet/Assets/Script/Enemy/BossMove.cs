using System.Collections;
using System.Collections.Generic;
using App.BaseSystem.DataStores.ScriptableObjects.Status;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public enum STATE_Boss
{
    NOMAL,
    //DAMAGED,
    MUTEKI
    //NEUTRAL
}

public class BossMove : MonoBehaviour
{
    //public string BossName;
    //private StatusDataStore statusDataStore;
    //private ShootingPlayer PlayerObject = GameObject.Find("Player").GetComponent<ShootingPlayer>();
    public float BossSpeed = 0.1f;// 敵の移動スピード
    [SerializeField] GameObject BossHPBarPrefab;
    //[SerializeField] Transform parent;
    HPBarManager BossHPBarManager;
    //☆ダメージ時点滅のための諸々
    float BlinkerTime;
    SpriteRendererBlinker spriteRendererBlinker;

    float timeCount = 0; // 経過時間
    float shotAngle = 0; // 発射角度
    float shotSpeed = 0; // 発射速度
    int shotNum = 10; // 発射数
    float shotInterval = 0.1f; // 発射間隔
    int j = 0;//ループのためのカウント


    float timeCountB = 0; // 経過時間
    float shotAngleB = 0; // 発射角度
    float shotSpeedB = 0; // 発射速度
    int shotNumB = 10; // 発射数
    float shotIntervalB = 0.1f; // 発射間隔

    Bullet BossBullet; // 弾のプレハブ
    Bullet BossBulletB; // 弾のプレハブ

    Familiar Familiar;//☆202410追加　使い魔のプレハブ
    //☆202410追加　sunset3用の変数
    float TimePoint1Shotage = 2.5f;
    float TimePoint1 = 0.9f;
    float TimePoint1Min = 1.2f;
    float MagnificationRateShotage = 1.025f;
    int k = 0; //一富士二鷹三茄子用]

    //☆202410追加　カットイン用プレハブ
    public GameObject Cutin_tenkoPrefab;
    bool CutinCalledOnce = false;
    float DisplayTime = 2.5f;
    float timeCount_Cutin = 0;
    float CutinTime = 2.0f;
    public STATE_Boss state; // 無敵かどうか
    float timeCountC = 0; // 経過時間
    bool ShotCalledOnce = false;
    public GameObject IllusionPrefab;


    [SerializeField]
    private List<BossBulletTable> bossBulletTable = new List<BossBulletTable>();
    //☆
    SpriteRenderer BackgroundSpriteRenderer;
    public GameObject BackgroundImage_prefab;

    // Nwaveの弾幕パラメータを設定
    [System.Serializable]
    class BossBulletTable
    {
        [System.NonSerialized]
        public float BulleTimeCount = 0; // 経過時間
        public float BulleTimeCountB = 0; // 経過時間

        [System.NonSerialized]
        public float BulleAngle = 0; // 発射角度
        public float BulleSpeed = 0; // 発射速度
        public int BulleNum = 10; // 発射数
        public float BulleInterval = 1.0f; // 発射間隔
        public Bullet BullePrefab;

        public float BulleAngleB = 0; // 発射角度
        public float BulleSpeedB = 0; // 発射速度
        public int BulleNumB = 10; // 発射数
        public float BulleIntervalB = 1.0f; // 発射間隔
        public Bullet BullePrefabB;

        public Familiar FamiliarPrefab;

        public BossBulletPattern bulletPattern;
        public Sprite BackgroundImage;

    }

    RaycastHit hit;

    public enum BossBulletPattern
    {
        Vortex,
        ToMe,
        nDirection,
        nDirection2,
        Baramaki,
        Sankaku,
        Sample2,
        Pondering,
        NCurcular,
        NSpiral,
        yuki_no_ootani,
        Kar,
        amidanyorai,
        zigokudani,
        nanatu_no_ike,
        hanabatake,
        Nishikigataki,
        dyamond,
        jukai,
        Daifunka,
        nitaka,
        Sunset1,
        Sunset2,
        Sunset3,
        Sample3,
        nDirectionAB,

	}



    BossBulletPattern bossbulletpattern;
    int wave_i = 0; // i番目の弾パターン

    public ExtinguishEnemy ExtinguishPrefab; // 爆発エフェクトのプレハブ
    float GemSpeedMin = 0; // 生成する宝石の移動の速さ（最小値）
    float GemSpeedMax = 0.1f; // 生成する宝石の移動の速さ（最大値）

    public int BossHPMax = 100; // HP の最大値
    private int BossHP; // HP

    public Movement movement = Movement.StraightMove;
    public Vector3 m_direction = new Vector3(-1, -1, 0); // 進行方向 

    public float StraightMoveTime = 5;
    private float StraightTimeCount = 0;
    private float StraightOverTime = 100;

    // ☆追加　エネミーのインスタンスを管理する static 変数
    public static BossMove m_instance;

    private List<Bullet> laserBullet = new List<Bullet>();
    public static bool BossPouse = false;
    public bool BossDead = false;

    //public AudioClip BulletClip; // 敵を倒した時に再生する SE
    public AudioClip bossBulletClip; // 弾のSE
    public AudioClip bossFamiliarClip; // 使い魔のSE
    public AudioClip bossCutinClip; // カットインのSE
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        //statusDataStore = FindObjectOfType<StatusDataStore>();
        //var BossData = statusDataStore.FindWithName(BossName); // 名前がBossNameであるデータを取得

        BossHP = BossHPMax; //HPの初期化
        //Debug.Log(movement);
        var parent = this.transform;
        //GameObject HPBar = Instantiate(HPBarPrefab, parent) as GameObject;
        GameObject HPBar = Instantiate(BossHPBarPrefab, this.transform.position, Quaternion.identity, parent);

        BossHPBarManager = this.GetComponentInChildren<HPBarManager>();

        spriteRendererBlinker = GetComponent<SpriteRendererBlinker>();

        MoveSwitchCase(movement);
        m_instance = this;

        audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>(); // SE読み込み
        //audioSource.volume = 0.5f;

        GameObject BackgroundImage1 = Instantiate(BackgroundImage_prefab) as GameObject; //カットインオブジェクトの出現
        BackgroundSpriteRenderer = BackgroundImage1.GetComponent<SpriteRenderer>();

    }

    // 敵が出現する時に初期化する関数
    public void BossInit(Movement BossMovement, Vector3 MoveDirection)
    {
        movement = BossMovement;
        m_direction = MoveDirection;
    }

    // Update is called once per frame
    void Update()
    {
        //一時停止するためのコード
        // var processManager = GameObject.Find("processManager").GetComponent<ProcessManager>();

        //☆追加1007　攻撃を受けてから一定時間点滅
        if (spriteRendererBlinker._isBlinking == true)
        {
            //Debug.Log("BlinkerTime加算開始");

            BlinkerTime += Time.deltaTime;
            if (BlinkerTime >= 1.0f)
            {
                spriteRendererBlinker.EndBlink();
                BlinkerTime = 0;
                //Debug.Log("EndBlink実行");

            }
        }

        if (BossPouse)
        {
            Debug.Log("BossPouse中");
            transform.DOPause();
            //GetComponent<BossMove>().DOPause();
            return;
        }
        //Debug.Log("BossPouse外");
        //GetComponent<BossMove>().DOPlay();
        transform.DOPlay();

        // wave数
        var waveNum = bossBulletTable.Count;

        var diffWave = waveNum - wave_i; // 計算省略のため

        // i waveのパラメータ代入
        if (BossHP > (diffWave - 1) * BossHPMax / waveNum & BossHP <= diffWave * BossHPMax / waveNum)
        {
            ChangeBarrage();
            //カットイン時、弾幕を打たない
            if (timeCount_Cutin < CutinTime) return;
            //カットイン終了後はダメージが入る
            state = STATE_Boss.NOMAL;
            NwaveBulletParam(wave_i);
        }
        // 2waveのパラメータ代入
        else
        {
            CutinCalledOnce = !CutinCalledOnce; //カットイン用の変数を反転させることで一度だけ実行
            wave_i ++;
        }
        //DeleteBoss(); // 範囲外の敵消去
    }

    void ChangeBarrage()
    {
            timeCount_Cutin += Time.deltaTime;

            //弾幕切り替え時、一度だけカットインさせる
            if (!CutinCalledOnce)
            {
                GameObject Cutin = Instantiate(Cutin_tenkoPrefab) as GameObject; //カットインオブジェクトの出現
                audioSource.PlayOneShot(bossCutinClip, 0.2f); // SE再生
                CutinCalledOnce = !CutinCalledOnce; //カットイン用の変数を反転させることで一度だけ実行

                //EnemyBulletタグのオブジェクトをすべて消去
                GameObject[] objects = GameObject.FindGameObjectsWithTag("EnemyBullet");
                foreach (GameObject bullets in objects)
                {
                    Destroy(bullets);
                }
                //カットイン時にtimecountを初期化
                timeCount_Cutin = 0;
                //カットイン中は無敵
                state = STATE_Boss.MUTEKI;
            }
    }
    void NwaveBulletParam(int i)
    {
        timeCount = bossBulletTable[i].BulleTimeCount;
        shotAngle = bossBulletTable[i].BulleAngle;
        shotSpeed = bossBulletTable[i].BulleSpeed;
        shotNum = bossBulletTable[i].BulleNum;
        shotInterval = bossBulletTable[i].BulleInterval;
        BossBullet = bossBulletTable[i].BullePrefab;

        timeCountB = bossBulletTable[i].BulleTimeCountB;
        shotAngleB = bossBulletTable[i].BulleAngleB;
        shotSpeedB = bossBulletTable[i].BulleSpeedB;
        shotNumB = bossBulletTable[i].BulleNumB;
        shotIntervalB = bossBulletTable[i].BulleIntervalB;
        BossBulletB = bossBulletTable[i].BullePrefabB;

        Familiar = bossBulletTable[i].FamiliarPrefab;

        bossbulletpattern = bossBulletTable[i].bulletPattern;
        bulletSwitchCase(bossbulletpattern);

        BackgroundSpriteRenderer.sprite = bossBulletTable[i].BackgroundImage;

        bossBulletTable[i].BulleTimeCount = timeCount;
        bossBulletTable[i].BulleAngle = shotAngle;
        bossBulletTable[i].BulleTimeCountB = timeCountB;
        bossBulletTable[i].BulleAngleB = shotAngleB;
    }

    void bulletSwitchCase(BossBulletPattern pattern)
    {
        //BossMoveLeft(); // 左移動
        switch (pattern)
        {
            case (BossBulletPattern.Vortex):
                VortexBullet();
                break;

            case (BossBulletPattern.ToMe):
                ToMeBullet();
                break;

            case (BossBulletPattern.nDirection):
                nDirectionBullet();
                break;

            case (BossBulletPattern.nDirection2):
                nDirectionBullet2();
                break;

            case (BossBulletPattern.Baramaki):
                BaramakiBullet();
                break;

            case (BossBulletPattern.Sankaku):
                SankakuBullet();
                break;

            case (BossBulletPattern.NCurcular):
                NCurcularBullet();
                break;

            case (BossBulletPattern.NSpiral):
                NSpiralBullet();
                break;

            case (BossBulletPattern.Sample2):
                LaserBullet();
                LaserCast();
                break;

            case (BossBulletPattern.Pondering):
                PonderingBullet();
                break;

            case (BossBulletPattern.yuki_no_ootani):
                yuki_no_ootani();
                break;

            case (BossBulletPattern.amidanyorai):
                amidanyorai();
                break;

            case (BossBulletPattern.zigokudani):
                zigokudani();
                break;

            case (BossBulletPattern.Kar):
                Kar();
                break;

            case (BossBulletPattern.nanatu_no_ike):
                nanatu_no_ike();
                break;

            case (BossBulletPattern.hanabatake):
                hanabatake();
                break;

            case (BossBulletPattern.Nishikigataki):
                Nishikigataki();
                break;

            case (BossBulletPattern.dyamond):
                dyamond();
                break;

            case (BossBulletPattern.jukai):
                jukai();
                break;

            case (BossBulletPattern.Daifunka):
                Daifunka();
                break;

            case (BossBulletPattern.nitaka):
                nitaka();
                break;

            case (BossBulletPattern.Sunset1):
                Sunset1();
                break;

            case (BossBulletPattern.Sunset2):
                Sunset2();
                break;

            case (BossBulletPattern.Sunset3):
                Sunset3();
                break;

            case (BossBulletPattern.Sample3):
                SampleBullet3();
                break;

            case (BossBulletPattern.nDirectionAB):
                nDirectionBulletAB();
                break;
        }
    }

    // 渦状の弾幕
    void VortexBullet()
    {

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        shotAngle -= 10; // 発射角度を10度ずらす
        // インスタンス生成
        // 発射する弾を生成する
        var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, 3);
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生
    }

    // 自機狙い
    void ToMeBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか 数字が小さいほど連射
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        // インスタンス生成
        // 発射する弾を生成する Instantiate( 生成するオブジェクト,  場所, 回転 );
        var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, 5);
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }

    void nDirectionBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle = 90 + i * 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);
        }
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }

    void nDirectionBulletAB()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        timeCountB += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle = 90 + i * 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);
        }

        // 1秒を超えているか
        if (timeCountB <= shotIntervalB) return;

        timeCountB = 0; // 再発射のために時間をリセット

        int nB = shotNumB; // nway弾のnを指定

        // 発射する回数分ループする
        for (int i = 0; i < nB; i++)
        {
            // 真下90度が基点
            shotAngleB = 90 + i * 360 / nB;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBulletB, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngleB, 4);
        }
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }


    void nDirectionBullet2()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定

        var shotAngle1 = shotAngle; // 左右で弾幕展開 shotAngle1が左側の角度

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle += 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);
        }

        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle1 -= 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle1, 4);
        }
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }

    //重力に従う弾を真上に放出
    void BaramakiBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // 発射弾数

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真上が基点
            float rnd = Random.Range(-9.9f, 9.9f);
            shotAngle = 270 + rnd;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 5);
        }
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }

    //三角形に放出
    void SankakuBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定
        shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        shotSpeed = 6.0f; // Speedは6で固定しておく。インスペクターから設定不可

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点


            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);

            shotSpeed = shotSpeed * 0.9f;
            shotAngle += 2;
        }

        shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        shotSpeed = 6.0f;

        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点


            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);

            shotSpeed = shotSpeed * 0.9f;
            shotAngle -= 2;
        }
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生
    }

    // 円運動する弾を配置する
    void NCurcularBullet()
    {

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // １回きりの出現
        if (timeCount > Time.deltaTime) return;

        //timeCount = 0; // 再発射のために時間をリセット
        var radius = 0.5f; // 回転半径。ループで0.5ずつ増加させる
        var angleVel = 50.0f;

        for (int i = 0; i < shotNum; i++)
        {
            // インスタンス生成
            // 発射する弾を生成する
            float initX = this.transform.position.x + radius * Mathf.Cos(shotAngle * Mathf.Deg2Rad);
            float initY = this.transform.position.y + radius * Mathf.Sin(shotAngle * Mathf.Deg2Rad);
            Vector3 initPosition = new Vector3(initX, initY, 0);
            var shot = Instantiate(BossBullet, initPosition, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.circularInit(radius, angleVel, this.transform.position);
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生
            radius += 0.5f;
            shotAngle += 30; // 発射角度を10度ずらす
        }

    }

    // 螺旋運動する弾を配置する
    void NSpiralBullet()
    {

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // １回きりの出現
        if (timeCount > Time.deltaTime) return;

        //timeCount = 0; // 再発射のために時間をリセット
        var radius = 0.2f; // 回転半径。ループで0.5ずつ増加させる
        var angleVel = 150.0f;

        for (int i = 0; i < shotNum; i++)
        {
            // インスタンス生成
            // 発射する弾を生成する
            float initX = this.transform.position.x + radius * Mathf.Cos(shotAngle * Mathf.Deg2Rad);
            float initY = this.transform.position.y + radius * Mathf.Sin(shotAngle * Mathf.Deg2Rad);
            Vector3 initPosition = new Vector3(initX, initY, 0);
            var shot = Instantiate(BossBullet, initPosition, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.spiralInit(shotSpeed, radius, angleVel, this.transform.position);
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生
            radius += 0.2f;
            shotAngle += 30; // 発射角度を10度ずらす
        }

    }

    //レーザー。現状動かない敵にしか実装できない（敵がうごくとあたり判定ずれる）
    void LaserBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= 0.5f) return;

        timeCount = 0; // 再発射のために時間をリセット

        int n = 6; // nway弾のnを指定
        var laserSpeed = 4.0f; // レーザー速度は4.0fで固定、インスペクターから変更不可

        // 真下90度が基点
        shotAngle += 360 / n;

        // 発射する弾を生成する
        var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, laserSpeed);
        laserBullet.Add(shot);

    }

    //テスト（レーザー）
    void LaserCast()
    {
        laserBullet.RemoveAll(s => s == null);
        foreach (var shot in laserBullet)
        {
            //Debug.Log(shot);

            //以下レーザー部分
            //まず架空の弾から敵機へのベクトルを求める
            var heading = shot.transform.position - this.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;

            //レーザーの長さ微調整用変数
            var laserWidthMargin = 0.0f;
            // レーザーの判定開始地点
            var laserStartPosition = this.transform.position + direction * (distance / 4 + laserWidthMargin); //
            // レーザーの判定距離
            var laserDistance = distance / 2 - laserWidthMargin;
            Ray ray = new Ray(laserStartPosition, direction * laserDistance); // レーザー生成
            //Ray可視化　開始地点、方向と長さ、可視化した時の色、表示時間
            Debug.DrawRay(ray.origin, ray.direction * laserDistance, Color.red, Time.deltaTime);
            // 衝突判定　どのRayについて判定するか、RaicastHitに格納されている衝突したオブジェクトの情報を得る、Rayの長さ、Rayが衝突するレイヤー
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, laserDistance);
            if (hit.collider)
            {
                // 当たったオブジェクトをログに出力する
                //Debug.Log("Hit object: " + hit.collider.gameObject.name);
                // プレイヤーに当たったら被弾させる
                if (hit.collider.gameObject.tag == "Player")
                {
                    Debug.Log("Damage");
                    var PlayerObject = GameObject.Find("Player");
                    var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();
                    shootingPlayerObject.Damage(1); //残機減らす
                    Destroy(shot.gameObject);

                }
            }
        }

    }

    // ポンデリング
    void PonderingBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        // 1秒を超えているか 数字が小さいほど連射
        if (timeCount <= shotInterval) return;
        timeCount = 0; // 再発射のために時間をリセット

        var radius = 1.2f; //弾を配置する半径
        var n = 12; //弾を配置する数

        //発射基準角度（0°から始まるので0で初期化）
        float deg = 0;

        //円周を一周するまで弾を作成
        for (int i = 0; i < n; i++)
        {
            //角度degからラジアンを作成
            var rad = deg * Mathf.Deg2Rad;
            //ラジアンを用いて、sinとcosを求める
            var sin = Mathf.Sin(rad);
            var cos = Mathf.Cos(rad);
            //円周上の点を求める
            //円の中心座標に半径をかけたcosとsinを足す
            var pos = this.gameObject.transform.position + new Vector3(cos * radius, sin * radius, 0);
            //弾の作成
            var shot = Instantiate(BossBullet, pos, Quaternion.identity);
            //自機方向に飛ばす
            shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);

            deg += 360 / n;

        }
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }

    //立山　雪の大谷
    public void yuki_no_ootani()
    {

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        int n = 3;

        // 1秒を超えているか 数字が小さいほど連射
        if (timeCount <= 0.5f) return;
        timeCount = 0; // 再発射のために時間をリセット


        // 弾速をイージング関数で緩急つける　timeCount2が1のとき最大値をとるため、繰り返し動かすように制御
        shotSpeed = 3;
        shotSpeedB = 2.5f;


        shotAngle = 270; // 真下に発射
        shotAngleB = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);

        // 追尾弾生成　星弾青を使う
        // 発射する弾を生成する
        var shot1 = Instantiate(BossBullet, this.transform.position, Quaternion.identity);
        // BulletスクリプトのInitを呼び出す
        shot1.Init(shotAngleB, shotSpeedB);

        //左右の壁生成 大玉弾灰色を使う
        for (int i = 0; i < n; i++)
        {
            var position_left = new Vector3(-4.7f + i * 1.0f, 6.0f, 0);

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBulletB, position_left, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);

        }
        for (int i = 0; i < n; i++)
        {
            var position_right = new Vector3(4.7f - i * 1.0f, 6.0f, 0);

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBulletB, position_right, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);

        }



    }

    //立山　阿弥陀如来 難しくする→自機狙いだけ休憩時間をなくす
    public void amidanyorai()
    {
        var TC1 = 2;//連射時間
        var TC2 = 0.7;//休憩時間
        // 前フレームからの時間の差を加算
        timeCountB += Time.deltaTime;
        // 連射フェーズ
        if (timeCountB <= TC1)
        {

            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= 0.3f) return;
            timeCount = 0; // 再発射のために時間をリセット

            shotSpeed = 15;

            var position_left = new Vector3(-3.5f, 3.6f, 0);
            var position_right = new Vector3(3.5f, 3.6f, 0);

            var shotAngle_left = GetAngle(position_left, ShootingPlayer.m_instance.transform.position);


            int nLeft = 20; // nway弾のnを指定
            int nRight = 30; // nway弾のnを指定

            // 左　自機狙い　矢弾黄色を使う
            for (int i = 0; i < nLeft; i++)
            {
                //自機狙いが基点
                shotAngle = shotAngle_left + i * 360 / nLeft;

                // インスタンス生成
                // 発射する弾を生成する
                var shot = Instantiate(BossBullet, position_left, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(shotAngle, shotSpeed);
            }

            // 右　固定　矢弾黄色を使う
            for (int i = 0; i < nRight; i++)
            {
                // 真下90度が基点
                shotAngle = 90 + i * 360 / nRight;

                // インスタンス生成
                // 発射する弾を生成する
                var shot = Instantiate(BossBullet, position_right, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(shotAngle, shotSpeed);
            }
        }
        if (timeCountB > TC1 && timeCountB <= TC1 + TC2) timeCount = 0;//休憩フェーズ
        if (timeCountB > TC1 + TC2) timeCountB = 0;//フェーズリセット


    }

    //立山　地獄谷　氷弾黄色を使う
    public void zigokudani()
    {
        float rnd = Random.Range(-4.9f, 4.9f);
        var shotSpeed = 10;

        var TC1 = 2;//連射時間
        var TC2 = 0.7;//休憩時間
        // 前フレームからの時間の差を加算
        timeCountB += Time.deltaTime;
        // 連射フェーズ
        if (timeCountB <= TC1)
        {

            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= 0.3f) return;
            timeCount = 0; // 再発射のために時間をリセット


            var BaseAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);


            int n = 20; // nway弾のnを指定

            for (int i = 0; i < n; i++)
            {
                //自機狙いが基点
                shotAngle = BaseAngle + i * 360 / n;

                // インスタンス生成
                // 発射する弾を生成する
                var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(shotAngle, shotSpeed);
            }
            BaseAngle += rnd;
        }
        if (timeCountB > TC1 && timeCountB <= TC1 + TC2) timeCount = 0;//休憩フェーズ
        if (timeCountB > TC1 + TC2) timeCountB = 0;//フェーズリセット

    }

    //白山　七つの池　大玉弾水色を使う
    public void nanatu_no_ike()
    {
        shotNum = 10;
        shotSpeed = 15;

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        float rnd = Random.Range(-1.0f, 1.0f);
        Vector3[] position = new Vector3[]
        {
            new Vector3(-4.0f+rnd, 3.6f+rnd, 0),
            new Vector3(-1.0f+rnd, 3.6f+rnd, 0),
            new Vector3(1.0f+rnd, 3.6f+rnd, 0),
            new Vector3(4.0f+rnd, 3.6f+rnd, 0),
            new Vector3(-3.0f+rnd, 1.8f+rnd, 0),
            new Vector3(0.0f+rnd, 1.8f+rnd, 0),
            new Vector3(3.0f+rnd, 1.8f+rnd, 0)
        };

        float rnd_angle = Random.Range(-4.9f, 4.9f);

        if (j < 7)
        {
            timeCount += Time.deltaTime;

            if (timeCount > 0.4f)
            {
                timeCount = 0; // 再発射のために時間をリセット

                var BaseAngle = GetAngle(position[j], ShootingPlayer.m_instance.transform.position);

                for (int i = 0; i < shotNum; i++)
                {
                    //自機狙いが基点
                    shotAngle = BaseAngle + i * 360 / shotNum;

                    // インスタンス生成
                    // 発射する弾を生成する
                    var shot = Instantiate(BossBullet, position[j], Quaternion.identity);

                    // BulletスクリプトのInitを呼び出す
                    shot.Init(shotAngle, shotSpeed);
                }
                j++;
            }
        }

        else
        {
            timeCountB += Time.deltaTime;
            if (timeCountB > 2.0f)
            {
                j = 0;
                timeCountB = 0;
            }
        }
    }

    //白山　カール（氷河）　鱗弾水色を使う
    void Kar()
    {
        shotInterval = 0.5f;
        shotNum = 6;

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        // 1秒を超えているか
        if (timeCount <= shotInterval) return;
        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定
        int m = 3;//さらにm方向に複製

        for (int j = 0; j < m; j++)
        {

            shotAngle = 120 * j + GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
            shotSpeed = 6.0f; // Speedは6で固定しておく。インスペクターから設定不可

            // 発射する回数分ループする
            for (int i = 0; i < n; i++)
            {
                // 真下90度が基点


                // インスタンス生成
                // 発射する弾を生成する
                var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(shotAngle, shotSpeed);

                shotSpeed = shotSpeed * 0.9f;
                shotAngle += 3;
            }

            shotAngle = 120 * j + GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
            shotSpeed = 6.0f;

            for (int i = 0; i < n; i++)
            {
                // 真下90度が基点


                // インスタンス生成
                // 発射する弾を生成する
                var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(shotAngle, shotSpeed);

                shotSpeed = shotSpeed * 0.9f;
                shotAngle -= 3;
            }
        }
        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生
    }

    //白山　花畑　星弾灰色を使う
    public void hanabatake()
    {
        var TP1 = 4.0f;//TimePoint 連射時間1
        var TP2 = 2.4f;//連射時間2
        var TP4 = 2.4f;//休憩時間

        shotInterval = 0.04f;
        //shotNum = 1;
        shotSpeed = 3f;// 螺旋弾では速度に影響なし

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        if (timeCount <= shotInterval) return;
        //timeCount = 0; // 再発射のために時間をリセット

        var radius = 0.2f; // 回転半径。ループで0.5ずつ増加させる
        var angleVel_left = 20.0f; //角速度
                                   var angleVel_right = -10.0f; //角速度
        //フェーズ1
        if (timeCount <= TP1)
        {

            // 前フレームからの時間の差を加算
            timeCountB += Time.deltaTime;
            // 1秒を超えているか
            if (timeCountB <= shotInterval) return;
            timeCountB = 0; // 再発射のために時間をリセット
                            // インスタンス生成
                            // 発射する弾を生成する
            float initX_left = this.transform.position.x +
                radius * Mathf.Cos(shotAngle * Mathf.Deg2Rad);
            float initY_left = this.transform.position.y +
                radius * Mathf.Sin(shotAngle * Mathf.Deg2Rad);
            Vector3 initPosition = new Vector3(initX_left, initY_left, 0);
            var shot = Instantiate(BossBullet, initPosition,
                Quaternion.identity);
            var shotC = Instantiate(BossBulletB, initPosition,
        Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.spiralInit(shotSpeed, radius, angleVel_left, this.transform.position);
            shotC.spiralInit(shotSpeed, radius, angleVel_left, this.transform.position);
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

            shotAngle += 31; // 発射角度を10度ずらす
        }
        //逆回転フェーズ
        else if (timeCount > TP1 && timeCount <= TP2+TP1)
        {
            // 前フレームからの時間の差を加算
            timeCountB += Time.deltaTime;
            // 1秒を超えているか
            if (timeCountB <= shotInterval) return;
            timeCountB = 0; // 再発射のために時間をリセット
                            // インスタンス生成
                            // 発射する弾を生成する
            float initX_left = this.transform.position.x +
                radius * Mathf.Cos(shotAngle * Mathf.Deg2Rad);
            float initY_left = this.transform.position.y +
                radius * Mathf.Sin(shotAngle * Mathf.Deg2Rad);
            Vector3 initPosition = new Vector3(initX_left, initY_left, 0);
            var shot = Instantiate(BossBullet, initPosition,
                Quaternion.identity);
            var shotC = Instantiate(BossBulletB, initPosition,
        Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.spiralInit(shotSpeed, radius, angleVel_right, this.transform.position);
            shotC.spiralInit(shotSpeed, radius, angleVel_right, this.transform.position);
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

            shotAngle -= 31; // 発射角度を10度ずらす
        }
        //休憩フェーズ
        else if (timeCount >TP2+TP1  && timeCount <= TP2 + TP1 + TP4)
        {
            // 前フレームからの時間の差を加算
            timeCountB += Time.deltaTime;
            // 1秒を超えているか
            if (timeCountB <= 0.5) return;
            timeCountB = 0; // 再発射のために時間をリセット

            var AngleToMe = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
            var shotToMe = Instantiate(BossBulletB, this.transform.position, Quaternion.identity);
            shotToMe.Init(AngleToMe, 0.7f);
            shotToMe.motion = MotionList.normal;
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

        }
        //フェーズリセット
        else if(timeCount > TP2 + TP1 + TP4)
            timeCount = 0;
    }

    //白山　錦ヶ滝　グミ弾水色を使う
    //Linerにするならspace = 1.0, timecount = 0.4
    //難易度上げるなら自機狙いを別個で出す
    public void Nishikigataki()
    {
        shotNum = 6;
        shotSpeed = 15;
        var space = 1.2f;//花同士の間隔 1.0

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        if (timeCount <= 0.65f) return;//0.4
        timeCount = 0; // 再発射のために時間をリセット

        //Vector3 shotPosition = new Vector3 (0.0f, 6.0f, 0.0f);
        var AngleBase = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);

        for (int i = 0; i < shotNum; i++)
        {
            float rnd_angle = Random.Range(-3.0f, 3.0f);
            float rnd_position = Random.Range(-0.3f, 0.3f);

            //自機から右側
            var positionPlus = new Vector3(0 + space * i + rnd_position, 6.0f, 0);
            // インスタンス生成
            // 発射する弾を生成する
            var shotPlus = Instantiate(BossBullet, positionPlus, Quaternion.identity);
            // BulletスクリプトのInitを呼び出す
            shotPlus.Init(AngleBase + rnd_angle, shotSpeed);

            //自機から左側
            var positionMinus = new Vector3(0 - space * i + rnd_position, 6.0f, 0);
            // インスタンス生成
            // 発射する弾を生成する
            var shotMinus = Instantiate(BossBullet, positionMinus, Quaternion.identity);
            // BulletスクリプトのInitを呼び出す
            shotMinus.Init(AngleBase + rnd_angle, shotSpeed);

        }
    }

    //富士山　ダイヤモンド富士　グミ弾橙を使う
    public void dyamond()
    {
        var shotSpeed = 1;
        var position_over = new Vector3(0, 4.0f, 0);
        var position_under = new Vector3(0, -4.0f, 0);

        //弾幕切り替え時、一度だけ実行させる
        if (!ShotCalledOnce)
        {
            GameObject Illusion = Instantiate(IllusionPrefab, position_under, Quaternion.identity); //カットインオブジェクトの出現
            audioSource.PlayOneShot(bossFamiliarClip, 0.5f); // SE再生
            ShotCalledOnce = !ShotCalledOnce; //カットイン用の変数を反転させることで一度だけ実行

        }

        var TC1 = 1.5;//連射時間
        var TC2 = 0.7;//休憩時間
        // 前フレームからの時間の差を加算
        timeCountB += Time.deltaTime;
        timeCountC += Time.deltaTime;

        int n = 8; // nway弾のnを指定

        // 連射フェーズ
        if (timeCountB <= TC1)
        {

            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= 0.05f) return;
            timeCount = 0; // 再発射のために時間をリセット

            for (int i = 0; i < n; i++)
            {
                shotAngle += i * 360 / n;

                // インスタンス生成
                // 発射する弾を生成する
                var shot = Instantiate(BossBullet, position_over, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(shotAngle, shotSpeed);
                shot.motion = MotionList.normal;

            }

            shotAngle += 1;

        }

        if (timeCountB > TC1 || timeCountB <= TC1 * 2)
        {

            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= 0.05f) return;
            timeCount = 0; // 再発射のために時間をリセット

            for (int i = 0; i < n; i++)
            {
                shotAngleB += i * 360 / n;

                // インスタンス生成
                // 発射する弾を生成する
                var shot = Instantiate(BossBullet, position_under, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(shotAngleB, shotSpeed);
                shot.motion = MotionList.normal;

            }
            shotAngleB -= 1;
        }
        //if (timeCountB > TC1 && timeCountB <= TC1 + TC2) timeCount = 0;//休憩フェーズ
        if (timeCountB > TC1 * 2) timeCountB = 0;//フェーズリセット

        //別個で自機狙いを出す
        if (timeCountC <= 2.0f) return;
        var AngleToMe = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        var shotToMe = Instantiate(BossBulletB, position_over, Quaternion.identity);
        shotToMe.Init(AngleToMe, 1.1f);
        shotToMe.motion = MotionList.normal;
        timeCountC = 0;
    }

    //富士山　樹海の魂　使い魔
    public void jukai()
    {
        shotNum = 4;
        shotSpeed = Random.Range(1.0f, 10f);
        var AngleBase = 180;
        //ここではrandomCountで抽出した数字の大きさに対して条件分岐することで
        //ランダムなインターバルを再現している。インターバルが開きすぎないように別途shotIntervalでも補完
        var randomCount = Random.Range(0.3f, 10.0f);
        shotInterval = 1.0f;

        float rnd_angle = Random.Range(-20.0f, 20.0f);
        float rnd_position = Random.Range(-2.0f, 3.0f);

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        if (randomCount >= 9.99f || timeCount > shotInterval)
        {


            timeCount = 0; // 再発射のために時間をリセット

            Vector3 shotPosition = new Vector3(6.0f, 1 + rnd_position, 0.0f);

            // インスタンス生成
            // 使い魔を生成する。使い魔のスクリプトで弾発射を組み込んでいるのでここではこれだけでよい
            var shotPlus = Instantiate(Familiar, shotPosition, Quaternion.identity);
            // FamiliarスクリプトのInitを呼び出す
            shotPlus.Init(AngleBase + rnd_angle, shotSpeed);
            shotPlus.shotInterval = 0.25f;
            shotPlus.VortexBullet_familiar_On = true; //この使い魔はvortexを出す

            timeCount = 0;
            audioSource.PlayOneShot(bossFamiliarClip, 0.2f); // SE再生

        }
    }

    // 富士山　大噴火　重力弾丸弾赤、重力弾星橙
    // 発射のリズムを作るのに煩雑になってしまったため、要リファクタリング
    void Daifunka()
    {
        var TP1 = 0.9f;//TimePoint 連射時間1
        var TP2 = TP1 * 7 / 3;//連射時間2
        var TP4 = 0.3f;//休憩時間

        shotInterval = 2.0f;
        // 前フレームからの時間の差を加算
        timeCountB += Time.deltaTime;

        int n; //弾を配置する数
        float radius; //弾を配置する半径

        //ポンデリングのサイズ抽選
        int rnd_size = Random.Range(0, 3);
        if (rnd_size == 0)
        {
            n = 8;
            radius = 0.6f;
        }
        else if (rnd_size == 1)
        {
            n = 12;
            radius = 0.8f;
        }
        else
        {
            n = 18;
            radius = 1.2f;
        }

        //重力弾に加える力に幅を持たせる
        float rnd_x = Random.Range(-200.0f, 200.0f);
        float rnd_y = Random.Range(-100.0f, 100.0f);

        //発射基準角度（0°から始まるので0で初期化）
        float deg = 0;

        // 3連射
        if (timeCountB <= TP1)
        {

            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= TP1 / 3) return;
            timeCount = 0; // 再発射のために時間をリセット

            for (int i = 0; i < n; i++)
            {
                //角度degからラジアンを作成
                var rad = deg * Mathf.Deg2Rad;
                //ラジアンを用いて、sinとcosを求める
                var sin = Mathf.Sin(rad);
                var cos = Mathf.Cos(rad);
                //円周上の点を求める
                //円の中心座標に半径をかけたcosとsinを足す
                var pos = this.gameObject.transform.position + new Vector3(cos * radius, sin * radius, 0);
                //弾の作成
                var shot = Instantiate(BossBullet, pos, Quaternion.identity);
                //自機方向に飛ばす
                shotAngle = 270;
                // BulletスクリプトのInitを呼び出す　重力弾のため加える速度は0
                shot.Init(shotAngle, 0);
                //重力弾　Initを呼び出すとMotionList.linearで上書きされるため、さらに上書き
                shot.motion = MotionList.gravity;

                Vector2 force = new Vector2(rnd_x, 300 + rnd_y);
                shot.rigid2D.AddForce(force);//上方向に力を加えて重力弾を打ち上げる
                deg += 360 / n;

            }
            //重力弾星橙をばらまく
            for (int i = 0; i < 10; i++)
            {
                //前述のrnd_x,yを用いると同じ軌道になってしまうため、別の変数にする必要があった
                float rnd_x2 = Random.Range(-100.0f, 100.0f);
                float rnd_y2 = Random.Range(-100.0f, 100.0f);
                float rnd_gravity = Random.Range(-0.05f, 0.05f);

                var shot = Instantiate(BossBulletB, this.transform.position, Quaternion.identity);
                shotAngle = 270;
                shot.Init(shotAngle, 0);
                shot.motion = MotionList.gravity;
                Vector2 force = new Vector2(rnd_x2, 300 + rnd_y2);
                shot.rigid2D.AddForce(force);//上方向に力を加える
                //重力もわずかにランダムにする
                shot.transform.gameObject.GetComponent<Rigidbody2D>().gravityScale += rnd_gravity;
            }
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生
        }
        //再度3連射
        if (timeCountB > TP1 + TP4 && timeCountB <= TP1 * 2 + TP4)
        {
            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= TP1 / 3) return;
            timeCount = 0; // 再発射のために時間をリセット

            for (int i = 0; i < n; i++)
            {
                //角度degからラジアンを作成
                var rad = deg * Mathf.Deg2Rad;
                //ラジアンを用いて、sinとcosを求める
                var sin = Mathf.Sin(rad);
                var cos = Mathf.Cos(rad);
                //円周上の点を求める
                //円の中心座標に半径をかけたcosとsinを足す
                var pos = this.gameObject.transform.position + new Vector3(cos * radius, sin * radius, 0);
                //弾の作成
                var shot = Instantiate(BossBullet, pos, Quaternion.identity);
                //自機方向に飛ばす
                shotAngle = 270;
                // BulletスクリプトのInitを呼び出す　重力弾のため加える速度は0
                shot.Init(shotAngle, 0);
                //重力弾　Initを呼び出すとMotionList.linearで上書きされるため、さらに上書き
                shot.motion = MotionList.gravity;

                Vector2 force = new Vector2(rnd_x, 300 + rnd_y);
                shot.rigid2D.AddForce(force);//上方向に力を加える
                deg += 360 / n;

            }
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

        }
        //7連射
        if (timeCountB > TP1 * 2 + TP4 * 2 && timeCountB <= TP1 * 2 + TP2 + TP4 * 2)
        {
            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= TP2 / 7) return;
            timeCount = 0; // 再発射のために時間をリセット

            for (int i = 0; i < n; i++)
            {
                //角度degからラジアンを作成
                var rad = deg * Mathf.Deg2Rad;
                //ラジアンを用いて、sinとcosを求める
                var sin = Mathf.Sin(rad);
                var cos = Mathf.Cos(rad);
                //円周上の点を求める
                //円の中心座標に半径をかけたcosとsinを足す
                var pos = this.gameObject.transform.position + new Vector3(cos * radius, sin * radius, 0);
                //弾の作成
                var shot = Instantiate(BossBullet, pos, Quaternion.identity);
                //自機方向に飛ばす
                shotAngle = 270;
                // BulletスクリプトのInitを呼び出す　重力弾のため加える速度は0
                shot.Init(shotAngle, 0);
                //重力弾　Initを呼び出すとMotionList.linearで上書きされるため、さらに上書き
                shot.motion = MotionList.gravity;

                Vector2 force = new Vector2(rnd_x, 300 + rnd_y);
                shot.rigid2D.AddForce(force);//上方向に力を加える
                deg += 360 / n;

            }
            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

        }
        //フェーズリセット
        if (timeCountB > TP1 * 2 + TP2 + TP4 * 3) timeCountB = 0;

    }

    //富士山　一富士二鷹三茄子　鱗弾青、使い魔鷹、大玉弾桃
    public void nitaka()
    {
        var TP1 = 1.50f;//TimePoint 連射時間1
        var TP2 = 2;//連射時間2
        var TP3 = 1.5f;//連射時間3
        var TP4 = 0.3f;//休憩時間
        // 前フレームからの時間の差を加算
        timeCountB += Time.deltaTime;
        // 一富士連射フェーズ
        if (timeCountB <= TP1)
        {

            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 1秒を超えているか
            if (timeCount <= 0.05f) return;
            timeCount = 0; // 再発射のために時間をリセット

            shotSpeed = 4f;

            //ワインダー開始地点　富士山の山頂をイメージして幅広に
            var position_left = new Vector3(-0.5f, 4, 0);
            var position_right = new Vector3(0.5f, 4, 0);

            // 左

            var shotAngle_left = 200 + j;
            // インスタンス生成
            // 発射する弾を生成する
            var shot_left = Instantiate(BossBullet, position_left, Quaternion.identity);
            // BulletスクリプトのInitを呼び出す
            shot_left.Init(shotAngle_left, shotSpeed);
            //イージングを使わない　Initを呼び出すとMotionList.linearで上書きされるため、さらに上書き
            shot_left.motion = MotionList.normal;

            //右
            var shotAngle_right = -20 - j;
            // インスタンス生成
            // 発射する弾を生成する
            var shot_right = Instantiate(BossBullet, position_right, Quaternion.identity);
            // BulletスクリプトのInitを呼び出す
            shot_right.Init(shotAngle_right, shotSpeed);
            shot_right.motion = MotionList.normal;


            j += 2;

        }
        //二鷹連射フェーズ　前の弾幕と若干かぶるように開始時刻を1秒前倒し
        if (timeCountB > TP1 - 1 && timeCountB <= (TP1 + TP2))
        {
            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 2つ発射
            if (timeCount <= TP2 / 2) return;
            timeCount = 0; // 再発射のために時間をリセット

            var shot = Instantiate(Familiar, this.transform.position, Quaternion.identity);
            // familiarスクリプトのInitを呼び出す
            shot.Init(shotAngle, 2);
            shot.shotInterval = 0.2f;
            shot.ToMeBullet_familiar_On = true; //この使い魔はToMeBulletを出す

            shotAngle += 180;
        }
        //三茄子連射フェーズ
        if (timeCountB > (TP1 + TP2 - 1) && timeCountB <= (TP1 + TP2 + TP3))
        {
            // 前フレームからの時間の差を加算
            timeCount += Time.deltaTime;
            // 3つ発射
            if (timeCount <= TP3 / 3) return;
            timeCount = 0; // 再発射のために時間をリセット

            shotSpeed = 2f;
            var AngleBase = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);

            for (int i = 0; i < 7; i++)
            {
                var shot = Instantiate(BossBulletB, this.transform.position, Quaternion.identity);
                // familiarスクリプトのInitを呼び出す
                shot.Init(AngleBase, shotSpeed);
                //イージングを使わない　Initを呼び出すとMotionList.linearで上書きされるため、さらに上書き
                shot.motion = MotionList.normal;
                //ウェーブごとに左右反転
                if (k >= 7 && k < 14) AngleBase -= 3;
                else AngleBase += 3;

                shotSpeed += 0.1f;
                k++;
            }
        }
        //休憩フェーズ
        if (timeCountB > TP1 + TP2 + TP3 && timeCountB <= TP1 + TP2 + TP3 + TP4) timeCount = 0;
        //フェーズリセット
        if (timeCountB > TP1 + TP2 + TP3 + TP4)
        {
            j = 0;
            k = 0;
            timeCountB = 0;
        }
    }

    // 富士山　夕焼け1/3　反転橙弾を使う
    void Sunset1()
    {
        transform.localPosition = Vector3.MoveTowards(this.transform.position, new Vector3(0f, 0f, 0f), 0.13f * Time.deltaTime);
        shotNum = 8;
        shotInterval = 0.2f;
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        // 1秒を超えているか
        if (timeCount <= shotInterval) return;
        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定

        //var shotAngle1 = shotAngle; // 左右で弾幕展開 shotAngle1が左側の角度
        shotAngle += 6.1f;

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle += 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);
            shot.LargerSwitch = true;
            //大きさ関係の初期値を設定
            shot.MagnificationRate = 1.05f;
            shot.LargersizeMax = 0.15f;
            shot.LargerInterval = 0.1f;
            shot.TimePoint1 = 0;
            shot.TimePoint2 = 10;
        }

        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }

    // 富士山　夕焼け2/3　反転橙弾を使う
    void Sunset2()
    {
        transform.localPosition = Vector3.MoveTowards(this.transform.position, new Vector3(0f, 0f, 0f), 0.13f * Time.deltaTime);
        shotNum = 6;
        shotInterval = 0.15f;
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        // 1秒を超えているか
        if (timeCount <= shotInterval) return;
        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定

        //var shotAngle1 = shotAngle; // 左右で弾幕展開 shotAngle1が左側の角度
        shotAngle += 6.1f;

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle += 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);
            shot.LargerSwitch = true;
            //大きさ関係の初期値を設定
            shot.MagnificationRate = 1.05f;
            shot.LargersizeMax = 0.2f;
            shot.LargerInterval = 0.1f;
            shot.TimePoint1 = 0;
            shot.TimePoint2 = 10;
        }

        audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生

    }

    // 富士山　夕焼け3/3　反転橙弾を使う
    //ポンデリングを応用して、TimePoint1からもう1重弾幕を打ってもいい
    void Sunset3()
    {
        shotNum = 4;
        shotInterval = 0.1f;

        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;
        timeCountB += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;
        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定

        shotAngle += 6.1f;
        var shotPos = new Vector3(0, 0, 0);
        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle += 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(BossBullet, transform.localPosition, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);
            shot.LargerSwitch = true;
            //大きさ関係の初期値を設定
            shot.MagnificationRate = MagnificationRateShotage;
            shot.LargersizeMax = 0.8f;//最大サイズ
            shot.LargerInterval = 0.01f;//大きくなるインターバル
            shot.TimePoint1 = TimePoint1Shotage;//大きくなり始めるまでの時間
            shot.TimePoint2 = 2.0f;//大きくなる時間
            //DeleteBulletの範囲拡大
            shot.motion = MotionList.Sunset;

        }

        //0.1fごとにTimePoint1を小さく　→弾が小さい範囲が小さくなる
        if (timeCountB >= 0.1f)
        {
            if (TimePoint1Shotage >= TimePoint1Min)
            {
                TimePoint1Shotage -= 0.01f;
            }

            audioSource.PlayOneShot(bossBulletClip, 0.1f); // SE再生
            timeCountB = 0;
        }
    }

    // バレット追加枠
    void SampleBullet3()
    {

    }

    // 2点間の角度を取得
    float GetAngle(Vector2 start, Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    // 範囲外に行った敵を消すスクリプト
    void DeleteBoss()
    {
        float xp = this.transform.position.x;
        float yp = this.transform.position.y;
        if (xp < -8.0f | xp > 8.0f | yp < -8.0f | yp > 8.0f)
        {
            Destroy(gameObject);
        }
    }

    // プレイヤーの弾に当たったときの処理
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Attack");
        // 当たったのがプレイヤーの弾
        if (other.gameObject.CompareTag("PlayerBullet"))
        {

            //自身を一瞬点滅させる。点滅時間はupdate関数を参照
            if (spriteRendererBlinker._isBlinking == false)
            {
                spriteRendererBlinker.BeginBlink();
                //Debug.Log("BeginBlink実行開始");

            }

            //無敵時でも点滅はさせる
            if (state == STATE_Boss.MUTEKI) return;
            BossHP--;
            BossHPBarManager.GaugeReduction(BossHPMax, BossHP, 1);//☆追加1014　HPバー減少

            if (BossHP > 0) return; // 体力が残っている場合は処理スキップ

            BossDead = true; // 死んだ信号をONにする　BossManagerで読み取り

            // 自身を消す
            Destroy(gameObject);
            // 弾も消す
            Destroy(other.gameObject);
            // 弾が当たった場所に爆発エフェクトを生成する
            Instantiate(
                ExtinguishPrefab,
                other.transform.localPosition,
                Quaternion.identity);

        }
    }

    public void MoveSwitchCase(Movement BossMovement)
    {
        switch (BossMovement)
        {
            case (Movement.StraightMove):
                BossStraightMove();
                break;
            case (Movement.ZMove):
                BossZMove();
                break;
            case (Movement.KOMove):
                BossKOMove();
                break;
            case (Movement.VMove):
                BossVMove();
                break;
            case (Movement.GoAndBackMove):
                BossGoAndBackMove();
                break;
            case (Movement.LMove):
                BossLMove();
                break;
            case (Movement.UMove):
                BossUMove();
                break;
            case (Movement.BendingMove):
                BossBendingMove();
                break;
            case (Movement.ToPlayerMove):
                BossToPlayerMove();
                break;
            case (Movement.CurveMove):
                BossCurveMove();
                break;
            case (Movement.SMove):
                BossSMove();
                break;
            case (Movement.CircleMove):
                BossCircleMove();
                break;
            case (Movement.WaveMove):
                BossWaveMove();
                break;
            case (Movement.RLMove):
                BossRLMove();
                break;
            default:
                BossStraightMove();
                break;

        }

    }

    // 直進スクリプト
    void BossStraightMove()
    {
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        var destination = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        // StraightMoveTime秒後にdestinationへ到達する
        var tween = this.transform.DOMove(destination, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear);

    }

    // Z字移動スクリプト
    void BossZMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        //オブジェクトからみて左斜め後ろに直進
        var m_direction2x = -0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2y = 0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * BossSpeed * StraightMoveTime;
        //元の方向で直進
        var destination3 = destination2 + m_direction * BossSpeed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // コの字移動スクリプト
    void BossKOMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);

        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        //左に直角で曲がる
        var m_direction2x = -m_direction.y;
        var m_direction2y = m_direction.x;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * BossSpeed * StraightMoveTime;
        //左に直角で曲がる
        var destination3 = destination2 - m_direction * BossSpeed * StraightOverTime;
        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // V字移動スクリプト
    void BossVMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;

        //オブジェクトからみて左斜め後ろに直進
        var m_direction2x = -0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2y = 0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * BossSpeed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));


    }

    // 往復移動スクリプト
    void BossGoAndBackMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        //逆方向に帰る
        var destination0 = destination1 - m_direction * BossSpeed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination0, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));


    }

    // L字移動スクリプト
    void BossLMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);

        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        var m_direction2x = -m_direction.y;
        var m_direction2y = m_direction.x;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        //左に直角で曲がる
        var destination2 = destination1 + m_direction2 * BossSpeed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // U字移動スクリプト
    void BossUMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);

        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        //半円移動
        var m_direction2x = -m_direction.y;
        var m_direction2y = m_direction.x;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);

        var destination2 = destination1 + m_direction2 * BossSpeed * StraightMoveTime;

        var destination12C = (destination1 + destination2) / 2; //1と2の中点計算
        var destination1_2 = destination12C + m_direction; // 単位ベクトルだけ位置ずらして中間地点設定

        //逆方向に直進
        var destination3 = destination2 - m_direction * BossSpeed * StraightOverTime;

        Vector3[] path = {
            destination1_2,
            destination2,
        };

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOLocalPath(path, StraightMoveTime, PathType.CatmullRom).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // 鈍角V字移動スクリプト
    void BossBendingMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        // 直進
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        // 左斜め前に直進
        var m_direction2x = 0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2y = 0.707f * m_direction.x + 0.707f * m_direction.y;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * BossSpeed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // 自機方向移動型スクリプト
    void BossToPlayerMove()
    {
        var sequence = DOTween.Sequence();
        // プレイヤーの現在地を取得する
        var playerPos = ShootingPlayer.m_instance.transform.position;
        var toPlayerdirection = playerPos - this.transform.position;
        toPlayerdirection.Normalize(); //正則化
        Debug.Log(toPlayerdirection);

        //目的地の計算
        //自機の方向へ直進
        var destination1 = this.transform.position + toPlayerdirection * BossSpeed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));
    }

    // カーブ移動スクリプト
    void BossCurveMove()
    {

        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        // 直進先の座標
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        // 垂直方向の計算
        var m_directionVerticalx = m_direction.y;
        var m_directionVerticaly = -m_direction.x;
        var m_directionVertical = new Vector3(m_directionVerticalx, m_directionVerticaly, 0);
        // 中点計算
        var destinationC = (this.transform.position + destination1) / 2;
        // 二点間距離
        var distance01 = Vector3.Distance(this.transform.position, destination1);
        // 描いたカーブが1/4円になるような中間座標を設定
        var destination0_2 = (1.0f - 1.0f / 1.41f) * distance01 * m_directionVertical
                            + destinationC;
        // 左斜め前に直進
        var m_direction2x = 0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2y = 0.707f * m_direction.x + 0.707f * m_direction.y;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * BossSpeed * StraightOverTime;


        Vector3[] path = {
            destination0_2,
            destination1,
        };

        sequence.Append(this.transform.DOLocalPath(path, StraightMoveTime, PathType.CatmullRom).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));
    }

    // S字移動スクリプト
    void BossSMove()
    {

        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        // S字の上半分の移動
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime;
        var m_directionVerticalx1 = m_direction.y;
        var m_directionVerticaly1 = -m_direction.x;
        var m_directionVertical1 = new Vector3(m_directionVerticalx1, m_directionVerticaly1, 0);

        var destinationC = (this.transform.position + destination1) / 2;
        var distance01 = Vector3.Distance(this.transform.position, destination1);

        var destination0_2 = 0.5f * distance01 * m_directionVertical1 + destinationC;
        // S字の下半分の移動
        var destination2 = destination1 + m_direction * BossSpeed * StraightMoveTime;
        var m_directionVerticalx2 = -m_direction.y;
        var m_directionVerticaly2 = m_direction.x;
        var m_directionVertical2 = new Vector3(m_directionVerticalx2, m_directionVerticaly2, 0);

        var destinationC2 = (destination1 + destination2) / 2;
        var distance02 = Vector3.Distance(destination1, destination2);

        var destination1_2 = 0.5f * distance02 * m_directionVertical2 + destinationC2;
        // 画面外まで直進
        var destination3 = destination2 + m_directionVertical1 * BossSpeed * StraightOverTime;


        Vector3[] path = {
            destination0_2,
            destination1,
            destination1_2,
            destination2,
        };

        sequence.Append(this.transform.DOLocalPath(path, StraightMoveTime * 2, PathType.CatmullRom).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));
    }

    // 円移動スクリプト
    void BossCircleMove()
    {

        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //往復移動に中間地点を追加して円運動を実装
        var destination05 = this.transform.position + m_direction * BossSpeed * StraightMoveTime / 2;
        var destination1 = destination05 + m_direction * BossSpeed * StraightMoveTime;
        var m_directionVerticalx1 = m_direction.y;
        var m_directionVerticaly1 = -m_direction.x;
        var m_directionVertical1 = new Vector3(m_directionVerticalx1, m_directionVerticaly1, 0);

        var destinationC = (destination05 + destination1) / 2;
        var distance01 = Vector3.Distance(destination05, destination1);

        var destination0_2 = 0.5f * distance01 * m_directionVertical1 + destinationC;

        var destination1_2 = -0.5f * distance01 * m_directionVertical1 + destinationC;

        var destination2 = destination05 + m_direction * BossSpeed * StraightOverTime;


        Vector3[] path = {
            destination0_2,
            destination1,
            destination1_2,
            destination05,
        };

        sequence.Append(this.transform.DOMove(destination05, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOLocalPath(path, StraightMoveTime * 2, PathType.CatmullRom).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));
    }

    // 波形移動スクリプト
    void BossWaveMove()
    {
        //S字移動を3回繰り返す
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        var destination1 = this.transform.position + m_direction * BossSpeed * StraightMoveTime / 4;
        var m_directionVerticalx1 = m_direction.y;
        var m_directionVerticaly1 = -m_direction.x;
        var m_directionVertical1 = new Vector3(m_directionVerticalx1, m_directionVerticaly1, 0);

        var destinationC = (this.transform.position + destination1) / 2;
        var distance01 = Vector3.Distance(this.transform.position, destination1);

        var destination0_2 = 1.41f * distance01 * m_directionVertical1 + destinationC;

        var destination2 = destination1 + m_direction * BossSpeed * StraightMoveTime / 4;
        var destinationC2 = (destination1 + destination2) / 2;

        var destination1_2 = -1.41f * distance01 * m_directionVertical1 + destinationC2;

        var destination2_2 = 2 * distance01 * m_direction + destination0_2;

        var destination3 = destination2 + m_direction * BossSpeed * StraightMoveTime / 4;

        var destination3_2 = 2 * distance01 * m_direction + destination1_2;

        var destination4 = destination3 + m_direction * BossSpeed * StraightMoveTime / 4;

        var destination4_2 = 2 * distance01 * m_direction + destination2_2;

        var destination5 = destination4 + m_direction * BossSpeed * StraightMoveTime / 4;

        var destination5_2 = 2 * distance01 * m_direction + destination3_2;

        var destination6 = destination5 + m_direction * BossSpeed * StraightMoveTime / 4;

        var destination7 = destination6 + m_direction * BossSpeed * StraightOverTime;

        Vector3[] path = {
            destination0_2,
            destination1,
            destination1_2,
            destination2,
            destination2_2,
            destination3,
            destination3_2,
            destination4,
            destination4_2,
            destination5,
            destination5_2,
            destination6,
        };

        sequence.Append(this.transform.DOLocalPath(path, StraightMoveTime * 3, PathType.CatmullRom).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination7, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));
    }

    // U字移動スクリプト
    public void BossRLMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);

        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction;
        //90度回転
        var m_direction2x = -m_direction.y;
        var m_direction2y = m_direction.x;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        //右に直進
        var destination2 = destination1 + m_direction2 * BossSpeed * StraightMoveTime / 2;
        //左に直進
        var destination3 = destination2 - m_direction2 * BossSpeed * StraightMoveTime;
        //右へ
        var destination4 = destination3 + m_direction2 * BossSpeed * StraightMoveTime;

        sequence.Append(this.transform.DOMove(destination1, 1).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo));

    }


}