using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Stage
{
    stage1,
    stage2,
    stage3,
}

public class MapManager : MonoBehaviour
{
    Vector3 BasePoint;
    Vector3 BasePoint1 = new Vector3(-15.82f, -3.85f, 0.00f); // 初期座標
    Vector3 BasePoint2 = new Vector3(-17.29f, -3.19f, 0.00f);
    Vector3 BasePoint3 = new Vector3(-15.86f, -3.07f, 0.00f);

    // 中継地点
    Vector3[] Target;
    // 1面用
    Vector3[] Target1 = new Vector3[]
    {
        new Vector3(-15.54f, -3.02f, 0.00f),
        new Vector3(-17.18f, -1.26f, 0.00f),
        new Vector3(-15.79f, 0.01f, 0.00f),
        new Vector3(-16.05f, 1.46f, 0.00f),
        new Vector3(-16.94f, 2.77f, 0.00f)
    };
    // 2面用
    Vector3[] Target2 = new Vector3[]
    {
        new Vector3(-18.27f, -1.78f, 0.00f),
        new Vector3(-15.93f, 0.79f, 0.00f),
        new Vector3(-16.49f, 1.37f, 0.00f),
        new Vector3(-14.56f, 2.49f, 0.00f),
        new Vector3(-15.51f, 2.46f, 0.00f),
        new Vector3(-16.96f, 3.24f, 0.00f),
        new Vector3(-16.39f, 3.38f, 0.00f),
        new Vector3(-16.64f, 3.65f, 0.00f)
    };
    // 3面用
    Vector3[] Target3 = new Vector3[]
    {
        new Vector3(-14.84f, -0.82f, 0.00f),
        new Vector3(-16.02f, -0.16f, 0.00f),
        new Vector3(-15.63f, 0.33f, 0.00f),
        new Vector3(-14.89f, 0.54f, 0.00f),
        new Vector3(-14.41f, 0.70f, 0.00f),
        new Vector3(-15.21f, 0.77f, 0.00f),
        new Vector3(-14.57f, 0.95f, 0.00f),
        new Vector3(-15.64f, 1.10f, 0.00f),
        new Vector3(-15.90f, 2.26f, 0.00f),
        new Vector3(-17.26f, 2.34f, 0.00f),
        new Vector3(-16.43f, 3.52f, 0.00f)
    };

        
    public Stage stage;

    // 道中にかかる累計時間
    public float TimeStage1;

    int TargetCount;

    float speed;
    float distance;

    public bool miniMapStop = false;


    // Start is called before the first frame update
    void Start()
    {
        if (stage == Stage.stage1){
            BasePoint = BasePoint1;
            Target = Target1;
        }
        else if (stage == Stage.stage2){
            BasePoint = BasePoint2;
            Target = Target2;
        }
        else if (stage == Stage.stage3){
            BasePoint = BasePoint3;
            Target = Target3;
        }
        //BasePoint = new Vector3(-15.82f, -3.85f, 0.00f);

        distance = Vector3.Distance(BasePoint, Target[0]);
        for (int i = 0; i < Target.Length - 1; i++) 
        { 
            distance += Vector3.Distance(Target[i], Target[i + 1]); 
        }

        speed = distance / TimeStage1; // 累計距離と時間から速度を求める

        transform.localPosition = BasePoint;
        TargetCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetCount >= Target.Length) return;
        if (miniMapStop) return;

        for (int i = 0; i< Target.Length; i++)
        {

            if (TargetCount == i)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, Target[i], speed * Time.deltaTime);
            }
            if (transform.localPosition == Target[i])
            {
                TargetCount++;
            }

        }

    }

    public void IconStop(bool boolean)
    {
        miniMapStop = boolean;
    }
}
