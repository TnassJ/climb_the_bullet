using System.Collections;
using System.Collections.Generic;
using App.BaseSystem.DataStores.ScriptableObjects.Status;
using UnityEngine;

public class DataBaseTest : MonoBehaviour
{
    private StatusDataStore statusDataStore;

    private void Awake()
    {
        statusDataStore = FindObjectOfType<StatusDataStore>();
    }

    private void Start()
    {
        var foxData = statusDataStore.FindWithName("Fox"); // 名前が"Fox"であるデータを取得
        Debug.Log($"{foxData.Name} : (HP : {foxData.MaxHp})"); // データの出力
    }
}