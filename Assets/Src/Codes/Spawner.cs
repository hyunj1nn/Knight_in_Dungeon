using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level; // 소환 스크립트에서 레벨 담당 변수
    float timer; // 시간에 따라 생성

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>(); // 여러가지 초기화를 위해 Component's' 사용
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // 시간에 따른 몬스터 소환 //
        timer += Time.deltaTime; // deltaTime = 한 프레임당 소비한 시간
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 120f); // 적절한 숫자로 나누어 시간에 맞춰 레벨이 올라가도록 작성

        if (timer > (level == 0 ? 0.7f : 0.3f)) // 레벨을 활용한 소환 타이밍  0.5f = 0.5초마다 생성
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(level); // 레벨에 따라 몬스터 생성
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 자신을 제외하기 때문에 1부터 시작
    }
}

[System.Serializable]  // 직렬화를 통해서 인스펙터에 메뉴 추가
public class SpawnData
{
    public int spriteType;  // 스프라이트 타입
    public float spawnTime;  // 소환시간
    public int health;  // 체력
    public float speed;  // 속도
    public int damage;
}

