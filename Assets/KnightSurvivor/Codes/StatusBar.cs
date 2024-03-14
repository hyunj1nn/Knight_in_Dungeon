using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBar : MonoBehaviour
{
    [SerializeField] Transform bar;

    public void SetState(float current, float max)  // float 함수를 사용해서 자연스럽게 채워지거나 줄어들기
    {
        float state = current / max;
        if (state < 0f) { state = 0f; }
        bar.transform.localScale = new Vector3(state, 1f, 1f);
    }
}
