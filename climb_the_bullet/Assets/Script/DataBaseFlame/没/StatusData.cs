using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace App.BaseSystem.DataStores.ScriptableObjects.Status
{
    /// <summary>
    /// ステータスを持つオブジェクトのデータ群 (対象: プレイヤー、敵、破壊可能オブジェクトなど)
    /// </summary>
    [CreateAssetMenu(menuName = "ScriptableObject/Data/Status")]
    public class StatusData : BaseData
    {
        public int MaxHp
        {
            get => maxhp;
            set => maxhp = value;
        }
        [SerializeField]
        private int maxhp;

        public float Speed
        {
            get => speed;
            set => speed = value;
        }
        [SerializeField]
        private float speed;

        public float StraightTime
        {
            get => straightTime;
            set => straightTime = value;
        }
        [SerializeField]
        private float straightTime;

        public int Exp
        {
            get => exp;
            set => exp = value;
        }
        [SerializeField]
        private int exp;

        public int LifeDrop
        {
            get => lifedrop;
            set => lifedrop = value; // 0 <= hp <= maxhp
        }

        [SerializeField]
        private int lifedrop;

        public int BombDrop
        {
            get => bombdrop;
            set => bombdrop = value; // 0 <= hp <= maxhp
        }

        [SerializeField]
        private int bombdrop;


    }
}
