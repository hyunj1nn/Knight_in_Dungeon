using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public GameObject eliteMonsterPrefab; // ����Ʈ ���� ������
    public List<Transform> spawnPoints = new List<Transform>(); // ����Ʈ ���� ���� ��ġ ����Ʈ
    public float spawnInterval = 180.0f; // 3�и��� ����Ʈ ���� ���� (�� ����)

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;  // �� ������ Ÿ�̸� ����

        if (timer >= spawnInterval)
        {
            SpawnEliteMonsters();
            timer = 0; // Ÿ�̸� �ʱ�ȭ
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
