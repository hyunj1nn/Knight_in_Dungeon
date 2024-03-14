using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject imageToShow; // 빛나는 이미지나 효과를 포함하는 GameObject를 할당

    // 마우스가 버튼 위에 올라갔을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        imageToShow.SetActive(true); // 이미지를 활성화
    }

    // 마우스가 버튼에서 벗어났을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        imageToShow.SetActive(false); // 이미지를 비활성화
    }
}
