using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // ���� ���� �� �ε� ȭ������ �����ϴ� �Լ�
    public void StartGame()
    {
        SceneManager.LoadScene("LoadingScene"); 
    }
}
