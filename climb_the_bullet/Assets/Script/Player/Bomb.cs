using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//ボム機能に関するスクリプト（プレイヤーにアタッチ）
public class Bomb : MonoBehaviour
{
    float timeCount_Bomb = 0; // ボム経過時間
    bool bombMUTEKI = false; 
    public Shot_Bomb ShotPrefab_Bomb; // （ボム）弾のプレハブ
    public ParticleSystem ExplodePrefab_TODA; // （ボム）爆発エフェクトのプレハブ
    public BarrierParticleEffect BarrierPrefab_TENKO; // （ボム）爆発エフェクトのプレハブ
    public Shot_Bunshin ShotPrefab_TENKU1; // 分身プレハブ1
    public Shot_Bunshin ShotPrefab_TENKU2; // 分身プレハブ2
    public Shot_Bunshin ShotPrefab_TENKU3; // 分身プレハブ3
    public Shot_Bunshin ShotPrefab_TENKU4; // 分身プレハブ4
    //public ParticleSystem SakuraPrefab_RIKUGO; // （ボム）爆発エフェクトのプレハブ
    public RIKUGOBombCollision SakuraPrefab_RIKUGO1; // （ボム）爆発エフェクトのプレハブ
    public RIKUGOBombCollision SakuraPrefab_RIKUGO2; // （ボム）爆発エフェクトのプレハブ
    public RIKUGOBombCollision SakuraPrefab_RIKUGO3; // （ボム）爆発エフェクトのプレハブ
    public RIKUGOBombCollision SakuraPrefab_RIKUGO4; // （ボム）爆発エフェクトのプレハブ
    public RIKUGOBombCollision SakuraPrefab_RIKUGO5; // （ボム）爆発エフェクトのプレハブ

    //カットインプレハブ
    public GameObject CutinPrefab_TOUDA;
    public GameObject CutinPrefab_TENKO;
    public GameObject CutinPrefab_TENKU;
    public GameObject CutinPrefab_RIKUGO;


    public float ShotSpeed_Bomb; // （ボム）弾の移動の速さ
    public float ShotAngleRange_Bomb; // （ボム）複数の弾を発射する時の角度
    public float ShotTimer_Bomb; // （ボム）弾の発射タイミングを管理するタイマー
    public int ShotCount_Bomb; // （ボム）弾の発射数
    public float ShotInterval_Bomb; // （ボム）弾の発射間隔（秒

    public float ShotSpeed_Bomb_TENKU; // 分身の弾の移動の速さ
    public float ShotTimer_Bomb_TENKU; // 分身の弾の発射タイミングを管理するタイマー
    public int ShotCount_Bomb_TENKU; // 分身の弾の発射数
    public float ShotInterval_Bomb_TENKU; // 分身の弾の発射間隔（秒
    public float RIKUGOBombSpeed; // 速度

    const int BombBonus = 10000; // ボムのボーナス
    
    public int BunshinRandam;

    private GameInputs _gameInputs;

    // 残機アイコンとボムアイコンの管理スクリプト呼び出し
    [SerializeField]
    private IconUIManager1 BombIconManager;

    public int MUTEKIFlashCount = 50;

    const int BombMUTEKITime = 5;

    SpriteRenderer PlayerRenderer; // プレイヤーの色相等を変えるのに必要

    // Start is called before the first frame update
    void Start()
    {
        // Actionスクリプトのインスタンス生成
        _gameInputs = new GameInputs();

        // Actionイベント登録
        _gameInputs.Player.Bomb.performed += OnBomb;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
        _gameInputs.Enable();

        // プレイヤーのレンダー情報取得
        var PlayerObject = GameObject.Find("Player");
        PlayerRenderer = PlayerObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //ボム発射
        //Bomb_Trigger();

        //☆Bomb_Trigger内にあったのを出して加筆
        var PlayerObject = GameObject.Find("Player");
        var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();
        timeCount_Bomb += Time.deltaTime;
        Debug.Log("MUTEKI");
        if (timeCount_Bomb < BombMUTEKITime) return;
        Debug.Log("MUTEKITIMEKOETA time:" + timeCount_Bomb);
        if (!bombMUTEKI) return;
        Debug.Log("MUTEKIOWARI");
        
        shootingPlayerObject.state = STATE.NOMAL;
        timeCount_Bomb = 0;
        bombMUTEKI = false;

    }

    private void OnDestroy()
    {
        // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、
        // 必ずDisposeする必要がある
        _gameInputs?.Dispose();
    }

    private void OnBomb(InputAction.CallbackContext context)
    {
        if (Time.timeScale <= 0) return;//ポーズ中は呼び出せない

        var PlayerObject = GameObject.Find("Player");
        var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();
        if (shootingPlayerObject.state == STATE.NEUTRAL) return; //NEUTRAL時は呼び出せない（撃破後等）

        //ボム数が0なら処理を終える
        if (ShootingPlayer.PlayerBomb == 0) return;

        //ボムを一つ消費
        ShootingPlayer.PlayerBomb--;
        // ボム数更新
        BombIconManager.IconNumChange(ShootingPlayer.PlayerBomb);
        //経験値ボーナス
        shootingPlayerObject.AddExp(BombBonus);

        //SeitchCountを呼び出すために必要
        var ShikigamiObject = GameObject.Find("式神UI管理");
        var ShikigamiManagerObject = ShikigamiObject.GetComponent<ShikigamiManager>();
        //カウントが一定時間になるまで主人公を無敵に（時間は式神による）

        var ShikigamiMode = Mathf.Abs(ShootingPlayer.SikigamiMode) % 4;

        if (ShikigamiMode == 0)
        {
            Bomb_Shoot_TODA();
        }
        else if (ShikigamiMode == 1)
        {
            Bomb_Shoot_TENKO();
        }

        else if (ShikigamiMode == 2)
        {
            //ボム時演出（式神アイコンにパーティクル）
            //GetComponent<ParticleSystem>().Play();

            //ボム弾幕の関数　式神に応じて動きを変えたい
            Bomb_Shoot_TENKU(ShotSpeed_Bomb_TENKU, ShotCount_Bomb_TENKU);
        }

        else if (ShikigamiMode == 3)
        {
            Bomb_Shoot_RIKUGO();
        }
        timeCount_Bomb = 0;
        StartCoroutine(MUTEKIEffect());
        //stateをMUTEKIにする（点滅しながら動けるようになる）
        shootingPlayerObject.state = STATE.MUTEKI;
        bombMUTEKI = true;
    }
    
