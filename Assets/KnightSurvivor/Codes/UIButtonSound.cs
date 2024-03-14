using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverClip; // ���콺�� ������ ����� ���� ȿ����
    public AudioClip clickClip; // Ŭ������ ���� ȿ����

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // ���콺�� ��ư�� ������ ����� ��
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverClip)
        {
            audioSource.clip = hoverClip;
            audioSource.Play();
        }
    }

    // ��ư�� Ŭ������ ��
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickClip)
        {
            audioSource.clip = clickClip;
            audioSource.Play();
        }
    }
}
