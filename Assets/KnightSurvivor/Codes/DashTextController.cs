using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashTextController : MonoBehaviour
{
    public Text dashText;
    public Image spacebarImage;
    public float displayTime = 7.0f; // 텍스트와 이미지가 완전히 사라지기 전까지의 총 시간
    public float fadeDuration = 3.0f; // 텍스트와 이미지가 사라지는 데 걸리는 시간

    private void Start()
    {
        ShowDashNotification();
    }

    void ShowDashNotification()
    {
        StartCoroutine(DisplayNotification());
    }

    IEnumerator DisplayNotification()
    {
        dashText.enabled = true;
        spacebarImage.enabled = true;

        yield return new WaitForSeconds(displayTime - fadeDuration); // fadeDuration 전까지 기다립니다.

        // 점점 옅어지면서 사라지는 애니메이션
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
            dashText.color = newColor;
            spacebarImage.color = newColor;

            yield return null; // 다음 프레임까지 기다립니다.
        }

        dashText.enabled = false;
        spacebarImage.enabled = false;
    }
}
