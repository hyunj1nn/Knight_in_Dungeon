using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAutoDestroy : MonoBehaviour
{
    void Start()
    {
        Animator animator = GetComponent<Animator>();

        // �ִϸ����Ͱ� �ִ� ���, �ִϸ��̼� Ŭ���� ���̸� ������
        float duration = 0;
        if (animator)
        {
            // ���� ������� �ִϸ��̼� Ŭ���� ������
            AnimationClip clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            duration = clip.length;
        }

        // ������ �ð��� ���� �Ŀ� �ڽ��� ����
        Destroy(gameObject, duration);
    }
}
