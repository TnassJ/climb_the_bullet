using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionMove : MonoBehaviour
{
    //移動の所要時間
    float moveTime = 1.5f;

    float speed;
    float distance;
    Vector3 Target;
    Vector3 BasePoint;

    //一定時間経ったら消去
    float DisplayTime = 5.0f;
    float timeCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        Target = new Vector3(0.0f, -4.0f, 0.0f);
        BasePoint = new Vector3(0.0f, 4.0f, 0.0f);
        //スタートからゴールまでの総距離を出す
        distance = Vector3.Distance(BasePoint, Target);
        //かかる時間と距離から速さを出す
        speed = distance / moveTime;
        //位置の初期化
        transform.localPosition = BasePoint;
    }

    // Update is called once per frame
    void Update()
    {
        var distanceNow = Vector3.Distance(transform.localPosition, Target);
        if (distanceNow < 1.0f)
        {
            speed = speed * 0.96f;
        }
        transform.localPosition = Vector3.MoveTowards(transform.localPosition, Target, speed * Time.deltaTime);

        timeCount += Time.deltaTime;

        //一定時間経ったら消去
        if (timeCount > DisplayTime)
        {
            Destroy(gameObject);
        }
    }
}
