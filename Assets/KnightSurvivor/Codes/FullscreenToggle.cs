using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    public Toggle fullscreenToggle;

    private void Start()
    {
        // ������ ���� �� Toggle�� ���¸� ���� ��ü ȭ�� ���� ��ġ�ϰ� ����
        fullscreenToggle.isOn = Screen.fullScreen;

        // Toggle�� ���� ����� �� ȣ��� �Լ��� ����
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
