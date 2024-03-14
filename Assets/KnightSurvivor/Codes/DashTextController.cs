using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashTextController : MonoBehaviour
{
    public Text dashText;
    public Image spacebarImage;
    public float displayTime = 7.0f; // �ؽ�Ʈ�� �̹����� ������ ������� �������� �� �ð�
    public float fadeDuration = 3.0f; // �ؽ�Ʈ�� �̹����� ������� �� �ɸ��� �ð�

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

        yield return new WaitForSeconds(displayTime - fadeDuration); // fadeDuration ������ ��ٸ��ϴ�.

        // ���� �������鼭 ������� �ִϸ��̼�
        float startTime = Time.time;
        while (Time.time < startTime + fadeDuration)
        {
            float t = (Time.time - startTime) / fadeDuration;
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(1, 0, t));
            dashText.color = newColor;
            spacebarImage.color = newColor;

            yield return null; // ���� �����ӱ��� ��ٸ��ϴ�.
        }

        dashText.enabled = false;
        spacebarImage.enabled = false;
    }
}
