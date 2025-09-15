using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public float defaultBackSpeed = 1.0f;
    float BackSpeed;
    bool stopON = false;
    public float slowTime = 1.0f;
    float elapsedTime;

    float elapsedTime_eff;
    public float effectCycleTime = 15.0f;
    public List<GameObject> effectList;
    int effectCount = 0;
    public MapManager mapManager;


    // Start is called before the first frame update
    void Start()
    {
        elapsedTime = 0.0f;
        elapsedTime_eff = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        // slowTime時間だけ加速or減速
        if (elapsedTime <= slowTime)
        {  
            if (stopON)
            {
                // Debug.Log("減速");
                BackSpeed = defaultBackSpeed * DecelerationFunc(elapsedTime); // 減速
                mapManager.IconStop(true);
            }
            else
            {
                BackSpeed = defaultBackSpeed * AccelerationFunc(elapsedTime); // 加速
            }
        }
        // slowTime時間経過後デフォ値に戻す
        else
        {
            if (stopON)
            {
                BackSpeed = 0.0f; 
            }
            else
            {
                BackSpeed = defaultBackSpeed;
                mapManager.IconStop(false);
            }
        }
        
        transform.position += -Vector3.up * BackSpeed * Time.deltaTime; // 背景移動
        
        if (BackSpeed < defaultBackSpeed) return;

        //Debug.Log("背景タイムカウント中" + elapsedTime_eff);
        elapsedTime_eff += Time.deltaTime;
        if (elapsedTime_eff < effectCycleTime) return;
        //Debug.Log("エフェクト発動");
        elapsedTime_eff = 0;
        GameObject effectObj = Instantiate (effectList[effectCount], new Vector3(0.0f,0.0f,0.0f), Quaternion.identity);
        Destroy(effectObj, effectCycleTime);
        effectCount ++;


        
    }

    // 中ボス出現時呼出
    public void StopBackground()
    {
        elapsedTime = 0.0f;
        stopON = true;
        
    }
    // 中ボス消滅時時呼出
    public void RestartBackground()
    {
        elapsedTime = 0.0f;
        stopON = false;
    }

    // 加速用関数
    float AccelerationFunc(float t)
    {
        return 1.0f - (t/slowTime - 1.0f) * (t/slowTime - 1.0f);
    }
    // 減速用関数
    float DecelerationFunc(float t)
    {
        return (t/slowTime - 1.0f) * (t/slowTime - 1.0f);
    }
}
