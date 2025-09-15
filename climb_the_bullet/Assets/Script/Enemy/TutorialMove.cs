using System.Collections;
using System.Collections.Generic;
using App.BaseSystem.DataStores.ScriptableObjects.Status;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class TutorialMove : MonoBehaviour
{
    //public string EnemyName;
    //private StatusDataStore statusDataStore;
    //private ShootingPlayer PlayerObject = GameObject.Find("Player").GetComponent<ShootingPlayer>();
    public float Speed = 0.1f;// 敵のスピード

    float timeCount = 0; // 経過時間
    float shotAngle = 0; // 発射角度
    float shotAngle_under = 270; // 発射角度

    float shotSpeed = 0; // 発射速度
    int shotNum = 10; // 発射数
    float shotInterval = 0.1f; // 発射間隔

    Bullet EnemyBullet; // 弾のプレハブ

    [SerializeField]
    private List<EnemyBulletTable> enemyBulletTable = new List<EnemyBulletTable>();

    [System.Serializable]
    class EnemyBulletTable
    {
        [System.NonSerialized]
        public float BulleTimeCount = 0; // 経過時間
        [System.NonSerialized]
        public float BulleAngle = 0; // 発射角度
        public float BulleSpeed = 0; // 発射速度
        public int BulleNum = 10; // 発射数
        public float BulleInterval = 1.0f; // 発射間隔
        public Bullet BullePrefab;
        public BulletPattern bulletPattern;
    }
    
    RaycastHit hit;

    public enum BulletPattern
    {
        Vortex,
        ToMe,
        nDirection,
        
    }

    BulletPattern bulletpattern;

    public ExtinguishEnemy ExtinguishPrefab; // 爆発エフェクトのプレハブ

    public int EnemyHPMax = 10; // HP の最大値
    private int EnemyHP; // HP

    [SerializeField] GameObject HPBarPrefab; //☆1012追加　HPBar呼び出しのため
    //[SerializeField] Transform parent;//☆1012追加　HPBar呼び出しのため
    HPBarManager HPBarManager;//☆1014追加 HPBar増減のため
    //☆ダメージ時点滅のための諸々
    float BlinkerTime;
    SpriteRendererBlinker spriteRendererBlinker;
    
    private Movement movement = Movement.StraightMove;
    private Vector3 m_direction = new Vector3(0, -1, 0); // 進行方向 

    public float StraightMoveTime = 5;
    private float StraightTimeCount = 0;
    private float StraightOverTime = 100;

    // チュートリアル進行に使うフラグ
    public static bool EnemyPouse = false; // 会話中に停止させる
    public bool Enemykilled = false; 
    // チュートリアルによって弾幕変える
    public bool EasyShot = false;
    public bool DenceShot = false;
    public bool FreeShot = false;

    //public AudioClip BulletClip; // 敵を倒した時に再生する SE
    public AudioClip BulletClip; // 弾のSE
    AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        //statusDataStore = FindObjectOfType<StatusDataStore>();
        //var EnemyData = statusDataStore.FindWithName(EnemyName); // 名前がEnemyNameであるデータを取得

        EnemyHP = EnemyHPMax; //HPの初期化
        //Debug.Log(movement);
        //☆1012追加　Enemyを親としたHPBar呼び出し
        var parent = this.transform;
        //GameObject HPBar = Instantiate(HPBarPrefab, parent) as GameObject;
        GameObject HPBar = Instantiate(HPBarPrefab, this.transform.position, Quaternion.identity, parent);

        HPBarManager = this.GetComponentInChildren<HPBarManager>();

        spriteRendererBlinker = GetComponent<SpriteRendererBlinker>();

        MoveSwitchCase(movement);

        audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>(); // SE読み込み
        //audioSource.volume = 1f;
    }

    // 敵が出現する時に初期化する関数
    public void EnemyInit(Movement EnemyMovement, Vector3 MoveDirection)
    {
        movement = EnemyMovement;
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
            if(BlinkerTime >= 1.0f)
            {
                spriteRendererBlinker.EndBlink();
                BlinkerTime = 0.0f;
                //Debug.Log("EndBlink実行");

            }
        }

        if (EnemyPouse) 
        {
            Debug.Log("EnemyPouse中");
            transform.DOPause();
            //GetComponent<EnemyMove>().DOPause();
            return;
        }
        //Debug.Log("EnemyPouse外");
        //GetComponent<EnemyMove>().DOPlay();
        transform.DOPlay();
        Debug.Log("EasyShot" + EasyShot);
        if (EasyShot) 
        {
            NwaveBulletParam(0);
            Debug.Log("EasyShot");
        }
        if (DenceShot) NwaveBulletParam(1);
        if (FreeShot) NwaveBulletParam(2);

        

        DeleteEnemy(); // 範囲外の敵消去
    }

    void NwaveBulletParam(int i)
    {
        timeCount = enemyBulletTable[i].BulleTimeCount;
        shotAngle = enemyBulletTable[i].BulleAngle;
        shotSpeed = enemyBulletTable[i].BulleSpeed;
        shotNum = enemyBulletTable[i].BulleNum;
        shotInterval = enemyBulletTable[i].BulleInterval;
        EnemyBullet = enemyBulletTable[i].BullePrefab;
        bulletpattern = enemyBulletTable[i].bulletPattern;
        bulletSwitchCase(bulletpattern);

        enemyBulletTable[i].BulleTimeCount = timeCount;
        enemyBulletTable[i].BulleAngle = shotAngle;
    }

    public void bulletSwitchCase(BulletPattern pattern)
    {
                //EnemyMoveLeft(); // 左移動
        switch (pattern)
        {
            case (BulletPattern.Vortex):
                VortexBullet();
                break;

            case (BulletPattern.ToMe):
                ToMeBullet();
                break;

            case (BulletPattern.nDirection):
                nDirectionBullet();
                break;
        }
    }

    // 渦状の弾幕
    public void VortexBullet()
    {
        
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        shotAngle -= 10; // 発射角度を10度ずらす
        // インスタンス生成
        // 発射する弾を生成する
        var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, shotSpeed);
        audioSource.PlayOneShot(BulletClip); // SE再生
    }

    // 自機狙い
    public void ToMeBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか 数字が小さいほど連射
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        var angleRange = 15;
        // 弾を複数発射する場合
        if (1 < shotNum)
        {
            // 発射する回数分ループする
            for (int i = 0; i < shotNum; ++i)
            {
                // 弾の発射角度を計算する
                var angle = shotAngle +
                    angleRange * (float)(i - (shotNum - 1) * 0.5f);

                // インスタンス生成
                // 発射する弾を生成する Instantiate( 生成するオブジェクト,  場所, 回転 );
                var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

                // BulletスクリプトのInitを呼び出す
                shot.Init(angle, shotSpeed);
            }
        }
        // 弾を 1 つだけ発射する場合
        else if (shotNum == 1)
        {
            // インスタンス生成
            // 発射する弾を生成する Instantiate( 生成するオブジェクト,  場所, 回転 );
            var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);
        }

        audioSource.PlayOneShot(BulletClip); // SE再生

    }

    public void nDirectionBullet()
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
            var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed); 
        }
        audioSource.PlayOneShot(BulletClip); // SE再生

    }

    // 2点間の角度を取得
    public float GetAngle(Vector2 start, Vector2 target)
    {
        Vector2 dt = target - start;
        float rad = Mathf.Atan2(dt.y, dt.x);
        float degree = rad * Mathf.Rad2Deg;

        return degree;
    }

    // 範囲外に行った敵を無効化するスクリプト
    public void DeleteEnemy()
    {
        float xp = this.transform.position.x;
        float yp = this.transform.position.y;
        if (xp < -8.0f | xp > 8.0f | yp < -8.0f | yp > 8.0f)
        {
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    // プレイヤーの弾に当たったときの処理
    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Attack");
        // 当たったのがプレイヤーの弾
        if (other.gameObject.CompareTag("PlayerBullet"))
        {

            var damage = other.gameObject.GetComponent<Shot_Shikigami>().shotDamage;	
            Debug.Log("EnemyHP"+EnemyHP);
            Debug.Log("damage"+damage);
            HPBarManager.GaugeReduction(EnemyHPMax, EnemyHP, damage);//☆追加1014　HPバー減少
            EnemyHP -= damage;

            //自身を一瞬点滅させる。点滅時間はupdate関数を参照
            if (spriteRendererBlinker._isBlinking == false)
            {
                spriteRendererBlinker.BeginBlink();
                Debug.Log("BeginBlink実行開始");
            }

            if (EnemyHP > 0) return; // 体力が残っている場合は処理スキップ
            Enemykilled = true;


            // 自身を消す
            Destroy(gameObject);
            var piercing = other.gameObject.GetComponent<Shot_Shikigami>().ShotPiercing;
            // 弾も消す
            if (!piercing) Destroy(other.gameObject);


            // 弾が当たった場所に爆発エフェクトを生成する
            Instantiate(
                ExtinguishPrefab,
                other.transform.localPosition,
                Quaternion.identity);

        }


    }

    public void MoveSwitchCase(Movement EnemyMovement)
    {
        switch (EnemyMovement)
        {
            case (Movement.StraightMove):
                EnemyStraightMove();
                break;
        }

    }

    // 直進スクリプト
    public void EnemyStraightMove()
    {
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        var destination = this.transform.position + m_direction * Speed * StraightMoveTime;
        // StraightMoveTime秒後にdestinationへ到達する
        var tween = this.transform.DOMove(destination, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear);

    }

}