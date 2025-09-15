using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroup : MonoBehaviour
{
    
    public int enemyLine = 1; // 列数
    public int enemyNumByLine = 1; // 列ごとの敵の数
    public float enemyArrowBetween = 0.2f; // スポーン間隔
    public int allKillBonus = 2500; // 列ごとの敵の数

    public Movement Groupmovement;
    public Vector3 Groupdirection; // 進行方向 

    private float Time0; //初期時間
    private float elapsedTime; // 経過時間を格納する変数

    List<float> EnmeySpawnlist = new List<float> {};
    float lastSpownTime;
    public EnemyTimer TimerForGroup = new EnemyTimer();//クラスのインスタンス生成
    public EnemyMove EnemyPrefab; // 敵プレハブ
    public SpawnPositionList groupSpawnPosition;
    public Vector3 EnemySpawnPosition;
    private float XRightLimit = Config.XRightLimit;
    private float XLeftLimit = Config.XLeftLimit;
    private float XUpLimit = Config.XUpLimit;

    //出現敵を入れておくリスト
    private List<EnemyMove> EnemyGroupList = new List<EnemyMove>();
    bool BonusON = false; //ボーナスが入る際のフラグ

    // Start is called before the first frame update
    void Start()
    {

        var SpownTime = 0f;
        // enemyNumByLineの数だけ敵を発声させる。ただし一体enemyArrowBetweenごと遅延
        for (int i = 0; i < enemyNumByLine; i++ )
        {
            EnmeySpawnlist.Add(SpownTime); // 各敵のスポーン時間をリストに追加
            SpownTime += enemyArrowBetween;
        }
        

        TimerForGroup.init(EnmeySpawnlist, 0f); // タイマークラス初期化
        Time0 = Time.time;

        SpawnSwitchCase(groupSpawnPosition);
        //Debug.Log ("groupSpawnPosition" + groupSpawnPosition);

        BonusON = true;
        lastSpownTime = SpownTime; //最後に出る敵の時間
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - Time0;
        // 該当の経過時間に到達
        if (TimerForGroup.check(elapsedTime)) 
        {
            var FarstSpawnPosition = EnemySpawnPosition;
            // enemyLineだけ同時に敵をスポーン
            for (int i = 0; i < enemyLine; i++ )
            {
                // スポーン座標を下1にずらす
                FarstSpawnPosition += new Vector3(0, -1, 0);

                var Enemy = Instantiate( EnemyPrefab, FarstSpawnPosition, Quaternion.identity );
                Enemy.EnemyInit(Groupmovement, Groupdirection);
                EnemyGroupList.Add(Enemy);

            }
        }

        EnemyGroupList.RemoveAll(s => s == null);
        //Debug.Log ("EnemyGroupNum" + EnemyGroupList.Count);
        // 敵グループを全滅させたときにボーナス ボスがスポーンしきった後に判定
        if ((EnemyGroupList.Count == 0) && (elapsedTime > lastSpownTime)){
            // 敵が消滅したらこのオブジェクト自体を消しておく
            if (!BonusON) 
            {
                Destroy(gameObject);
                return;
            }
            var PlayerObject = GameObject.Find("Player");
            var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();
            shootingPlayerObject.AddExp(allKillBonus); //経験値ボーナス
            BonusON = false; //ONのままだと無限にボーナス加算されてしまう
        }
    }

    public void GroupInit(SpawnPositionList spawnposition, float angle)
    {
        groupSpawnPosition = spawnposition;
        Groupdirection = Utils.GetDirection(angle);
        Debug.Log ("Groupdirection" + Groupdirection);
    }


    void SpawnSwitchCase(SpawnPositionList spawnpositionList)
    {

        switch(spawnpositionList) 
        {
            case(SpawnPositionList.position_W6N0):
                EnemySpawnPosition = new Vector3(XLeftLimit, 0, 0);
                break;
            case(SpawnPositionList.position_W6N1):
                EnemySpawnPosition = new Vector3(XLeftLimit, 1, 0);
                break;
            case(SpawnPositionList.position_W6N2):
                EnemySpawnPosition = new Vector3(XLeftLimit, 2, 0);
                break;
            case(SpawnPositionList.position_W6N3):
                EnemySpawnPosition = new Vector3(XLeftLimit, 3, 0);
                break;
            case(SpawnPositionList.position_W6N4):
                EnemySpawnPosition = new Vector3(XLeftLimit, 4, 0);
                break;
            case(SpawnPositionList.position_W6N5):
                EnemySpawnPosition = new Vector3(XLeftLimit, 5, 0);
                break;
            case(SpawnPositionList.position_W6N6):
                EnemySpawnPosition = new Vector3(XLeftLimit, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_W5N6):
                EnemySpawnPosition = new Vector3(-5, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_W4N6):
                EnemySpawnPosition = new Vector3(-4, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_W3N6):
                EnemySpawnPosition = new Vector3(-3, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_W2N6):
                EnemySpawnPosition = new Vector3(-2, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_W1N6):
                EnemySpawnPosition = new Vector3(-1, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E0N6):
                EnemySpawnPosition = new Vector3(0, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E1N6):
                EnemySpawnPosition = new Vector3(1, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E2N6):
                EnemySpawnPosition = new Vector3(2, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E3N6):
                EnemySpawnPosition = new Vector3(3, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E4N6):
                EnemySpawnPosition = new Vector3(4, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E5N6):
                EnemySpawnPosition = new Vector3(5, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E6N6):
                EnemySpawnPosition = new Vector3(XRightLimit, XUpLimit, 0);
                break;
            case(SpawnPositionList.position_E6N5):
                EnemySpawnPosition = new Vector3(XRightLimit, 5, 0);
                break;
            case(SpawnPositionList.position_E6N4):
                EnemySpawnPosition = new Vector3(XRightLimit, 4, 0);
                break;
            case(SpawnPositionList.position_E6N3):
                EnemySpawnPosition = new Vector3(XRightLimit, 3, 0);
                break;
            case(SpawnPositionList.position_E6N2):
                EnemySpawnPosition = new Vector3(XRightLimit, 2, 0);
                break;
            case(SpawnPositionList.position_E6N1):
                EnemySpawnPosition = new Vector3(XRightLimit, 1, 0);
                break;
            case(SpawnPositionList.position_E6N0):
                EnemySpawnPosition = new Vector3(XRightLimit, 0, 0);
                break;

        }
    }
}

// 敵スポーン用のスケジュールタイマー
public class EnemyTimer
{
    public List<float> schedule;
    public int pt;
    public float birth;
    public bool check(float t)
    {
        if (this.pt >= this.schedule.Count) { return false; }
        if (t - this.birth >= this.schedule[pt])
        {
            this.pt++;
            return true;
        }
        return false;
    }
    //----------------------------
    //初期化
    public void init(List<float> schedule, float t)
    {
        this.schedule = schedule;
        this.pt = 0;
        this.birth = t;//クラスの生成時刻を入れる
    }
}
