using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//using Cysharp.Threading.Tasks;
public enum STATE
{
    NOMAL,
    DAMAGED,
    MUTEKI,
    NEUTRAL
}

public class ShootingPlayer : MonoBehaviour
{
    [SerializeField]
    float PlayerSpeed = 0.1f;

    public int PlayerHP0 = 2; // HP の初期値
    public int PlayerHPMAX = 8; // HP の最大値
    public static int PlayerHP; // HP

    public int PlayerBomb0 = 2; // ボム の初期値
    public int PlayerBombMAX = 8; // ボム の最大値
    public static int PlayerBomb; // ボム数

    public static int SikigamiMode = 0;
    public float equipFromPlayer = 0.5f; // 弾幕発射体とプレイヤーとの間隔

    //☆追加
    public GameObject option_TODA;
    GameObject TODA_R;
    GameObject TODA_L;
    public GameObject option_TENKO;
    GameObject TENKO_R;
    GameObject TENKO_L;
    public GameObject option_TENKU;
    GameObject TENKU_R;
    GameObject TENKU_L;
    public GameObject option_RIKUGO;
    GameObject RIKUGO_R;
    GameObject RIKUGO_L;
    // public GameObject BlueEffect;
    // public GameObject YellowEffect;
    // public GameObject GreenEffect;
    // public GameObject RedEffect;

    // プレイヤーのインスタンスを管理する static 変数
    public static ShootingPlayer m_instance;

    public static int Playerlevel = 0; // レベル
    int maxLevel = 3; // レベルの最大
    public static int PlayerExpSum = 0; // 経験値 （スコアとして表示。ScoreUIManagerで参照しているので注意）
    public static int PlayerExpSum_first = 0; // シーン開始時の経験値
    [SerializeField]
    private List<int> NextExpData; // n+2レベルまでに必要な経験値(最初はレベル2)
    int NextExp; // 次のレベルに必要な経験値
    int EnemyKillNum = 0; // 敵を倒した数

    // 残機アイコンとボムアイコンの管理スクリプト呼び出し
    public IconUIManager1 LifeIconManager;
    public IconUIManager1 BombIconManager;

    public GameObject LifeDownIcon; // 被弾時に出る身代わり式神
    public GameObject MagicCircle; // 被弾時に出る魔法陣
    public AudioClip DamageSE; //被弾したときに再生するSE
    public float flashInterval = 0.1f; // 被弾したときの無敵点滅の間隔
    public int loopCount = 30; // 被弾後の無敵点滅回数
    SpriteRenderer sp; // プレイヤーの色相等を変えるのに必要
    public STATE state; // 無敵かどうか

    //☆20241117追加し直し　ゲームオーバースクリプト呼び出し
    public Gameover Gameover1;

    private GameInputs _gameInputs;
    float slowSpeedRatio = 1.0f;
    private Vector3 _moveInputValue;
    bool OnSlow = false;

    void Start()
    {
        Debug.Log("主人公初期化");
        // 他のクラスからプレイヤーを参照できるように
        // static 変数にインスタンス情報を格納する
        sp = GetComponent<SpriteRenderer>();

        m_instance = this;
        if (PlayerHP == 0) PlayerHP = PlayerHP0; // HP
        if (PlayerBomb == 0) PlayerBomb = PlayerBomb0; // HP
        PlayerExpSum_first = PlayerExpSum;

        //☆20241117追加し直し　ゲームオーバー初期化
        Gameover1.CallGameover(false);


        //Playerlevel = 0; // レベル
        //Debug.Log(Playerlevel);
        NextExp = NextExpData[0]; // レベルアップ経験値の設定
        
        // 式神有効化
        // 射出勾玉の左右座標を算出
        var Position_R = new Vector3(equipFromPlayer, 0, 0);
        var Position_L = new Vector3(-equipFromPlayer, 0, 0);

        // 右と左でそれぞれのプレハブ生成
        TODA_R = SikigamiInst(option_TODA, Position_R);
        TODA_L = SikigamiInst(option_TODA, Position_L);
        TENKO_R = SikigamiInst(option_TENKO, Position_R);
        TENKO_L = SikigamiInst(option_TENKO, Position_L);
        TENKU_R = SikigamiInst(option_TENKU, Position_R);
        TENKU_L = SikigamiInst(option_TENKU, Position_L);
        RIKUGO_R = SikigamiInst(option_RIKUGO, Position_R);
        RIKUGO_L = SikigamiInst(option_RIKUGO, Position_L);

        OptionChange(); // 式神機能切替
        ShotUpdate();

        // 残機とボムをアイコンに反映
        LifeIconManager.IconNumChange(PlayerHP);
        BombIconManager.IconNumChange(PlayerBomb);

        // Actionスクリプトのインスタンス生成
        _gameInputs = new GameInputs();
        // Actionイベント登録
        //_gameInputs.Player.Move.started += OnMove; startがあると押しっぱなしの挙動が変になる
        _gameInputs.Player.Move.performed += OnMove;
        _gameInputs.Player.Move.canceled += OnMove;
        _gameInputs.Player.ShikigamiChangeLeft.performed += OnShikigamiChangeLeft;
        _gameInputs.Player.ShikigamiChangeRight.performed += OnShikigamiChangeRight;
        _gameInputs.Player.Slow.performed += PressShift;
        _gameInputs.Player.Slow.canceled += PressShift;

        // Input Actionを機能させるためには、
        // 有効化する必要がある
        _gameInputs.Enable();
    }

