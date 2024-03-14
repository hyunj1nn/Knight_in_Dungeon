using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject imageToShow; // ������ �̹����� ȿ���� �����ϴ� GameObject�� �Ҵ��մϴ�.

    // ���콺�� ��ư ���� �ö��� �� ȣ��˴ϴ�.
    public void OnPointerEnter(PointerEventData eventData)
    {
        imageToShow.SetActive(true); // �̹����� Ȱ��ȭ�մϴ�.
    }

    // ���콺�� ��ư���� ����� �� ȣ��˴ϴ�.
    public void OnPointerExit(PointerEventData eventData)
    {
        imageToShow.SetActive(false); // �̹����� ��Ȱ��ȭ�մϴ�.
    }
}
