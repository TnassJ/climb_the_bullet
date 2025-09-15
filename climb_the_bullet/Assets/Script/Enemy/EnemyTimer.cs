using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 敵スポーン用のスケジュールタイマー
/*
public static class EnemyTimer
{
    public static List<float> schedule;
    public static int pt;
    public static float birth;
    public static bool check(float t)
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
    public static void init(List<float> schedule, float t)
    {
        this.schedule = schedule;
        this.pt = 0;
        this.birth = t;//クラスの生成時刻を入れる
    }
}
*/
