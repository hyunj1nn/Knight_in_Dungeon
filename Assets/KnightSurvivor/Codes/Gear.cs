using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        name = "Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();

    }

    public void LevelUp(float additionalRate)
    {
        this.rate += additionalRate;
        ApplyGear();
    }

    void ApplyGear()
    {
        switch (type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
        }
    }

    void RateUp()
    {
        WhipWeapon[] whipweapons = transform.parent.GetComponentsInChildren<WhipWeapon>();

        foreach (WhipWeapon whipWeapon in whipweapons)
        {
            switch (whipWeapon.id)
            {
                default:
                    whipWeapon.timeToAttack = 2.0f * (1f - rate);
                    break;
            }
        }

        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();

        foreach (Weapon Weapon in weapons)
        {
            switch (Weapon.id)
            {
                default:
                    Weapon.timeToAttack = 1.0f * (1f - rate);
                    break;
            }
        }

        Bullet[] bullets = transform.parent.GetComponentsInChildren<Bullet>();

        foreach (Bullet bullet in bullets)
        {
            switch(bullet.id)
            {
                default:
                    bullet.speed = 2.0f * (1f - rate);
                    break;
            }
        }

        SwordBullet[] swordbullets = transform.parent.GetComponentsInChildren<SwordBullet>();

        foreach (SwordBullet swordbullet in swordbullets)
        {
            switch (swordbullet.id)
            {
                default:
                    swordbullet.speed = 2.0f * (1f - rate);
                    break;
            }
        }

        Thunder[] thunders = transform.parent.GetComponentsInChildren<Thunder>();

        foreach (Thunder thunder in thunders)
        {
            switch (thunder.id)
            {
                default:
                    thunder.speed = 3.0f * (1f - rate);
                    break;
            }
        }

        Boomerang[] boomerangs = transform.parent.GetComponentsInChildren<Boomerang>();

        foreach (Boomerang boomerang in boomerangs)
        {
            switch (boomerang.id)
            {
                default:
                    boomerang.speed = 5.0f * (1f - rate);
                    break;
            }
        }
    }

    
    void SpeedUp()
    {
        float speed = 3.8f;
        GameManager.instance.player.speed = speed + speed * rate;
    }
}
