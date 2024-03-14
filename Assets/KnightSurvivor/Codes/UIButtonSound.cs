using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip; // 마우스를 가져다 대었을 때의 효과음
    public AudioClip clickClip; // 클릭했을 때의 효과음

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // 마우스를 버튼에 가져다 대었을 때
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip)
        {
            audioSource.clip = hoverClip;
            audioSource.Play();
        }
    }

    // 버튼을 클릭했을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip)
        {
            audioSource.clip = clickClip;
            audioSource.Play();
        }
    }
}
