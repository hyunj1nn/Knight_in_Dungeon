using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public GameObject player; // �÷��̾ ���� ����
    public GameObject obstaclePrefab; // ��ֹ� ������
    public List<GameObject> obstacles = new List<GameObject>(); // ��ֹ� ����Ʈ
    public float spawnDistance = 35f; // �÷��̾�� ��ֹ� ������ �ּ� �Ÿ�
    public int obstacleCount; // ������ ��ֹ� ��
    public float safeDistance = 20f; // ���� ���� ����
    public Transform obstacleParent; // ��ֹ����� ���� �θ� ������Ʈ

    void Start()
    {
        for (int i = 0; i < obstacleCount; i++)
        {
            SpawnObstacle();
        }
    }

    void Update()
    {
        // ����Ʈ �� ��� ��ֹ��� Ȯ���ϰ�, �ʹ� �ָ� ������ ��ֹ��� �����ϰ� ���� ����
        for (int i = obstacles.Count - 1; i >= 0; i--)
        {
            // ���� ��ֹ��� �̹� �ı��Ǿ��ٸ� ����Ʈ���� �����ϰ� continue�� ���� �ݺ����� �Ѿ�ϴ�.
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
        while (Vector2.Distance(player.transform.position, new Vector2(spawnX, spawnY)) < safeDistance || IsTooCloseToOtherObstacles(spawnPosition)); // ���� ���� �� ���� ��ֹ����� �Ÿ� Ȯ��

        GameObject newObstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        newObstacle.transform.SetParent(obstacleParent); // ������ ��ֹ��� �θ� ������Ʈ�� �ڽ����� ����
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
