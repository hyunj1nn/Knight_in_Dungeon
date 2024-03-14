using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpUi : MonoBehaviour
{
    RectTransform rect;
    Item[] items;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        items = GetComponentsInChildren<Item>(true);
    }

    public void Show()
    {
        next();
        rect.localScale = Vector3.one;
        Debug.Log("Level Up UI Show!"); // Add this line
        GameManager.instance.Stop();
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
    }

    public void Select(int index)
    {
        items[index].Onclick();
    }

    void next()
    {
        // 1. 모든 아이템 비활성화
        foreach (Item item in items)
        {
            item.gameObject.SetActive(false);
        }

        // Create a list with indices of all items
        List<int> indices = new List<int>();
        for (int i = 0; i < items.Length; i++)
        {
            indices.Add(i);
        }

        // Select and remove 3 random indices
        int[] ran = new int[3];
        for (int i = 0; i < ran.Length; i++)
        {
            int index = Random.Range(0, indices.Count);
            ran[i] = indices[index];
            indices.RemoveAt(index);
        }

        for (int i = 0; i < ran.Length; i++)
        {
            Item ranItem = items[ran[i]];

            // 만렙 아이템의 경우 소비아이템 
            if (ranItem.level == ranItem.data.damages.Length)
            {
                items[Random.Range(8,11)].gameObject.SetActive(true); // 소비 아이템이 8, 9, 10, 11번째일 경우
            }
            else
            {
                ranItem.gameObject.SetActive(true);
            }
        }
    }
}
