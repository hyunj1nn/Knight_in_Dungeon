using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullscreenToggle : MonoBehaviour
{
    public Toggle fullscreenToggle;

    private void Start()
    {
        // 게임의 시작 시 Toggle의 상태를 현재 전체 화면 모드와 일치하게 설정
        fullscreenToggle.isOn = Screen.fullScreen;

        // Toggle의 값이 변경될 때 호출될 함수를 설정
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
