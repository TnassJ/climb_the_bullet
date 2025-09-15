using System.Collections;
using System.Collections.Generic;
using App.BaseSystem.DataStores.ScriptableObjects.Status;
using UnityEngine;
using System.Linq;
using DG.Tweening;

public class EnemyMove : MonoBehaviour
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
        nDirection2,
        Baramaki,
        Sankaku,
        Sample2,
        Pondering,
        RLBullet,
        RevVortex,

    }

    BulletPattern bulletpattern;

    [SerializeField] GameObject ExpItem; // ドロップアイテム1
    public int ExpItemNum = 4; // ドロップアイテム個数
    [SerializeField] GameObject OneUpItem; // 残機ドロップ
    public int OneUpDropOn = 0;
    [SerializeField] GameObject BombItem; // ボムドロップ
    public int BombDropOn = 0;
    public ExtinguishEnemy ExtinguishPrefab; // 爆発エフェクトのプレハブ
    public float GemSpeedMin = 0; // 生成する宝石の移動の速さ（最小値）
    public float GemSpeedMax = 0.1f; // 生成する宝石の移動の速さ（最大値）

    public int EnemyHPMax = 10; // HP の最大値
    private int EnemyHP; // HP

    [SerializeField] GameObject HPBarPrefab; //☆1012追加　HPBar呼び出しのため
    //[SerializeField] Transform parent;//☆1012追加　HPBar呼び出しのため
    HPBarManager HPBarManager;//☆1014追加 HPBar増減のため
    //☆ダメージ時点滅のための諸々
    float BlinkerTime;
    SpriteRendererBlinker spriteRendererBlinker;
    
    private Movement movement = Movement.StraightMove;
    private Vector3 m_direction = new Vector3(-1, -1, 0); // 進行方向 

    public float StraightMoveTime = 5;
    private float StraightTimeCount = 0;
    private float StraightOverTime = 100;

    // ☆追加　エネミーのインスタンスを管理する static 変数
    public static EnemyMove m_instance;

    private List<Bullet> laserBullet = new List<Bullet>();
    public static bool EnemyPouse = false;
    public bool Enemykilled = false;


    
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
        m_instance = this;

        audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>(); // SE読み込み
        //audioSource.volume = 0.1f;
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
        
        for(int i = 0; i < enemyBulletTable.Count; i++){

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
        

        DeleteEnemy(); // 範囲外の敵消去
    }

    public void bulletSwitchCase(BulletPattern pattern)
    {
                //EnemyMoveLeft(); // 左移動
        switch (pattern)
        {
            case (BulletPattern.Vortex):
                VortexBullet();
                break;

            case (BulletPattern.RevVortex):
                RevVortexBullet();
                break;


            case (BulletPattern.ToMe):
                ToMeBullet();
                break;

            case (BulletPattern.nDirection):
                nDirectionBullet();
                break;

            case (BulletPattern.nDirection2):
                nDirectionBullet2();
                break;

            case (BulletPattern.Baramaki):
                BaramakiBullet();
                break;

            case (BulletPattern.Sankaku):
                SankakuBullet();
                break;

            case (BulletPattern.Sample2):
                LaserBullet();
                LaserCast();
                break;

            case (BulletPattern.Pondering):
                PonderingBullet();
                break;

            case (BulletPattern.RLBullet):
                RLBullet();
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
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生
    }

        // 渦状の弾幕
    public void RevVortexBullet()
    {
        
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        shotAngle += 10; // 発射角度を10度ずらす
        // インスタンス生成
        // 発射する弾を生成する
        var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, shotSpeed);
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生
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

        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生

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
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生

    }


    public void nDirectionBullet2()
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
            var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);
        }

        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点
            shotAngle1 -= 360 / n;

            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle1, shotSpeed);
        }
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生

    }

    //重力に従う弾を真上に放出
    public void BaramakiBullet()
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
            var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);
        }
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生

    }

    //三角形に放出
    public void SankakuBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット

        int n = shotNum; // nway弾のnを指定
        shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        //shotSpeed = 6.0f; // Speedは6で固定しておく。インスペクターから設定不可

        // 発射する回数分ループする
        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点


            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);

            shotSpeed = shotSpeed * 0.9f;
            shotAngle += 2;
        }

        shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
        //shotSpeed = 6.0f;

        for (int i = 0; i < n; i++)
        {
            // 真下90度が基点


            // インスタンス生成
            // 発射する弾を生成する
            var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, shotSpeed);

            shotSpeed = shotSpeed * 0.9f;
            shotAngle -= 2;
        }
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生
    }


    //レーザー。現状動かない敵にしか実装できない（敵がうごくとあたり判定ずれる）
    public void LaserBullet()
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
        var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle, laserSpeed);
        laserBullet.Add(shot);

    }

    //テスト（レーザー）
    public void LaserCast()
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
    public void PonderingBullet()
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
            var shot = Instantiate(EnemyBullet, pos, Quaternion.identity);
            //自機方向に飛ばす
            shotAngle = GetAngle(this.transform.position, ShootingPlayer.m_instance.transform.position);
            // BulletスクリプトのInitを呼び出す
            shot.Init(shotAngle, 4);

            deg += 360 / n;

        }
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生

    }

    // バレット追加枠
    public void RLBullet()
    {
        // 前フレームからの時間の差を加算
        timeCount += Time.deltaTime;

        // 1秒を超えているか
        if (timeCount <= shotInterval) return;

        timeCount = 0; // 再発射のために時間をリセット
        var changeAngle = 10.0f;
         // 発射角度を10度ずらす
        changeAngle *= -1.1f;
        if (shotAngle_under > 60 + 270) shotAngle_under = 270;
        else if (shotAngle_under < -60 + 270) shotAngle_under = 270;
        
        shotAngle_under += changeAngle;
        // インスタンス生成
        // 発射する弾を生成する
        var shot = Instantiate(EnemyBullet, this.transform.position, Quaternion.identity);

        // BulletスクリプトのInitを呼び出す
        shot.Init(shotAngle_under, shotSpeed);
        audioSource.PlayOneShot(BulletClip, 0.1f); // SE再生
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
        var BulletTag = other.gameObject.CompareTag("PlayerBullet");
        var BombTag = other.gameObject.CompareTag("BombRIKUGO");
        if (BulletTag)
        {
            // 弾のダメージを取得
            var damage = other.gameObject.GetComponent<Shot_Shikigami>().shotDamage;	
            //Debug.Log("EnemyHP"+EnemyHP);
            //Debug.Log("damage"+damage);
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

            EnemyKillProcess();

        }

        else if (BombTag)
        {
            // 弾のダメージを取得 ここはBombのダメージ
            var damage = 1;
            //Debug.Log("EnemyHP"+EnemyHP);
            //Debug.Log("damage"+damage);
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

            EnemyKillProcess();
        }


    }

    private void OnParticleCollision(GameObject other)
    {
        //Debug.Log("衝突");
        if (other.gameObject.CompareTag("BombTODA"))
        {
                        // 弾のダメージを取得 ここはBombのダメージ
            var damage = 20;
            //Debug.Log("EnemyHP"+EnemyHP);
            //Debug.Log("damage"+damage);
            HPBarManager.GaugeReduction(EnemyHPMax, EnemyHP, damage);//☆追加1014　HPバー減少
            EnemyHP -= damage;

            if (EnemyHP > 0) return; // 体力が残っている場合は処理スキップ
            Enemykilled = true;

            EnemyKillProcess();
        }
    }

    void EnemyKillProcess()
    {
            // 弾が当たった場所に爆発エフェクトを生成する
            Instantiate(
                ExtinguishPrefab,
                this.transform.localPosition,
                Quaternion.identity);

            //敵が死亡した場合はアイテムを散らばらせる
            var ItemN = ExpItemNum; // ExpItemNum個経験値アイテムドロップ

            while (0 < ItemN)
            {
                //経験値ドロップ
                ItemPrefabInit(ExpItem);
                ItemN -= 1;
            }

            //残機ドロップ
            if (OneUpDropOn == 1) ItemPrefabInit(OneUpItem);
            //ボムドロップ
            if (BombDropOn == 1) ItemPrefabInit(BombItem);
    }

    // ドロップアイテムの初期化処理
    public void ItemPrefabInit(GameObject ItemObject)
    {
        // 敵の位置にアイテムを生成する
        GameObject createObject = Instantiate(
            ItemObject, transform.position, Quaternion.identity);

        ItemDrop DropItemPrefab = createObject.GetComponent<ItemDrop>();

        // アイテム側の関数を初期化
        DropItemPrefab.ItemInit(GemSpeedMin, GemSpeedMax);
    }

    // 宝石が出現する時に初期化する関数
    public void MoveSwitchCase(Movement EnemyMovement)
    {
        switch (EnemyMovement)
        {
            case (Movement.StraightMove):
                EnemyStraightMove();
                break;
            case (Movement.ZMove):
                EnemyZMove();
                break;
            case (Movement.KOMove):
                EnemyKOMove();
                break;
            case (Movement.VMove):
                EnemyVMove();
                break;
            case (Movement.GoAndBackMove):
                EnemyGoAndBackMove();
                break;
            case (Movement.LMove):
                EnemyLMove();
                break;
            case (Movement.UMove):
                EnemyUMove();
                break;
            case (Movement.BendingMove):
                EnemyBendingMove();
                break;
            case (Movement.ToPlayerMove):
                EnemyToPlayerMove();
                break;
            case (Movement.CurveMove):
                EnemyCurveMove();
                break;
            case (Movement.SMove):
                EnemySMove();
                break;
            case (Movement.CircleMove):
                EnemyCircleMove();
                break;
            case (Movement.WaveMove):
                EnemyWaveMove();
                break;
            case (Movement.RLMove):
                EnemyRLMove();
                break;
            default:
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

    // Z字移動スクリプト
    public void EnemyZMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
        //オブジェクトからみて左斜め後ろに直進
        var m_direction2x = -0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2y = 0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * Speed * StraightMoveTime;
        //元の方向で直進
        var destination3 = destination2 + m_direction * Speed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // コの字移動スクリプト
    public void EnemyKOMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);

        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
        //左に直角で曲がる
        var m_direction2x = -m_direction.y;
        var m_direction2y = m_direction.x;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * Speed * StraightMoveTime;
        //左に直角で曲がる
        var destination3 = destination2 - m_direction * Speed * StraightOverTime;
        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // V字移動スクリプト
    public void EnemyVMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;

        //オブジェクトからみて左斜め後ろに直進
        var m_direction2x = -0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2y = 0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * Speed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));


    }

    // 往復移動スクリプト
    public void EnemyGoAndBackMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
        //逆方向に帰る
        var destination0 = destination1 - m_direction * Speed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination0, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));


    }

    // L字移動スクリプト
    public void EnemyLMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);

        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
        var m_direction2x = -m_direction.y;
        var m_direction2y = m_direction.x;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        //左に直角で曲がる
        var destination2 = destination1 + m_direction2 * Speed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // U字移動スクリプト
    public void EnemyUMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);

        //目的地の計算
        //直進
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
        //半円移動
        var m_direction2x = -m_direction.y;
        var m_direction2y = m_direction.x;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);

        var destination2 = destination1 + m_direction2 * Speed * StraightMoveTime;

        var destination12C = (destination1 + destination2) / 2; //1と2の中点計算
        var destination1_2 = destination12C + m_direction; // 単位ベクトルだけ位置ずらして中間地点設定

        //逆方向に直進
        var destination3 = destination2 - m_direction * Speed * StraightOverTime;

        Vector3[] path = {
            destination1_2,
            destination2,
        };

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOLocalPath(path, StraightMoveTime, PathType.CatmullRom).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // 鈍角V字移動スクリプト
    public void EnemyBendingMove()
    {
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        // 直進
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
        // 左斜め前に直進
        var m_direction2x = 0.707f * m_direction.x - 0.707f * m_direction.y;
        var m_direction2y = 0.707f * m_direction.x + 0.707f * m_direction.y;
        var m_direction2 = new Vector3(m_direction2x, m_direction2y, 0);
        var destination2 = destination1 + m_direction2 * Speed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));

    }

    // 自機方向移動型スクリプト
    public void EnemyToPlayerMove()
    {
        var sequence = DOTween.Sequence();
        // プレイヤーの現在地を取得する
        var playerPos = ShootingPlayer.m_instance.transform.position;
        var toPlayerdirection = playerPos - this.transform.position;
        toPlayerdirection.Normalize(); //正則化
        Debug.Log(toPlayerdirection);

        //目的地の計算
        //自機の方向へ直進
        var destination1 = this.transform.position + toPlayerdirection * Speed * StraightOverTime;

        sequence.Append(this.transform.DOMove(destination1, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));
    }

    // カーブ移動スクリプト
    public void EnemyCurveMove()
    {

        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        // 直進先の座標
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
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
        var destination2 = destination1 + m_direction2 * Speed * StraightOverTime;


        Vector3[] path = {
            destination0_2,
            destination1,
        };

        sequence.Append(this.transform.DOLocalPath(path, StraightMoveTime, PathType.CatmullRom).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightOverTime).SetLink(gameObject).SetEase(Ease.Linear));
    }

    // S字移動スクリプト
    public void EnemySMove()
    {

        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        // S字の上半分の移動
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime;
        var m_directionVerticalx1 = m_direction.y;
        var m_directionVerticaly1 = -m_direction.x;
        var m_directionVertical1 = new Vector3(m_directionVerticalx1, m_directionVerticaly1, 0);

        var destinationC = (this.transform.position + destination1) / 2;
        var distance01 = Vector3.Distance(this.transform.position, destination1);

        var destination0_2 = 0.5f * distance01 * m_directionVertical1 + destinationC;
        // S字の下半分の移動
        var destination2 = destination1 + m_direction * Speed * StraightMoveTime;
        var m_directionVerticalx2 = -m_direction.y;
        var m_directionVerticaly2 = m_direction.x;
        var m_directionVertical2 = new Vector3(m_directionVerticalx2, m_directionVerticaly2, 0);

        var destinationC2 = (destination1 + destination2) / 2;
        var distance02 = Vector3.Distance(destination1, destination2);

        var destination1_2 = 0.5f * distance02 * m_directionVertical2 + destinationC2;
        // 画面外まで直進
        var destination3 = destination2 + m_directionVertical1 * Speed * StraightOverTime;


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
    public void EnemyCircleMove()
    {

        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        //往復移動に中間地点を追加して円運動を実装
        var destination05 = this.transform.position + m_direction * Speed * StraightMoveTime / 2;
        var destination1 = destination05 + m_direction * Speed * StraightMoveTime;
        var m_directionVerticalx1 = m_direction.y;
        var m_directionVerticaly1 = -m_direction.x;
        var m_directionVertical1 = new Vector3(m_directionVerticalx1, m_directionVerticaly1, 0);

        var destinationC = (destination05 + destination1) / 2;
        var distance01 = Vector3.Distance(destination05, destination1);

        var destination0_2 = 0.5f * distance01 * m_directionVertical1 + destinationC;

        var destination1_2 = -0.5f * distance01 * m_directionVertical1 + destinationC;

        var destination2 = destination05 + m_direction * Speed * StraightOverTime;


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
    public void EnemyWaveMove()
    {
        //S字移動を3回繰り返す
        var sequence = DOTween.Sequence();
        m_direction.Normalize(); //正則化
        Debug.Log(m_direction);
        //目的地の計算
        var destination1 = this.transform.position + m_direction * Speed * StraightMoveTime / 4;
        var m_directionVerticalx1 = m_direction.y;
        var m_directionVerticaly1 = -m_direction.x;
        var m_directionVertical1 = new Vector3(m_directionVerticalx1, m_directionVerticaly1, 0);

        var destinationC = (this.transform.position + destination1) / 2;
        var distance01 = Vector3.Distance(this.transform.position, destination1);

        var destination0_2 = 1.41f * distance01 * m_directionVertical1 + destinationC;

        var destination2 = destination1 + m_direction * Speed * StraightMoveTime / 4;
        var destinationC2 = (destination1 + destination2) / 2;

        var destination1_2 = -1.41f * distance01 * m_directionVertical1 + destinationC2;

        var destination2_2 = 2 * distance01 * m_direction + destination0_2;

        var destination3 = destination2 + m_direction * Speed * StraightMoveTime / 4;

        var destination3_2 = 2 * distance01 * m_direction + destination1_2;

        var destination4 = destination3 + m_direction * Speed * StraightMoveTime / 4;

        var destination4_2 = 2 * distance01 * m_direction + destination2_2;

        var destination5 = destination4 + m_direction * Speed * StraightMoveTime / 4;

        var destination5_2 = 2 * distance01 * m_direction + destination3_2;

        var destination6 = destination5 + m_direction * Speed * StraightMoveTime / 4;

        var destination7 = destination6 + m_direction * Speed * StraightOverTime;

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
    public void EnemyRLMove()
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
        var destination2 = destination1 + m_direction2 * Speed * StraightMoveTime / 2;
        //左に直進
        var destination3 = destination2 - m_direction2 * Speed * StraightMoveTime;
        //右へ
        var destination4 = destination3 + m_direction2 * Speed * StraightMoveTime;

        sequence.Append(this.transform.DOMove(destination1, 1).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination2, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear))
                .Append(this.transform.DOMove(destination3, StraightMoveTime).SetLink(gameObject).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo));

    }


}