using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject eliteMonsterPrefab; // 엘리트 몬스터 프리팹
    public List<Transform> spawnPoints = new List<Transform>(); // 엘리트 몬스터 생성 위치 리스트
    public float spawnInterval = 180.0f; // 3분마다 엘리트 몬스터 생성 (초 단위)

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;  // 초 단위로 타이머 증가

        if (timer >= spawnInterval)
        {
            SpawnEliteMonsters();
            timer = 0; // 타이머 초기화
        }
    }

    void SpawnEliteMonsters()
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(eliteMonsterPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
