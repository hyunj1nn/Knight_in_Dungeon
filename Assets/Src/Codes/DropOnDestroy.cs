using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject[] pickUpItems; // 여러 아이템을 저장하는 배열
    [SerializeField] [Range(0f, 1f)] float chanceToDrop = 1f; // 아이템을 드랍할 확률
    [SerializeField] float[] itemWeights; // 아이템 드랍 확률의 가중치

    private bool isQuitting = false;

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDestroy()
    {
        if (Application.isPlaying && !isQuitting)
        {
            if (Random.value < chanceToDrop && pickUpItems.Length > 0)
            {
                // 확률적으로 아이템을 선택
                GameObject itemToDrop = ChooseRandomItemByWeight();
                if (itemToDrop)
                {
                    Transform t = Instantiate(itemToDrop).transform;
                    t.position = transform.position;
                }
            }
        }
    }

    // 가중치를 고려하여 아이템을 무작위로 선택하는 함수
    GameObject ChooseRandomItemByWeight()
    {
        if (pickUpItems.Length != itemWeights.Length)
        {
            Debug.LogError("Items and weights array lengths do not match!");
            return null;
        }

        float totalWeight = 0;
        foreach (float weight in itemWeights)
        {
            totalWeight += weight;
        }

        float randomValue = Random.Range(0, totalWeight);
        float weightSum = 0;
        for (int i = 0; i < pickUpItems.Length; i++)
        {
            weightSum += itemWeights[i];
            if (randomValue < weightSum)
            {
                return pickUpItems[i];
            }
        }

        return null;
    }
}
