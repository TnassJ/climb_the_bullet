using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager2 : MonoBehaviour
{
    //�}�b�v�}�[�J�[�̈ړ�
    public Vector3 BasePoint; //�����ʒu
    //�}�b�v�}�[�J�[���H��_
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

    //�X�e�[�W1�̏��v����
    public float TimeStage2;

    //�e�|�C���g�ɓ��B��������Z���邱�ƂŔ���
    public int TargetCount2;

    public float speed;
    public float distance;


    // Start is called before the first frame update
    void Start()
    {
        BasePoint = new Vector3(-17.29f, -3.19f, 0.00f);

        //�X�^�[�g����S�[���܂ł̑��������o��
        distance = Vector3.Distance(BasePoint, Target2[0]);
        for (int i = 0; i < Target2.Length - 1; i++)
        {
            distance += Vector3.Distance(Target2[i], Target2[i + 1]);
        }

        //�����鎞�ԂƋ������瑬�����o��
        speed = distance / TimeStage2;

        //�ʒu�̏�����
        transform.localPosition = BasePoint;
        TargetCount2 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (TargetCount2 >= 8) return;
        //movetoward�Ŋe�_�ړ�
        for (int i = 0; i < Target2.Length; i++)
        {

            if (TargetCount2 == i)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, Target2[i], speed * Time.deltaTime);


            }
            if (transform.localPosition == Target2[i])
            {
                TargetCount2++;
            }

        }

    }
}
