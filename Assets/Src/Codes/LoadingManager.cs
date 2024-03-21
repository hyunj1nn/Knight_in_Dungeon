using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public string sceneToLoad = "SampleScene"; // �ε� �� �̵��� ��
    public float minimumLoadingTime = 5f;      // �ε� ȭ���� ǥ���� �ּ� �ð� 

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

            // �ε��� �������� �ּ� �ε� �ð��� ���� ������ �ʾҴٸ� �߰��� ���
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