    void Update()
    {
        if (state == STATE.NEUTRAL)
        {
            SikigamiDisactive();
            return;
        }

        // プレイヤーが画面外に出ないように位置を制限する
        transform.position = Utils.ClampPosition(transform.position);
    }

    private void OnDestroy()
    {
        // 自身でインスタンス化したActionクラスはIDisposableを実装しているので、
        // 必ずDisposeする必要がある
        _gameInputs?.Dispose();
    }

    // 射出勾玉を子オブジェクトとして生成する関数
    GameObject SikigamiInst(GameObject prefab, Vector3 position)
    {
        GameObject childObject = Instantiate(prefab, this.transform);

        childObject.transform.localPosition = position;
        childObject.transform.localRotation = Quaternion.identity;
        //childObject.transform.localScale = new Vector3(1, 1, 1);
        return childObject;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if (Time.timeScale <= 0) return;//ポーズ中は呼び出せない

        //追記　ステートがダメージならリターン
        if (state == STATE.DAMAGED) return;

        // Moveアクションの入力取得
        _moveInputValue = context.ReadValue<Vector2>();

    }

    // 押された瞬間のコールバック
    public void PressShift(InputAction.CallbackContext context)
    {
        if (Time.timeScale <= 0) return;//ポーズ中は呼び出せない

        // 押された瞬間でPerformedとなる
        if (context.performed) OnSlow = true;

        else OnSlow = false;
    }

    private void FixedUpdate()
    {
        if (state == STATE.DAMAGED) return;
        // 移動方向のベクトルに移動量をかけて座標に加算
        if (!OnSlow) slowSpeedRatio = 1.0f;

        else slowSpeedRatio = 0.5f;

        transform.position += new Vector3(
            _moveInputValue.x,
            _moveInputValue.y,
            0
        ) * PlayerSpeed * Time.deltaTime * slowSpeedRatio;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (state == STATE.NEUTRAL) return;

        // 当たったのが敵の弾
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            Debug.Log("BulletHit");
            // 弾も消す
            Destroy(other.gameObject);
            Damage(1); //残機減らす
        }

        // 残機アイテムを拾ったとき
        else if (other.gameObject.CompareTag("LifeItem"))
        {
            // 残機を1個増やす
            PlayerHP++;
            if (PlayerHP > PlayerHPMAX) PlayerHP = PlayerHPMAX; //Max超えない調整
            // 残機更新
            LifeIconManager.IconNumChange(PlayerHP);
        }

