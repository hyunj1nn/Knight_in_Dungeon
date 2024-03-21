using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public string sceneToLoad = "SampleScene"; // 로딩 후 이동할 씬
    public float minimumLoadingTime = 5f;      // 로딩 화면을 표시할 최소 시간 

    void Start()
    {
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        float loadingStartTime = Time.time;
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneToLoad);

        while (!asyncOperation.isDone)
        {
            yield return null;

            // 로딩이 끝났지만 최소 로딩 시간이 아직 지나지 않았다면 추가로 대기
            if (asyncOperation.isDone)
            {
                float elapsedTime = Time.time - loadingStartTime;
                if (elapsedTime < minimumLoadingTime)
                {
                    yield return new WaitForSeconds(minimumLoadingTime - elapsedTime);
                }
            }
        }
    }
}
