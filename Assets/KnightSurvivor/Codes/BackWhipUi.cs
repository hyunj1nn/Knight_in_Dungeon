using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackWhipUi : MonoBehaviour
{
    public static BackWhipUi instance;

    RectTransform rect;
    public Item[] items;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void ShowBossDrop()
    {
        DisplayBossItems();
        rect.localScale = Vector3.one;
        Debug.Log("Boss Drop UI Show!");
        GameManager.instance.Stop();
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
    }

    void DisplayBossItems()
    {
        if (items.Length == 0)
        {
            Debug.LogWarning("No items in the BossDropUi's items array!");
            return;
        }

        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }
        items[0].gameObject.SetActive(true);
    }
}
