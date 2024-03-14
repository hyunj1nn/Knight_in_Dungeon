using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageTextManager : MonoBehaviour
{
    private static DamageTextManager instance;
    public static DamageTextManager Instance
    {
        get { return instance; }
    }

    public GameObject damageTextPrefab;
    private List<DamageText> damageTextPool = new List<DamageText>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void ShowDamageText(int damage, Vector3 position)
    {
        DamageText damageText = GetDamageText();
        damageText.transform.position = position;
        damageText.ShowDamage(damage);
    }

    private DamageText GetDamageText()
    {
        foreach (DamageText damageText in damageTextPool)
        {
            if (!damageText.gameObject.activeInHierarchy)
            {
                return damageText;
            }
        }

        DamageText newDamageText = Instantiate(damageTextPrefab, transform).GetComponent<DamageText>();
        damageTextPool.Add(newDamageText);
        return newDamageText;
    }
}