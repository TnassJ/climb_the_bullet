using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;

public class TutorialManager : MonoBehaviour
{
    public TutorialMove tutorialEnemy;
    //TutorialMove tutorialEnemyPrefab;
    public TutorialShootingPlayer tutorialPlayer;

    public Vector3 spawnPosition = new Vector3(0, 4, 0);

    public Fungus.Flowchart flowChart;

    int processNum = 0;

    bool dialogueEnd0 = false;
    bool dialogueEnd1 = false;
    bool dialogueEnd2 = false;
    bool dialogueEnd3 = false;
    bool dialogueEnd4 = false;
    bool dialogueEnd5 = false;
    bool dialogueEnd6 = false;

    bool dialogueIn = false;
    // Start is called before the first frame update
    void Start()
    {
        //tutorialEnemyPrefab = Instantiate(tutorialEnemy, spawnPosition, Quaternion.identity);
        StartCoroutine("Movewait");
        processNum ++;

    }

    // Update is called once per frame
    void Update()
    {
        // fungusからフラグ値獲得
        BoolValGet();

        // 会話中は他のオブジェクトの動きを止める
        if (dialogueIn) DialogueInProcess();
        else DialogueOutProcess();

        Debug.Log("dialogueEnd0" + dialogueEnd0);

        // 最初の会話が終わったら敵のショットを有効にする
        if (dialogueEnd0)
        {
            //flowChart.SendFungusMessage("FirstDialogueON");
            tutorialEnemy.EasyShot = true;
            flowChart.SetBooleanVariable("DialogueEnd0", false);
            StartCoroutine("Shotwait");
        }

        // プレイヤーがダメージを受けたらセリフを流す
        if (tutorialPlayer.firstDamageCheck)
        {
            flowChart.SendFungusMessage("HitON");
            //processNum ++;
            tutorialPlayer.firstDamageCheck = false;
        }

        // 最初の会話が終わったら敵のショットを有効にする
        if (dialogueEnd2)
        {
            
            //flowChart.SendFungusMessage("FirstDialogueON");
            tutorialEnemy.DenceShot = true;
            flowChart.SetBooleanVariable("DialogueEnd2", false);
            StartCoroutine("DenseShotwait");
        }

        // プレイヤーがボムを撃ったらセリフを流す
        if (tutorialPlayer.firstBombCheck)
        {
            //flowChart.SendFungusMessage("BombON");
            StartCoroutine("BombShotwait");
            //processNum ++;
            tutorialPlayer.firstBombCheck = false;
            tutorialPlayer.changeLock = false;
        }

        // 式神切替で天后呼出
        if (tutorialPlayer.shotChange1)
        {
            //tutorialPlayer.shotLock = true;
            StartCoroutine("TENKOwait");
            tutorialPlayer.shotChange1 = false;
        }
        // 式神切替で天空呼出
        if (tutorialPlayer.shotChange2)
        {
            //tutorialPlayer.shotLock = true;
            StartCoroutine("TENKUwait");
            tutorialPlayer.shotChange2 = false;
        }
        // 式神切替で六合呼出
        if (tutorialPlayer.shotChange3)
        {
            //tutorialPlayer.shotLock = true;
            StartCoroutine("RIKUGOwait");
            tutorialPlayer.shotChange3 = false;
        }
    }

    IEnumerator Movewait()
    {
        yield return new WaitForSeconds(1.0f);
        flowChart.SendFungusMessage("startON");
        //遅らせたい処理
    }

    IEnumerator Shotwait()
    {
        yield return new WaitForSeconds(2.0f);
        
        flowChart.SendFungusMessage("FirstShotON");
    }

    IEnumerator DenseShotwait()
    {
        yield return new WaitForSeconds(2.0f);
        
        flowChart.SendFungusMessage("DenseShotON");
        tutorialPlayer.bombLock = false;
    }
    IEnumerator BombShotwait()
    {
        yield return new WaitForSeconds(5.0f);
        flowChart.SendFungusMessage("BombON");
        tutorialPlayer.changeLock = false;
        tutorialPlayer.shotLock = false;
        tutorialPlayer.shotChange0 = true;
    }

    IEnumerator TENKOwait()
    {
        yield return new WaitForSeconds(1.0f);
        flowChart.SendFungusMessage("TENKOON");
        tutorialPlayer.changeLock = false;
    }

    IEnumerator TENKUwait()
    {
        yield return new WaitForSeconds(1.0f);
        flowChart.SendFungusMessage("TENKUON");
        tutorialPlayer.changeLock = false;
    }

    IEnumerator RIKUGOwait()
    {
        yield return new WaitForSeconds(1.0f);
        flowChart.SendFungusMessage("RIKUGOON");
        tutorialPlayer.changeLock = false;
    }

    void BoolValGet()
    {
        dialogueEnd0 = flowChart.GetBooleanVariable("DialogueEnd0");
        dialogueEnd1 = flowChart.GetBooleanVariable("DialogueEnd1");
        dialogueEnd2 = flowChart.GetBooleanVariable("DialogueEnd2");
        dialogueEnd3 = flowChart.GetBooleanVariable("DialogueEnd3");
        dialogueEnd4 = flowChart.GetBooleanVariable("DialogueEnd4");
        dialogueEnd5 = flowChart.GetBooleanVariable("DialogueEnd5");
        dialogueEnd6 = flowChart.GetBooleanVariable("DialogueEnd6");
        dialogueIn = flowChart.GetBooleanVariable("DialogueIn");
    }

    // 会話中の処理_時間止める
    void DialogueInProcess()
    {
        TutorialMove.EnemyPouse = true;
        TargetPauser.Pause();

    }
    // 会話中の処理_時間動かす
    void DialogueOutProcess()
    {
        TargetPauser.Resume();
        TutorialMove.EnemyPouse = false;
        
    }
}
