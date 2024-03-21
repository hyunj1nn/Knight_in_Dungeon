using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ItemData data;
    public int level;
    public Weapon weapon;
    public WhipWeapon whipweapon;
    public Bullet bullet;
    public Boomerang boomerang;
    public SwordBullet swordbullet;
    public Thunder thunder;
    public GameObject rightWhipPrefab;
    public GameObject leftWhipPrefab;
    public GameObject attackObjectPrefab;  
    public GameObject lightningPrefab;

    public Gear gear;

    public Color normalColor;    
    public Color highlightColor; 

    private Image image;

    Image icon;
    Text textLevel;
    Text textName;
    Text textBaseDesc;
    Text textDesc;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1];
        icon.sprite = data.itemIcon;

        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textBaseDesc = texts[1];
        textDesc = texts[1];
        textName.text = data.itemName;

        image = GetComponent<Image>(); 
        normalColor = image.color;
    }

    void OnEnable()
    {
        textLevel.text = data.itemName;

        if (level == 0)
        {
            textBaseDesc.text = data.itemBaseDesc;
        }
        else
        {
            switch (data.itemType)
            {
                case ItemData.ItemType.Slash:
                case ItemData.ItemType.Rotation:
                case ItemData.ItemType.Range:
                case ItemData.ItemType.FlameBullet:
                case ItemData.ItemType.Boomerang:
                case ItemData.ItemType.Thunder:
                case ItemData.ItemType.Glove:
                case ItemData.ItemType.Shoe:
                case ItemData.ItemType.Heal:
                case ItemData.ItemType.Armor:
                    textDesc.text = string.Format(data.itemDesc.Replace("{0}%", "<color=#c20000>{0}%</color>"), data.damages[level] * 100, data.counts[level]);
                    // Debug.Log("Desc: " + textDesc.text);
                    break;

                default:
                    textDesc.text = string.Format(data.itemDesc);
                    // Debug.Log("Desc: " + textDesc.text);
                    break;
            }
        }
    }
    public void UpdateDescription()
    {
        textLevel.text = data.itemName;

        if (level == 0)
        {
            textBaseDesc.text = data.itemBaseDesc;
            Debug.Log("Base Desc: " + textBaseDesc.text);
        }
        else if (level < data.damages.Length) // 배열의 범위를 확인
        {
            switch (data.itemType)
            {
                case ItemData.ItemType.Slash:
                case ItemData.ItemType.Rotation:
                case ItemData.ItemType.Range:
                case ItemData.ItemType.FlameBullet:
                case ItemData.ItemType.Boomerang:
                case ItemData.ItemType.Thunder:
                case ItemData.ItemType.Glove:
                case ItemData.ItemType.Shoe:
                case ItemData.ItemType.Heal:
                case ItemData.ItemType.Armor:
                    textDesc.text = string.Format(data.itemDesc.Replace("{0}%", "<color=#c20000>{0}%</color>"), data.damages[level] * 100, data.counts[level]);
                    Debug.Log("Desc: " + textDesc.text);
                    break;

                default:
                    textDesc.text = string.Format(data.itemDesc);
                    Debug.Log("Desc: " + textDesc.text);
                    break;
            }
        }
    }

    public void Onclick()
    {
        switch (data.itemType)
        {
            case ItemData.ItemType.Slash:
                if (level == 0)
                {
                    GameObject newWhipWeapon = new GameObject();
                    whipweapon = newWhipWeapon.AddComponent<WhipWeapon>(); // whipweapon 변수에 할당
                    whipweapon.Init(data, rightWhipPrefab, leftWhipPrefab);
                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    for (int i = 0; i <= level; i++)
                    {
                        nextDamage = nextDamage * (1 + data.damages[i]);
                    }

                    nextCount += data.counts[level];

                    whipweapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;

            case ItemData.ItemType.Rotation:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.attackObject = attackObjectPrefab; 
                    weapon.Init(data);
                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;

                    for (int i = 0; i <= level; i++)
                    {
                        nextDamage = nextDamage * (1 + data.damages[i]);
                    }

                    nextCount += data.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;

            case ItemData.ItemType.Range:
                if (level == 0)
                {
                    GameObject newSwordBullet = new GameObject();
                    swordbullet = newSwordBullet.AddComponent<SwordBullet>();
                    swordbullet.Init(data, data.projectileCounts[level]);
                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;
                    int nextProjectileCount = data.projectileCounts[level];

                    for (int i = 0; i <= level; i++)
                    {
                        nextDamage = nextDamage * (1 + data.damages[i]);
                    }
                    nextCount += data.counts[level];

                    swordbullet.LevelUp(nextDamage, nextCount, nextProjectileCount);
                }
                level++;
                break;

            case ItemData.ItemType.FlameBullet:
                if (level == 0)
                {
                    GameObject newBullet = new GameObject();
                    bullet = newBullet.AddComponent<Bullet>();
                    bullet.attackObject = attackObjectPrefab; 
                    bullet.Init(data, data.projectileCounts[level]);
                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = data.counts[level];
                    int nextProjectileCount = data.projectileCounts[level];

                    for (int i = 0; i <= level; i++)
                    {
                        nextDamage = nextDamage * (1 + data.damages[i]);
                    }

                    nextCount += data.counts[level];

                    bullet.LevelUp(nextDamage, nextCount, nextProjectileCount);
                }
                level++;
                break;

            case ItemData.ItemType.Boomerang:
                if (level == 0)
                {
                    GameObject newBoomerang = new GameObject();
                    boomerang = newBoomerang.AddComponent<Boomerang>();
                    boomerang.attackObject = attackObjectPrefab; 
                    boomerang.Init(data, data.projectileCounts[level]);
                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = 0;
                    int nextProjectileCount = data.projectileCounts[level];

                    for (int i = 0; i <= level; i++)
                    {
                        nextDamage = nextDamage * (1 + data.damages[i]);
                    }

                    nextCount += data.counts[level];

                    boomerang.LevelUp(nextDamage, nextCount, nextProjectileCount);
                }
                level++;
                break;

            case ItemData.ItemType.Thunder:
                if (level == 0)
                {
                    GameObject newThunder = new GameObject();
                    thunder = newThunder.AddComponent<Thunder>();
                    thunder.attackObject = attackObjectPrefab; 
                    thunder.lightningPrefab = lightningPrefab;
                    thunder.Init(data, data.projectileCounts[level]);
                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = data.counts[level]; 
                    int nextProjectileCount = data.projectileCounts[level];

                    for (int i = 0; i <= level; i++)
                    {
                        nextDamage = nextDamage * (1 + data.damages[i]);
                    }

                    nextCount += data.counts[level];

                    thunder.LevelUp(nextDamage, nextCount, nextProjectileCount);
                }
                level++;
                break;

            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoe:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(data);
                }
                else
                {
                    float nextRate = data.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                Character character = FindObjectOfType<Character>(); // 플레이어 객체 찾기
                if (character != null)
                {
                    character.Heal(200); // 체력을 200만큼 회복
                }
                break;
            case ItemData.ItemType.Armor:
                Character characterArmor = FindObjectOfType<Character>();
                if (characterArmor != null)
                {
                    characterArmor.IncreaseArmor(0.1f);  // 방어력을 0.1만큼 증가
                }
                break;

        }

        if (level < data.damages.Length)
        {
            UpdateDescription();
        }

        if (level == data.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = highlightColor; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        image.color = normalColor; 
    }
}
