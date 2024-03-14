using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject player; // 플레이어에 대한 참조
    public GameObject obstaclePrefab; // 장애물 프리팹
    public List<GameObject> obstacles = new List<GameObject>(); // 장애물 리스트
    public float spawnDistance = 35f; // 플레이어와 장애물 사이의 최소 거리
    public int obstacleCount; // 생성할 장애물 수
    public float safeDistance = 20f; // 안전 범위 설정
    public Transform obstacleParent; // 장애물들을 담을 부모 오브젝트

    void Start()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            SpawnObstacle();
        }
    }

    void Update()
    {
        // 리스트 내 모든 장애물을 확인하고, 너무 멀리 떨어진 장애물은 제거하고 새로 생성
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            // 만약 장애물이 이미 파괴되었다면 리스트에서 제거하고 continue로 다음 반복으로 넘어갑니다.
            if (obstacles[i] == null)
            {
                obstacles.RemoveAt(i);
                continue;
            }
            if (Vector3.Distance(player.transform.position, obstacles[i].transform.position) > spawnDistance)
            {
                Destroy(obstacles[i]);
                obstacles.RemoveAt(i);
                SpawnObstacle();
            }
        }
    }

    void SpawnObstacle()
    {
        float spawnX, spawnY;
        Vector3 spawnPosition;

        do
        {
            spawnX = Random.Range(player.transform.position.x - spawnDistance, player.transform.position.x + spawnDistance);
            spawnY = Random.Range(player.transform.position.y - spawnDistance, player.transform.position.y + spawnDistance);
            spawnPosition = new Vector3(spawnX, spawnY, 0);
        }
        while (Vector2.Distance(player.transform.position, new Vector2(spawnX, spawnY)) < safeDistance || IsTooCloseToOtherObstacles(spawnPosition)); // 안전 범위 및 기존 장애물과의 거리 확인

        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        newObstacle.transform.SetParent(obstacleParent); // 생성된 장애물을 부모 오브젝트의 자식으로 설정
        obstacles.Add(newObstacle);
    }

    bool IsTooCloseToOtherObstacles(Vector3 position)
    {
        foreach (GameObject obstacle in obstacles)
        {
            if (Vector3.Distance(position, obstacle.transform.position) < safeDistance)
            {
                return true;
            }
        }

        return false;
    }
}