        // ボムアイテムを拾ったとき
        else if (other.gameObject.CompareTag("BombItem"))
        {
            //ボム数1個増加
            PlayerBomb++;
            if (PlayerBomb > PlayerBombMAX) PlayerBomb = PlayerBombMAX; //Max超えない調整
            // ボム数更新
            BombIconManager.IconNumChange(PlayerBomb);
        }


    }

    // ダメージを受ける関数
    // 敵とぶつかった時に呼び出される
    public void Damage(int damage)
    {
        //Debug.Log("state:" + state);
        //変更　ノーマルじゃない（ダメージ中、無敵中）ときはリターン
        if (state != STATE.NOMAL) return;

        state = STATE.DAMAGED;
        
        // HP を減らす
        PlayerHP -= damage;
        LifeIconManager.IconNumChange(PlayerHP);            
        //ボム数1個増加
        PlayerBomb++;
        if (PlayerBomb > PlayerBombMAX) PlayerBomb = PlayerBombMAX; //Max超えない調整
        // ボム数更新
        BombIconManager.IconNumChange(PlayerBomb);
        
        EnemyKillNum = 0; // 敵を倒した数をリセット

        // HP がまだある場合、ここで処理を終える
        if (0 <= PlayerHP)
        {
            DamageAnimation();
            // SE を再生する
            var audioSource = GameObject.Find("AudioSource(SE)").GetComponent<AudioSource>();
            audioSource.PlayOneShot(DamageSE);
            Debug.Log("Hit");
            StartCoroutine(HitedEffect());
            return;
        }


        // プレイヤーが死亡したので非表示にする
        // 本来であれば、ここでゲームオーバー演出を再生したりする
        PlayerHP = 0;
        PlayerBomb = 0;
        Gameover1.CallGameover(true);
        gameObject.SetActive(false);

    }

    // 敵とぶつかった時に呼び出される身代わりアニメーション
    public void DamageAnimation()
    {
        //Animationオブジェクトの生成
        var lifeDownIns = Instantiate(LifeDownIcon, this.transform.position, Quaternion.identity);
        var magicCircleIns = Instantiate(MagicCircle, this.transform.position, Quaternion.identity);
    }

    // スコア倍率変動
    public float AddScoreRatio()
    {

        float ratio = 1.0f + EnemyKillNum * 0.01f; // 敵倒すごとに0.01加算
        Debug.Log("ratio:" + ratio);
        return ratio;
    }

    // 経験値を増やす関数
    // 宝石を取得した時に呼び出される
    public void AddExp(int exp)
    {
        var bonusScoreRatio = AddScoreRatio();
        // 経験値を増やすcv
        PlayerExpSum += (int)(bonusScoreRatio * exp);
        Debug.Log("PlayerExpSum:" + PlayerExpSum);

        // まだレベルアップに必要な経験値に足りていない場合、ここで処理を終える
        if (PlayerExpSum < NextExp) return;
        if (Playerlevel >= maxLevel) return; // レベル上限を超えたらレベルアップ等スキップ

        // レベルアップする
        Playerlevel++;
        NextExp = NextExpData[Playerlevel]; // 必要経験値更新
        ShotUpdate(); // ショット更新
    }

    void ShotUpdate()
    {
        Debug.Log("Playerlevel" + Playerlevel);

        // レベルごとにショットのパラメータ変更

        if (Playerlevel <= -1 || Playerlevel >= 4) return;
        
        // 騰蛇のパラメータ更新
        var TODA_r = TODA_R.GetComponent<ShootingShikigami_TODA>();
        var TODA_l = TODA_L.GetComponent<ShootingShikigami_TODA>();
        TODA_r.ParameterUpdate(Playerlevel);
        TODA_l.ParameterUpdate(Playerlevel);
        
        // 天后のパラメータ更新

        var TENKO_r = TENKO_R.GetComponent<ShootingShikigami_TENKO>();
        var TENKO_l = TENKO_L.GetComponent<ShootingShikigami_TENKO>();
        TENKO_r.ParameterUpdate(Playerlevel);
        TENKO_l.ParameterUpdate(Playerlevel);

        // 天空のパラメータ更新
        var TENKU_r = TENKU_R.GetComponent<ShootingShikigami_TENKU>();
        var TENKU_l = TENKU_L.GetComponent<ShootingShikigami_TENKU>();
        TENKU_r.ParameterUpdate(Playerlevel);
        TENKU_l.ParameterUpdate(Playerlevel);

        // 六合のパラメータ更新
        var RIKUGO_r = RIKUGO_R.GetComponent<ShootingShikigami_RIKUGO>();
        var RIKUGO_l = RIKUGO_L.GetComponent<ShootingShikigami_RIKUGO>();
        RIKUGO_r.ParameterUpdate(Playerlevel);
        RIKUGO_l.ParameterUpdate(Playerlevel);

    }

    public void AddKillEnemy( int num )
    {
        EnemyKillNum += num;
    }
    
    IEnumerator HitedEffect()
    {
        //色を白にする
        sp.color = Color.white;

        transform.position = new Vector3(0, -5, 0);
        //sp.color += new Color(0, 0, 0, 255);
        sp.enabled = false;
        yield return new WaitForSeconds(0.5f);
        //sp.color -= new Color(0, 0, 0, 255);
        sp.enabled = true;
        for (int i = 0; i < loopCount; i++)
        {
            yield return new WaitForSeconds(flashInterval);
            sp.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            sp.enabled = true;

            if (i > 1)
            {
                //stateをMUTEKIにする（点滅しながら動けるようになる）
                state = STATE.MUTEKI;
                //色を緑にする
                sp.color = Color.white;
            }
        }

        //ループが抜けたらstateをNOMALにする
        state = STATE.NOMAL;
        //色を白にする
        sp.color = Color.white;
    }

    private void OnShikigamiChangeLeft(InputAction.CallbackContext context)
    {
        if (Time.timeScale <= 0) return;//ポーズ中は呼び出せない

        SikigamiMode = (SikigamiMode + 1) % 4;
        OptionChange();
    }
    private void OnShikigamiChangeRight(InputAction.CallbackContext context)
    {
        if (Time.timeScale <= 0) return;//ポーズ中は呼び出せない

        SikigamiMode += 4; // 4を足してマイナス回避
                           // 4で割った余りで算出
        SikigamiMode = (SikigamiMode - 1) % 4;
        OptionChange();
    }

    // 式神変更に関わるタグを全て無効化する
    void SikigamiDisactive()
    {
        var targets = GameObject.FindGameObjectsWithTag("SikigamiChange");
        foreach (GameObject target in targets) {
            // 無効にしておく
            target.SetActive(false);
        }
    }

    //☆SwitchCountに合わせてオプションも変更
    public void OptionChange()
    {
        SikigamiDisactive(); // 最初に式神を全て無効化
        if (SikigamiMode == 0)
        {
            TODA_R.SetActive(true);
            TODA_L.SetActive(true);
        }
        else if (SikigamiMode == 1)
        {
            TENKO_R.SetActive(true);
            TENKO_L.SetActive(true);
        }
        else if (SikigamiMode == 2)
        {
            TENKU_R.SetActive(true);
            TENKU_L.SetActive(true);
        }
        else if (SikigamiMode == 3)
        {
            RIKUGO_R.SetActive(true);
            RIKUGO_L.SetActive(true);
        }

    }

}
