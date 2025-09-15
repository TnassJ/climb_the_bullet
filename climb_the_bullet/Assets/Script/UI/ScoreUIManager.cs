using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System; //DateTimeを使用する為追加。

public class ScoreUIManager : MonoBehaviour
{
    public TextMeshProUGUI m_levelText;// スコアのテキスト
    public TextMeshProUGUI m_levelText_Hiscore;// ハイスコアのテキスト
    public TextMeshProUGUI m_levelText_notification;// スコア通知のテキスト

    bool ScoreCalledOnce = false;//スコアは一回のみ更新
    List<int> score = new List<int>();//スコアランキングをリスト化
    List<string> scoreYMD = new List<string>();//スコアの日付をリスト化
    DateTime TodayNow;
    //ゲームオーバースクリプト呼び出し
    public Gameover Gameover1;

    // Start is called before the first frame update
    void Start()
    {
        //既存のスコアの値をすべてリストに挿入、score[0]がハイスコア
        for (int i = 0; i < 10; i++)
        {
            //第一引数にリストに入れるスコア、第二引数はスコアがない場合に入れる数値
            score.Add(PlayerPrefs.GetInt($"SCORE[{i}]", 0));
            scoreYMD.Add(PlayerPrefs.GetString($"SCOREYMD[{i}]", "YYYY/MM/DDFFF"));

        }
        Debug.Log("score=" + string.Join(" , ", score));
        m_levelText_Hiscore.text = score[0].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < 10; i++)
        //{
        //    Debug.Log($"score[{ i}] =" + score[i]);
        //}

        // プレイヤーのインスタンスを取得する
        //var player = ShootingPlayer.m_instance;
        // スコアのテキストの表示を更新する
        int scoreNow = ShootingPlayer.PlayerExpSum;
        m_levelText.text = scoreNow.ToString();

        //ゲームオーバー時にスコアを更新
        if (Gameover1.GameoverJudge == true && !ScoreCalledOnce)
        {
            //瞬間の日付を取得
            TodayNow = DateTime.Now;
            // スコアを保存
            //既存のハイスコアより高いとき
            if (score[0] < scoreNow)
            {
                                
                score.Insert(0, scoreNow);//0番目に現在のスコアを挿入
                score.RemoveAt(10);//10番目に押し出されたスコアを消去
                //日付についても同様
                scoreYMD.Insert(0, TodayNow.ToString());
                scoreYMD.RemoveAt(10);
                //すべてのスコアをデータにセットし直す
                for (int i = 0; i < 10; i++)
                {
                    //第一引数にセットする場所、第二引数にセットする数値
                    PlayerPrefs.SetInt($"SCORE[{i}]", score[i]);
                    PlayerPrefs.SetString($"SCOREYMD[{i}]", scoreYMD[i]);

                }
                PlayerPrefs.Save();//データにセットしたら保存する
                ScoreCalledOnce = true; //スコア更新は一度のみ
                                        //Debug.Log("ハイスコアは" + score[0]);
                                        //Debug.Log("Prefsの値は" + PlayerPrefs.GetInt("SCORE[0]", 0));
                m_levelText_notification.text = "HIGH SCORE!";

                return; //スコアの更新を行った場合、処理を終了

            }

            //既存のスコア中に収まっているとき（下位から比較）
            for (int i = 9; i > 0; i--)
            {
                //scoreNowがスコアi以上でスコアi-1より小さい　→スコアiに挿入
                if (score[i] < scoreNow && scoreNow <= score[i - 1])
                {

                    score.Insert(i, scoreNow);//0番目に現在のスコアを挿入
                    score.RemoveAt(10);//10番目に押し出されたスコアを消去

                    scoreYMD.Insert(i, TodayNow.ToString());//0番目に現在のスコアを挿入
                    scoreYMD.RemoveAt(10);//10番目に押し出されたスコアを消去

                    //すべてのスコアをデータにセットし直す
                    for (int j = 0; j < 10; j++)
                    {
                        PlayerPrefs.SetInt($"SCORE[{j}]", score[j]);
                        PlayerPrefs.SetString($"SCOREYMD[{j}]", scoreYMD[j]);

                    }
                    PlayerPrefs.Save();
                    ScoreCalledOnce = true;
                    m_levelText_notification.text = "RANK IN " + i.ToString() + "TH!";

                    return; //スコアの更新を行った場合、処理を終了
                }
            }

            //既存のどのスコアよりも低いとき
            if (scoreNow <= score[9])
            {
                ScoreCalledOnce = true;
                m_levelText_notification.text = "OUT OF RANKING";

                return;
            }


        }
    }
}
