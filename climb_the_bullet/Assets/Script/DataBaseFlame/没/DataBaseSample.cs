using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アイテムの種類を表す列挙型
public enum ItemType
{
    Ball,   // ボール
    Heal    // 回復
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Item Properties")]
    public Sprite icon;        // アイテムのアイコン画像
    public ItemType itemType;  // アイテムの種類
    public string itemName;    // アイテムの名前
    [TextArea]
    public string description; // アイテムの説明
    public int effectValue;    // 効果値

    // ここに必要に応じてメソッドや追加のプロパティを定義できます
}
