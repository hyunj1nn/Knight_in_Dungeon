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
    private List<GameObject> currentItems = new List<GameObject>(); // 현재 생성된 아이템들을 추적하기 위한 리스트

    public bool showWaypoint = true;  // 이 변수를 통해 인스펙터에서 웨이포인트 표시 여부를 설정
    public ItemWaypoint waypointManager;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        for (int i = 0; i < numberOfItems; i++)
        {
            SpawnRandomItem();
        }
        StartCoroutine(RespawnRoutine()); // 시작과 동시에 리스폰 루틴 시작
    }

    void SpawnRandomItem()
    {
        if (currentItems.Count >= numberOfItems) return;

        // 플레이어의 위치를 찾기
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
        // 파괴된 아이템 제거
        currentItems.RemoveAll(item => item == null);
    }

    private IEnumerator RespawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(respawnDelay);

            int itemsToSpawn = numberOfItems - currentItems.Count;  // 스폰해야할 아이템의 수 계산

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
