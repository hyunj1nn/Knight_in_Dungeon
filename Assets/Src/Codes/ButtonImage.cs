using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject imageToShow; // ������ �̹����� ȿ���� �����ϴ� GameObject�� �Ҵ�

    // ���콺�� ��ư ���� �ö��� �� ȣ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        imageToShow.SetActive(true); // �̹����� Ȱ��ȭ
    }

    // ���콺�� ��ư���� ����� �� ȣ��
    public void OnPointerExit(PointerEventData eventData)
    {
        imageToShow.SetActive(false); // �̹����� ��Ȱ��ȭ
    }
}