        IEnumerator MUTEKIEffect()
    {
        // 1ループに点灯と消灯を行うので1/2しておく
        float flashInterval = 0.5f * (float)BombMUTEKITime / (float)MUTEKIFlashCount;
        Debug.Log("flashInterval"+flashInterval);

        //色を白にする
        PlayerRenderer.color = Color.white;
        for (int i = 0; i < MUTEKIFlashCount; i++)
        {
            yield return new WaitForSeconds(flashInterval);
            PlayerRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            PlayerRenderer.enabled = true;
        }

        //色を白にする
        PlayerRenderer.color = Color.white;
    }

    private void Bomb_Shoot_TODA()
    {
        //パーティクルシステムのインスタンスを生成
        ParticleSystem ExplodeParticle = Instantiate(ExplodePrefab_TODA, this.transform.position, Quaternion.identity);

        //パーティクルの発生
        ExplodeParticle.Play();

        //カットインオブジェクトの出現
        GameObject Cutin = Instantiate(CutinPrefab_TOUDA) as GameObject;

        //ボム弾幕の関数　式神に応じて動きを変えたい
        //var angle = 90;
    }
    private void Bomb_Shoot_TENKO()
    {
        //パーティクルシステムのインスタンスを生成
        var BarrierPrefab = Instantiate(BarrierPrefab_TENKO, this.transform.position, Quaternion.identity);

        //パーティクルの発生
        //ExplodeParticle.Play();

        //カットインオブジェクトの出現
        GameObject Cutin = Instantiate(CutinPrefab_TENKO) as GameObject;
    }
    private void Bomb_Shoot_TENKU(float speed, int count)
    {
        var rot = transform.rotation; // プレイヤーの向き



        // 発射する回数分ループする
        for (int i = 0; i < count; ++i)
        {
            BunshinRandam = Random.Range(1, 5);
            //進行方向がランダム
            var angle = Random.Range(0, 360);

            //出現場所が枠内でランダム
            var pos = new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 3.0f), 0);
            // 弾の発射角度を計算する
            //var angle = angleBase + angleRange * ((float)i / (count - 1) - 0.5f);

            // 発射する弾を生成する
            if (BunshinRandam == 1)
            {
                var shot = Instantiate(ShotPrefab_TENKU1, pos, rot);
                // 弾を発射する方向と速さを設定する
                shot.Init_Bomb(angle, speed);
            }

            if (BunshinRandam == 2)
            {
                var shot = Instantiate(ShotPrefab_TENKU2, pos, rot);
                // 弾を発射する方向と速さを設定する
                shot.Init_Bomb(angle, speed);
            }

            if (BunshinRandam == 3)
            {
                var shot = Instantiate(ShotPrefab_TENKU3, pos, rot);
                // 弾を発射する方向と速さを設定する
                shot.Init_Bomb(angle, speed);
            }

            if (BunshinRandam == 4)
            {
                var shot = Instantiate(ShotPrefab_TENKU4, pos, rot);
                // 弾を発射する方向と速さを設定する
                shot.Init_Bomb(angle, speed);
            }



        }
        //カットインオブジェクトの出現
        GameObject Cutin = Instantiate(CutinPrefab_TENKU) as GameObject;
    }

    private void Bomb_Shoot_RIKUGO()
    {
        // 手前の敵を追尾、消滅したら別の敵を追尾したい
        // タグを使って画面上の全ての敵の情報を取得
        var targets = GameObject.FindGameObjectsWithTag("Enemy");
        // 要素数が0（敵がいない）なら早期リターン
        if (targets.Length == 0) return; 

        int[] BombAngleList = 
        {0,
        Random.Range (0, 90),
        Random.Range (90, 180),
        Random.Range (180, 270),
        Random.Range (270, 360)};

        RIKUGOBombCollision[] SakuraPrefabs = 
        {SakuraPrefab_RIKUGO1,
        SakuraPrefab_RIKUGO2,
        SakuraPrefab_RIKUGO3,
        SakuraPrefab_RIKUGO4,
        SakuraPrefab_RIKUGO5};

        //for文を使って各要素を出力
        var BombNum = 1;
        for(int j = 0; j < BombNum; j++){
            for(int i = 0; i < BombAngleList.Length; i++){
                // 発射する弾を生成する
                var bomb = Instantiate(SakuraPrefabs[i], targets[j].transform.position, Quaternion.identity);
                // 弾を発射する方向と速さを設定する
                bomb.Init(BombAngleList[i], RIKUGOBombSpeed);
            }
        }
        //カットインオブジェクトの出現
        GameObject Cutin = Instantiate(CutinPrefab_RIKUGO) as GameObject;
    }


}
