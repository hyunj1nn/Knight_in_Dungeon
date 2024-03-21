using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuPanel;     // �ʱ� ���� �޴� �г� (���� ����, ���� ��ư�� �ִ� �г�)
    public GameObject subMenuPanel;      // ���� �޴� �г� (���� ����, ĳ���� ���� ���� ��ư�� �ִ� �г�)

    public void OnStartButtonClicked()
    {
        // ���� �޴� �г��� ��Ȱ��ȭ�ϰ� ���� �޴� �г��� Ȱ��ȭ
        mainMenuPanel.SetActive(false);
        subMenuPanel.SetActive(true);
    }
}
