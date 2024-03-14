using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAutoDestroy : MonoBehaviour
{
    void Start()
    {
        // 애니메이터 컴포넌트를 가져옵니다.
        Animator animator = GetComponent<Animator>();

        // 애니메이터가 있는 경우, 애니메이션 클립의 길이를 가져옵니다.
        float duration = 0;
        if (animator)
        {
            // 현재 재생중인 애니메이션 클립을 가져옵니다.
            AnimationClip clip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
            duration = clip.length;
        }

        // 지정된 시간이 지난 후에 자신을 삭제합니다.
        Destroy(gameObject, duration);
    }
}
