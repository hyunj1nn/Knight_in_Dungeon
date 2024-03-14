using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Text loadingText; // 로딩 상태를 표시할 텍스트

    private void Start()
    {
        StartCoroutine(LoadGameScene());
    }

    private IEnumerator LoadGameScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("YourGameSceneName");

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            loadingText.text = "Loading... " + (progress * 100).ToString("F0") + "%";
            yield return null;
        }
    }
}
