using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptble Object/ItemData")]

public class ItemData : ScriptableObject
{
    public enum ItemType {  Slash, Rotation, Range, FlameBullet, Boomerang, Thunder, Glove, Shoe, Heal, Armor, Health} // 근접, 원거리, 공속, 이속, 힐 추후에 더 추가

    [Header("Main Info")]
    public ItemType itemType;
    public int itemId;
    public string itemName;
    [TextArea]
    public string itemBaseDesc;
    public string itemDesc;
    public Sprite itemIcon;

    [Header("Level Data")]
    public float baseDamage;
    public int baseCount;
    public float[] damages;
    public int[] counts;
    public int[] projectileCounts; // 레벨별 무기 갯수 증가


    [Header("Weapon")]
    public GameObject projectile;
    public float baseRange;

}
