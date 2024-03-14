using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public SpawnData[] spawnData;

    int level; // ��ȯ ��ũ��Ʈ���� ���� ��� ����
    float timer; // �ð��� ���� ����

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>(); // �������� �ʱ�ȭ�� ���� Component's' ���
    }

    void Update()
    {
        if (!GameManager.instance.isLive)
            return;

        // �ð��� ���� ���� ��ȯ //
        timer += Time.deltaTime; // deltaTime = �� �����Ӵ� �Һ��� �ð�
        level = Mathf.FloorToInt(GameManager.instance.gameTime / 120f); // ������ ���ڷ� ������ �ð��� ���� ������ �ö󰡵��� �ۼ�

        if (timer > (level == 0 ? 0.7f : 0.3f)) // ������ Ȱ���� ��ȯ Ÿ�̹�  0.5f = 0.5�ʸ��� ����
        {
            timer = 0;
            Spawn();
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.Get(level); // ������ ���� ���� ����
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // �ڽ��� �����ϱ� ������ 1���� ����
    }
}

[System.Serializable]  // ����ȭ�� ���ؼ� �ν����Ϳ� �޴� �߰�
public class SpawnData
{
    public int spriteType;  // ��������Ʈ Ÿ��
    public float spawnTime;  // ��ȯ�ð�
    public int health;  // ü��
    public float speed;  // �ӵ�
    public int damage;
}

