using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAutoDestroy : MonoBehaviour
{
    void Start()
    {
        // �ִϸ����� ������Ʈ�� �����ɴϴ�.
        Animator animator = GetComponent<Animator>();

        // �ִϸ����Ͱ� �ִ� ���, �ִϸ��̼� Ŭ���� ���̸� �����ɴϴ�.
        float duration = 0;
        if (animator)
        {
            // ���� ������� �ִϸ��̼� Ŭ���� �����ɴϴ�.
            AnimationClip clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            duration = clip.length;
        }

        // ������ �ð��� ���� �Ŀ� �ڽ��� �����մϴ�.
        Destroy(gameObject, duration);
    }
}
