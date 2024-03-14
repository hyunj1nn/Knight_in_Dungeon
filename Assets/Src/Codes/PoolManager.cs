using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;  // �����յ� �����ϴ� ����

    List<GameObject>[] pools;  // Ǯ ��� ����Ʈ

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];  // prefabs.Length = ������ ����

        for (int index = 0; index < pools.Length; index++) // index�� 5���� ������ �ݺ� 
        {
            pools[index] = new List<GameObject>();

            //Debug.Log(pools.Length);
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // �������� ������ Ǯ�� ���(��Ȱ��ȭ) �ִ� ���ӿ�����Ʈ ���� ex) ����, ����..

        foreach (GameObject item in pools[index])  // �迭, ����Ʈ���� �����͸� ���������� �����ϴ� �ݺ���
        {
            if (!item.activeSelf) // �������� ��Ȱ��ȭ ��������..
            {
                select = item; // ..�߰��ϸ� select �Լ��� �Ҵ�
                select.SetActive(true);
                break;
            }
        }
                  
        if (!select) // �߰����� ���ϸ�..
        {
            select = Instantiate(prefabs[index], transform); // ..���Ӱ� �����ϰ� select ������ �Ҵ�
            pools[index].Add(select);
        }

        return select;
    }
}
