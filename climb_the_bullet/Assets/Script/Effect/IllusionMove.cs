using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IllusionMove : MonoBehaviour
{
    //�ړ��̏��v����
    float moveTime = 1.5f;

    float speed;
    float distance;
    Vector3 Target;
    Vector3 BasePoint;

    // Start is called before the first frame update
    void Start()
    {
        Target = new Vector3(0.0f, -4.0f, 0.0f);
        BasePoint = new Vector3(0.0f, 4.0f, 0.0f);
        //�X�^�[�g����S�[���܂ł̑��������o��
        distance = Vector3.Distance(BasePoint, Target);
        //�����鎞�ԂƋ������瑬�����o��
        speed = distance / moveTime;
        //�ʒu�̏�����
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

    }
}
