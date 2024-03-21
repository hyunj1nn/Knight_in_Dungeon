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
    public Text timeText; // 시간을 표시할 UI 텍스트
    public Text killText; // 킬 수를 표시할 UI 텍스트

    [Header("# Regular Boss Settings")]
    public List<GameObject> regularBossPrefabs;  // 각 보스의 프리팹들을 저장하는 리스트
    private float nextBossSpawnTime = 5 * 60f; // 다음 보스 스폰 시간 (5분)
    private int bossCount = 0;  // 현재까지 스폰된 보스 수

    [Header("# Boss Settings")]
    public GameObject finalBossPrefab;  // 마지막 보스 프리팹을 드래그 & 드롭
    public Transform bossSpawnPoint;  // 보스가 스폰될 위치
    private bool bossSpawned = false;  // 보스가 이미 스폰됐는지 여부
    public float elapsedTime = 0f; // 게임이 시작된 후 경과한 시간

    [Header("# Game UI")]
    public GameObject endGamePanel;  // EndGamePanel을 참조합니다.
    public GameObject victoryScreen; // 승리 화면 UI 참조

    void Awake()
    {
        instance = this;
        player = FindObjectOfType<Player>();  // Player 객체를 찾아서 할당

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

        // 임시로 무기 부여
        uilevelup.Select(0);
        AudioManager.instance.PlayBgm(true);
    }

    void Update()
    {
        if (!isLive)
            return;

        gameTime += Time.deltaTime;

        if (gameTime > nextBossSpawnTime && bossCount < 3) // 보스는 3번까지 스폰 (5분, 10분, 15분)
        {
            SpawnRegularBoss();
            bossCount++;
            nextBossSpawnTime += 5 * 60f; // 다음 보스 스폰 시간 설정 (다시 5분 후)
        }

        // 20분(1200초)이 지나고, 마지막 보스가 아직 등장하지 않았다면
        if (gameTime > 1200f && !bossSpawned)
        {
            SpawnFinalBoss();
            bossSpawned = true;
            AudioManager.instance.PlayFinalBossBgm(); // 이 부분을 추가하여 BGM 변경
        }

        UpdateKillText();
        //Debug.Log("Current Level: " + level + ", Current Exp: " + exp); // 현재 레벨과 경험치를 출력하는 로그 메시지
    }

    void SpawnRegularBoss()
    {
        if (bossCount < regularBossPrefabs.Count) // 현재 보스 카운트가 리스트의 인덱스를 벗어나지 않도록 확인
        {
            GameObject bossToSpawn = regularBossPrefabs[bossCount]; 
            Instantiate(bossToSpawn, bossSpawnPoint.position, Quaternion.identity);  // 해당 보스 프리팹을 인스턴스화
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

            yield return new WaitForSeconds(1f); // 1초마다 업데이트
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
        Stop();  // 게임을 중지합니다.
        endGamePanel.SetActive(true);  // 게임 종료 UI 패널을 활성화
    }

    void ActivateVictoryScreen()
    {
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true); // 승리 화면 활성화
        }
    }

    public void RestartGame()
    {
        // DropOnDestroy 오브젝트 파괴
        foreach (var obj in FindObjectsOfType<DropOnDestroy>())
        {
            Destroy(obj.gameObject);
        }

        // PickUp 오브젝트 파괴
        foreach (var potion in FindObjectsOfType<PickUp>())
        {
            Destroy(potion.gameObject);
        }
        Debug.Log("Restarting the game...");
        endGamePanel.SetActive(false); // 게임 종료 UI 패널을 비활성화
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Resume();  // 게임 시간 흐름을 재개
    }

    public void GoToMainMenu()
    {
        // DropOnDestroy 오브젝트 파괴
        foreach (var obj in FindObjectsOfType<DropOnDestroy>())
        {
            Destroy(obj.gameObject);
        }

        // PickUp 오브젝트 파괴
        foreach (var potion in FindObjectsOfType<PickUp>())
        {
            Destroy(potion.gameObject);
        }
        endGamePanel.SetActive(false); // 게임 종료 UI 패널을 비활성화
        SceneManager.LoadScene("Home Screen");   // 메인 메뉴 씬으로 이동
        Resume();  // 게임 시간 흐름을 재개
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