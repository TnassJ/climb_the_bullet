using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager3 : MonoBehaviour
{
    //マップマーカーの移動
    public Vector3 BasePoint; //初期位置
    //マップマーカーが辿る点
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

    //ステージ1の所要時間
    public float TimeStage3;

    //各ポイントに到達したら加算することで判別
    public int TargetCount3;

    public float speed;
    public float distance;


    // Start is called before the first frame update
    void Start()
    {
        BasePoint = new Vector3(-15.86f, -3.07f, 0.00f);

        //スタートからゴールまでの総距離を出す
        distance = Vector3.Distance(BasePoint, Target3[0]);
        for (int i = 0; i < Target3.Length - 1; i++)
        {
            distance += Vector3.Distance(Target3[i], Target3[i + 1]);
        }

        //かかる時間と距離から速さを出す
        speed = distance / TimeStage3;

        //位置の初期化
        transform.localPosition = BasePoint;
        TargetCount3 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetCount3 >= 11) return;
        //movetowardで各点移動
        for (int i = 0; i < Target3.Length; i++)
        {

            if (TargetCount3 == i)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, Target3[i], speed * Time.deltaTime);


            }
            if (transform.localPosition == Target3[i])
            {
                TargetCount3++;
            }

        }

    }
}
