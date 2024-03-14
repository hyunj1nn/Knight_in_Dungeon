using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject[] prefabs;  // 프리팹들 보관하는 변수

    List<GameObject>[] pools;  // 풀 담당 리스트

    void Awake()
    {
        pools = new List<GameObject>[prefabs.Length];  // prefabs.Length = 프리팹 갯수

        for (int index = 0; index < pools.Length; index++) // index가 5보다 작을때 반복 
        {
            pools[index] = new List<GameObject>();

            //Debug.Log(pools.Length);
        }
    }

    public GameObject Get(int index)
    {
        GameObject select = null;

        // 프리팹을 선택한 풀의 놀고(비활성화) 있는 게임오브젝트 접근 ex) 몬스터, 무기..

        foreach (GameObject item in pools[index])  // 배열, 리스트들의 데이터를 순차적으로 접근하는 반복문
        {
            if (!item.activeSelf) // 아이템이 비활성화 돼있을때..
            {
                select = item; // ..발견하면 select 함수에 할당
                select.SetActive(true);
                break;
            }
        }
                  
        if (!select) // 발견하지 못하면..
        {
            select = Instantiate(prefabs[index], transform); // ..새롭게 생성하고 select 변수에 할당
            pools[index].Add(select);
        }

        return select;
    }
}
