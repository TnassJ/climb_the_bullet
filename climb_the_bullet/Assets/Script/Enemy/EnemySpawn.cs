using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class EnemyTable : Serialize.TableBase<EnemyGroup, float, SamplePair>{


}

/// <summary>
/// ジェネリックを隠すために継承してしまう
/// [System.Serializable]を書くのを忘れない
/// </summary>
[System.Serializable]
public class SamplePair : Serialize.KeyAndValue<EnemyGroup, float>{

    public SamplePair (EnemyGroup key, float value) : base (key, value) {

    }
}

public class EnemySpawn : MonoBehaviour
{
    float timeCount = 0; // 経過時間

    

    private float Time0; //初期時間
    private float elapsedTime; // 経過時間を格納する変数
    


    [SerializeField]
    private List<EnemyTable> enemyTable = new List<EnemyTable>();

    [System.Serializable]
    class EnemyTable
    {
        public EnemyGroup EnemyPrefab;
        public float EnemyTime;
        public SpawnPositionList groupSpawnPosition;
        public int DirectionAngle;

    }
    public List<EnemyGroup> Enemylist = new List<EnemyGroup>{};
    public List<float> EnemyTimelist = new List<float> {};
    public List<SpawnPositionList> positionList = new List<SpawnPositionList>{};
    public List<int> angleList = new List<int> {};

    public EnemyTimer TimerForGroup = new EnemyTimer();//クラスのインスタンス生成

    public List<EnemyGroup> SpawnedEnemylist = new List<EnemyGroup>{};
    public bool SpawnEnd = false;
    bool SpawnLastEnemy; // 最後の敵がスポーンしたらtrueにする

    public Background background;

    int EnemyCount = 0;
    // Start is called before the first frame update
    void Start()
    {

        //foreach (KeyValuePair<EnemyGroup, float> pair in enemyTable.GetTable()) {
        //Debug.Log ("Key : " + pair.Key + "  Value : "  + pair.Value);
        Debug.Log ("enemyTable" + enemyTable);
        Debug.Log ("enemyTable.EnemyPrefab" + enemyTable[0].EnemyPrefab);
        for(int i = 0; i < enemyTable.Count; i++){
            Enemylist.Add(enemyTable[i].EnemyPrefab);
            EnemyTimelist.Add(enemyTable[i].EnemyTime);
            positionList.Add(enemyTable[i].groupSpawnPosition);
            angleList.Add(enemyTable[i].DirectionAngle);

        }

        TimerForGroup.init(EnemyTimelist, 0f);//初期化
        Time0 = Time.time;
        SpawnLastEnemy = false; 

    }
    
    // Update is called once per frame
    void Update()
    {
        elapsedTime = Time.time - Time0;

        if (TimerForGroup.check(elapsedTime)) 
        {
            var EnemyGroup = Instantiate( Enemylist[EnemyCount], new Vector3(0, 0, 0), Quaternion.identity );
            EnemyGroup.GroupInit(positionList[EnemyCount], angleList[EnemyCount]);
            //Debug.Log ("positionList[EnemyCount]" + positionList[EnemyCount]);
            //Debug.Log ("angleList[EnemyCount]" + angleList[EnemyCount]);
            EnemyCount += 1;
            SpawnedEnemylist.Add(EnemyGroup); // 生成した敵グループをリストに入れて管理

        }

        if (EnemyCount < enemyTable.Count) return; // 以下、中ボスの処理
        // 最後の敵（中ボス）がスポーンしたら背景を止めておく
        if (SpawnLastEnemy == false){
            background.StopBackground(); 
            SpawnLastEnemy = true;
        }
        // もし最後の敵（中ボス）が消滅したら次の処理を走らせるフラグを出す
        else{
            if (SpawnedEnemylist.Last() != null) return; // 中ボスが生きてるかどうか
            Debug.Log("中ボス消滅");
            SpawnEnd = true;
            background.RestartBackground();
            Destroy(gameObject); // 消しておかないとまた背景止まる
        }

        
    }
}