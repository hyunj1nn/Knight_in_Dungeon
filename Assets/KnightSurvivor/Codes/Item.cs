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
    public GameObject attackObjectPrefab;  // Add this line to reference the attack object prefab
    public GameObject lightningPrefab;

    public Gear gear;

    public Color normalColor;    // Default color
    public Color highlightColor; // Color when mouse is over

    // You need to have an Image component attached to the button
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

        image = GetComponent<Image>(); // Get the Image component
        normalColor = image.color; // Store the initial color
    }

    void OnEnable()
    {
        //Debug.Log("Level: " + level);

        textLevel.text = data.itemName;

        if (level == 0)
        {
            textBaseDesc.text = data.itemBaseDesc;
            //Debug.Log("Base Desc: " + textBaseDesc.text);
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
        else if (level < data.damages.Length) // �迭�� ������ Ȯ��
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
                    whipweapon = newWhipWeapon.AddComponent<WhipWeapon>(); // whipweapon ������ �Ҵ�
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
                    weapon.attackObject = attackObjectPrefab; // Uncomment this line to assign the attackObjectPrefab
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
                    bullet.attackObject = attackObjectPrefab; // Uncomment this line to assign the attackObjectPrefab
                    bullet.Init(data, data.projectileCounts[level]);
                    //bullet.Init(data);

                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = data.counts[level]; // This is the penetration count
                    int nextProjectileCount = data.projectileCounts[level];
                    //int nextCount = 0;

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
                    boomerang.attackObject = attackObjectPrefab; // Uncomment this line to assign the attackObjectPrefab
                    boomerang.Init(data, data.projectileCounts[level]);
                    //boomerang.Init(data);
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
                    thunder.attackObject = attackObjectPrefab; // Uncomment this line to assign the attackObjectPrefab
                    thunder.lightningPrefab = lightningPrefab;
                    thunder.Init(data, data.projectileCounts[level]);
                    //bullet.Init(data);

                }
                else if (level < data.damages.Length)
                {
                    float nextDamage = data.baseDamage;
                    int nextCount = data.counts[level]; // This is the penetration count
                    int nextProjectileCount = data.projectileCounts[level];
                    //int nextCount = 0;

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
                Character character = FindObjectOfType<Character>(); // �÷��̾� ��ü ã��
                if (character != null)
                {
                    character.Heal(200); // ü���� 200��ŭ ȸ���մϴ�.
                }
                break;
            case ItemData.ItemType.Armor:
                Character characterArmor = FindObjectOfType<Character>();
                if (characterArmor != null)
                {
                    characterArmor.IncreaseArmor(0.1f);  // ������ 0.1��ŭ ������ŵ�ϴ�.
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
        // This method is called when the mouse pointer is over the button
        image.color = highlightColor; // Change the color to the highlight color
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // This method is called when the mouse pointer leaves the button
        image.color = normalColor; // Change the color back to the normal color
    }
}