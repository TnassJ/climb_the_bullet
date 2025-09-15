// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// //ボム機能に関するスクリプト（プレイヤーにアタッチ）
// public class Bomb : MonoBehaviour
// {
//     float timeCount_Bomb = 0; // ボム経過時間
//     bool bombMUTEKI = false; 
//     public ParticleSystem ExplodePrefab_TODA; // （ボム）爆発エフェクトのプレハブ

//     public float ShotSpeed_Bomb; // （ボム）弾の移動の速さ
//     public float ShotAngleRange_Bomb; // （ボム）複数の弾を発射する時の角度
//     public float ShotTimer_Bomb; // （ボム）弾の発射タイミングを管理するタイマー
//     public int ShotCount_Bomb; // （ボム）弾の発射数
//     public float ShotInterval_Bomb; // （ボム）弾の発射間隔（秒

//     // 残機アイコンとボムアイコンの管理スクリプト呼び出し
//     [SerializeField]
//     private IconUIManager1 BombIconManager;

//     // Start is called before the first frame update
//     void Start()
//     {

//     }

//     // Update is called once per frame
//     void Update()
//     {
//         //ボム発射
//         Bomb_Trigger();

//         //☆Bomb_Trigger内にあったのを出して加筆
//         var PlayerObject = GameObject.Find("Player");
//         var shootingPlayerObject = PlayerObject.GetComponent<TutorialShootingPlayer>();
//         timeCount_Bomb += Time.deltaTime;
//         if (timeCount_Bomb < 5) return;
//         if (!bombMUTEKI) return;
        
//         shootingPlayerObject.state = STATE.NOMAL;
//         timeCount_Bomb = 0;
//         bombMUTEKI = false;

//     }


//     //ボムを呼び出す関数
//     private void Bomb_Trigger()
//     {
//         var PlayerObject = GameObject.Find("Player");
//         var shootingPlayerObject = PlayerObject.GetComponent<TutorialShootingPlayer>();

//         if (Input.GetKeyDown(KeyCode.X)) //TODO:連続で打てないようにする
//         {
//             //ボム数が0なら処理を終える
//             if (ShootingPlayer.PlayerBomb == 0) return;

//             //ボムを一つ消費
//             ShootingPlayer.PlayerBomb--;
//             // ボム数更新
//             BombIconManager.IconNumChange(ShootingPlayer.PlayerBomb);
//             //経験値ボーナス
//             shootingPlayerObject.AddExp(BombBonus); 
            
//             //SeitchCountを呼び出すために必要
//             var ShikigamiObject = GameObject.Find("式神UI管理");
//             var ShikigamiManagerObject = ShikigamiObject.GetComponent<ShikigamiManager>();
//             //カウントが一定時間になるまで主人公を無敵に（時間は式神による）
            
//             var ShikigamiMode = Mathf.Abs(ShikigamiManagerObject.SwitchCount) % 4;
//             if (ShikigamiMode == 0)
//             {
//                 Bomb_Shoot_TODA();
//             }
            

//         }
//     }

//     private void Bomb_Shoot_TODA()
//     {
//         //パーティクルシステムのインスタンスを生成
//         ParticleSystem ExplodeParticle = Instantiate(ExplodePrefab_TODA, this.transform.position, Quaternion.identity);

//         //パーティクルの発生
//         ExplodeParticle.Play();

//         //ボム弾幕の関数　式神に応じて動きを変えたい
//         //var angle = 90;
//     }

// }
