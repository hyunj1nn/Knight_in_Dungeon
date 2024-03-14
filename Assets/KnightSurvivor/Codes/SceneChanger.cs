using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // 게임 시작 시 로딩 화면으로 변경하는 함수
    public void StartGame()
    {
        SceneManager.LoadScene("LoadingScene"); // "LoadingScene"은 로딩 화면 씬의 이름이어야 합니다.
    }
}
