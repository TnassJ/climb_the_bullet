using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WayProcessManager : MonoBehaviour
{
    //チュートリアルフラグ管理変数
    
    bool StageWay1 = false;
    public GameObject enemyWayObject1; // 道中1
    EnemySpawn enemyWay1; // 道中1ソース

    bool StageWay2 = false;
    public GameObject enemyWayObject2; // 道中2
    EnemySpawn enemyWay2; // 道中2ソース

    public GameObject endLightUp; // 道中2
    public GameObject sceneChangeEffect; // 道中2

    public SceneObject m_nextScene; // シーン呼出

    void Start()
    {
        enemyWay1 = enemyWayObject1.GetComponent<EnemySpawn>(); // 道中1
        enemyWay2 = enemyWayObject2.GetComponent<EnemySpawn>(); // 道中2
        
        StageWay1 = true;
        enemyWayObject1.SetActive(StageWay1);
        
    }

    // Update is called once per frame
    void Update()
    {
        // 道中1が終わったら道中2を起動
        if (enemyWay1.SpawnEnd & StageWay2 == false){
            StageWay2 = true;
            enemyWayObject2.SetActive(StageWay2);
        }

        // ボス前の会話シーンに移行
        if (enemyWay2.SpawnEnd){
            endLightUp.SetActive(true);
            StartCoroutine("sceneChangeWait");
        }

    }
    
    IEnumerator sceneChangeWait()
    {
        yield return new WaitForSeconds(6.0f);
        sceneChangeEffect.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(m_nextScene);
    }

}
