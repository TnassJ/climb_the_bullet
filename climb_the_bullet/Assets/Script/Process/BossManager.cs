using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public BossMove bossObject;
    BossMove bossPrefab;
    public Vector3 spawnPosition = new Vector3(0, 4, 0);
    public GameObject bossDownProcess;
    public GameObject backGround;
    Animator backGroundAnimaor;

    // Start is called before the first frame update
    void Start()
    {
        bossPrefab = Instantiate(bossObject, spawnPosition, Quaternion.identity);
        backGroundAnimaor = backGround.AddComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (bossPrefab == null) return;

        var bossDown = bossPrefab.BossDead; // ボスを倒したかどうか
        if (bossDown)
        {
            Invoke("BossDownActive", 0.5f);
            backGroundAnimaor.enabled = false;
        }
    }

    void BossDownActive()
    {
        //PlayerをNEUTRAL（無入力状態）
        var PlayerObject = GameObject.Find("Player");
        var shootingPlayerObject = PlayerObject.GetComponent<ShootingPlayer>();
        shootingPlayerObject.state = STATE.NEUTRAL;

        GameObject[] enemyBullet = GameObject.FindGameObjectsWithTag("EnemyBullet");
        foreach (GameObject bullet in enemyBullet) 
        {
            Destroy(bullet);
        }
        GameObject[] playerBullet = GameObject.FindGameObjectsWithTag("PlayerBullet");
        foreach (GameObject bullet in playerBullet) 
        {
            Destroy(bullet);
        }
        bossDownProcess.SetActive(true);
    }
}
