using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("# Game Control")]
    public bool isLive;
    public float gameTime;
    public float maxGameTime = 15 * 60f;

    [Header("# Player Info")]
    public int level;
    public int kill;
    public int exp;
    public List<int> nextExp = new List<int> { 10, 30, 60, 130, 160, 220, 260, 300, 330, 370, 400, 450, 490, 530, 570, 600, 630, 670};
    public LevelUp playerLevel;

    [Header("# Game Object")]
    public PoolManager pool;
    public Player player;
    public LevelUpUi uilevelup;

    [Header("# UI Elements")]
    public Text timeText; // �ð��� ǥ���� UI �ؽ�Ʈ
    public Text killText; // ų ���� ǥ���� UI �ؽ�Ʈ

    [Header("# Regular Boss Settings")]
    public List<GameObject> regularBossPrefabs;  // �� ������ �����յ��� �����ϴ� ����Ʈ
    private float nextBossSpawnTime = 5 * 60f; // ���� ���� ���� �ð� (5��)
    private int bossCount = 0;  // ������� ������ ���� ��

    [Header("# Boss Settings")]
    public GameObject finalBossPrefab;  // ������ ���� �������� �巡�� & ���
    public Transform bossSpawnPoint;  // ������ ������ ��ġ
    private bool bossSpawned = false;  // ������ �̹� �����ƴ��� ����
    public float elapsedTime = 0f; // ������ ���۵� �� ����� �ð�

    [Header("# Game UI")]
    public GameObject endGamePanel;  // EndGamePanel�� �����մϴ�.
    public GameObject victoryScreen; // �¸� ȭ�� UI ����

    void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();  // Player ��ü�� ã�Ƽ� �Ҵ�

        uilevelup = FindObjectOfType<LevelUpUi>();
        playerLevel = FindObjectOfType<LevelUp>();

        if (uilevelup == null)
        {
            Debug.LogError("LevelUpUi not found in the scene");
        }
    }

    void Start()
    {
        StartCoroutine(UpdateTimeText());

        // �ӽ÷� ���� �ο�
        uilevelup.Select(0);
        AudioManager.instance.PlayBgm(true);
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > nextBossSpawnTime && bossCount < 3) // ������ 3������ ���� (5��, 10��, 15��)
        {
            SpawnRegularBoss();
            bossCount++;
            nextBossSpawnTime += 5 * 60f; // ���� ���� ���� �ð� ���� (�ٽ� 5�� ��)
        }

        // 20��(1200��)�� ������, ������ ������ ���� �������� �ʾҴٸ�
        if (gameTime > 1200f && !bossSpawned)
        {
            SpawnFinalBoss();
            bossSpawned = true;
            AudioManager.instance.PlayFinalBossBgm(); // �� �κ��� �߰��Ͽ� BGM ����
        }

        UpdateKillText();
        //Debug.Log("Current Level: " + level + ", Current Exp: " + exp); // ���� ������ ����ġ�� ����ϴ� �α� �޽���
    }

    void SpawnRegularBoss()
    {
        if (bossCount < regularBossPrefabs.Count) // ���� ���� ī��Ʈ�� ����Ʈ�� �ε����� ����� �ʵ��� Ȯ��
        {
            GameObject bossToSpawn = regularBossPrefabs[bossCount]; 
            Instantiate(bossToSpawn, bossSpawnPoint.position, Quaternion.identity);  // �ش� ���� �������� �ν��Ͻ�ȭ
        }
        else
        {
            Debug.LogWarning("No boss prefab available for this count!");
        }
    }

    void SpawnFinalBoss()
    {
        Instantiate(finalBossPrefab, bossSpawnPoint.position, Quaternion.identity);
    }

    public void OnBossDefeated()
    {

    }
    void UpdateKillText()
    {
        killText.text = "Kill: " + kill.ToString();
    }

    IEnumerator UpdateTimeText()
    {
        while (true)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);

            timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            yield return new WaitForSeconds(1f); // 1�ʸ��� ������Ʈ
        }
    }
    public void LevelUp(int newLevel)
    {
        level = newLevel;
        uilevelup.Show();
        Debug.Log("Level Up! Current level: " + level);
    }

    public void GetExp(int amount, bool killedMonster = false)
    {
        exp += amount;
        playerLevel.AddExperience(amount);

        if (killedMonster)
        {
            kill++;
        }

        //if (exp >= nextExp[playerLevel.level])
        if (playerLevel.level < nextExp.Count && exp >= nextExp[playerLevel.level])
        {
            playerLevel.CheckLevelUp();
        }
    }

    public void Stop()
    {
        isLive = false;
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        Stop();  // ������ �����մϴ�.
        endGamePanel.SetActive(true);  // ���� ���� UI �г��� Ȱ��ȭ
    }

    void ActivateVictoryScreen()
    {
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true); // �¸� ȭ�� Ȱ��ȭ
        }
    }

    public void RestartGame()
    {
        // DropOnDestroy ������Ʈ �ı�
        foreach (var obj in FindObjectsOfType<DropOnDestroy>())
        {
            Destroy(obj.gameObject);
        }

        // PickUp ������Ʈ �ı�
        foreach (var potion in FindObjectsOfType<PickUp>())
        {
            Destroy(potion.gameObject);
        }
        Debug.Log("Restarting the game...");
        endGamePanel.SetActive(false); // ���� ���� UI �г��� ��Ȱ��ȭ
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Resume();  // ���� �ð� �帧�� �簳
    }

    public void GoToMainMenu()
    {
        // DropOnDestroy ������Ʈ �ı�
        foreach (var obj in FindObjectsOfType<DropOnDestroy>())
        {
            Destroy(obj.gameObject);
        }

        // PickUp ������Ʈ �ı�
        foreach (var potion in FindObjectsOfType<PickUp>())
        {
            Destroy(potion.gameObject);
        }
        endGamePanel.SetActive(false); // ���� ���� UI �г��� ��Ȱ��ȭ
        SceneManager.LoadScene("Home Screen");   // ���� �޴� ������ �̵�
        Resume();  // ���� �ð� �帧�� �簳
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}