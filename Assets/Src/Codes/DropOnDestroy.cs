using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject[] pickUpItems; // ���� �������� �����ϴ� �迭
    [SerializeField] [Range(0f, 1f)] float chanceToDrop = 1f; // �������� ����� Ȯ��
    [SerializeField] float[] itemWeights; // ������ ��� Ȯ���� ����ġ

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
                // Ȯ�������� �������� ����
                GameObject itemToDrop = ChooseRandomItemByWeight();
                if (itemToDrop)
                {
                    Transform t = Instantiate(itemToDrop).transform;
                    t.position = transform.position;
                }
            }
        }
    }

    // ����ġ�� ����Ͽ� �������� �������� �����ϴ� �Լ�
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
