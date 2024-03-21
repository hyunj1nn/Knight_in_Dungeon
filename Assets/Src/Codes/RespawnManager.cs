using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public GameObject itemPrefab;
    public Vector2 minSpawnRange = new Vector2(-1, 1);
    public Vector2 maxSpawnRange = new Vector2(-10, 10);
    public int numberOfItems = 4;
    public float respawnDelay = 10f;
    private bool isSpawning = false;
    private List<GameObject> currentItems = new List<GameObject>(); // ���� ������ �����۵��� �����ϱ� ���� ����Ʈ

    public bool showWaypoint = true;  // �� ������ ���� �ν����Ϳ��� ��������Ʈ ǥ�� ���θ� ����
    public ItemWaypoint waypointManager;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < numberOfItems; i++)
        {
            SpawnRandomItem();
        }
        StartCoroutine(RespawnRoutine()); // ���۰� ���ÿ� ������ ��ƾ ����
    }

    void SpawnRandomItem()
    {
        if (currentItems.Count >= numberOfItems) return;

        // �÷��̾��� ��ġ�� ã��
        Vector3 playerPosition = playerTransform.position;

        Vector3 spawnPosition;

        do
        {
            float x = playerPosition.x + Random.Range(minSpawnRange.x, maxSpawnRange.y);
            float y = playerPosition.y + Random.Range(minSpawnRange.x, maxSpawnRange.y);
            spawnPosition = new Vector3(x, y, 0);

        } while ((spawnPosition.x > playerPosition.x + minSpawnRange.x && spawnPosition.x < playerPosition.x + minSpawnRange.y) && (spawnPosition.y > playerPosition.y + minSpawnRange.x && spawnPosition.y < playerPosition.y + minSpawnRange.y));

        GameObject newItem = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        currentItems.Add(newItem);

        if (showWaypoint)
        {
            ItemWaypoint itemWaypoint = FindObjectOfType<ItemWaypoint>();
            if (itemWaypoint != null)
            {
                itemWaypoint.CreateWaypoint(spawnPosition);
            }
        }
    }

    public Vector3 GenerateRandomSpawnPosition()
    {
        Vector3 playerPosition = playerTransform.position;
        Vector3 spawnPosition;

        do
        {
            float x = playerPosition.x + Random.Range(minSpawnRange.x, maxSpawnRange.y);
            float y = playerPosition.y + Random.Range(minSpawnRange.x, maxSpawnRange.y);
            spawnPosition = new Vector3(x, y, 0);

        } while ((spawnPosition.x > playerPosition.x + minSpawnRange.x && spawnPosition.x < playerPosition.x + minSpawnRange.y) && (spawnPosition.y > playerPosition.y + minSpawnRange.x && spawnPosition.y < playerPosition.y + minSpawnRange.y));

        return spawnPosition;
    }

    private void Update()
    {
        // �ı��� ������ ����
        currentItems.RemoveAll(item => item == null);
    }

    private IEnumerator RespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnDelay);

            int itemsToSpawn = numberOfItems - currentItems.Count;  // �����ؾ��� �������� �� ���

            for (int i = 0; i < itemsToSpawn; i++)
            {
                SpawnRandomItem();
            }
        }
    }

    private void OnDisable()
    {
        StopCoroutine(RespawnRoutine());

    }
    public IEnumerator RespawnAfterDelay()
    {
        if (isSpawning) yield break;

        isSpawning = true;

        yield return new WaitForSeconds(respawnDelay);

        for (int i = 0; i < numberOfItems; i++)
        {
            SpawnRandomItem();
        }

        isSpawning = false;
    }
}
