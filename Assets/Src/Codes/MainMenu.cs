using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;     // 초기 메인 메뉴 패널 (게임 시작, 종료 버튼이 있는 패널)
    public GameObject subMenuPanel;      // 서브 메뉴 패널 (던전 입장, 캐릭터 선택 등의 버튼이 있는 패널)

    public void OnStartButtonClicked()
    {
        // 메인 메뉴 패널을 비활성화하고 서브 메뉴 패널을 활성화
        mainMenuPanel.SetActive(false);
        subMenuPanel.SetActive(true);
    }
}
