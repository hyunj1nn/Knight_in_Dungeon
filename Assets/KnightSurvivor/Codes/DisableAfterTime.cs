using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterTime : MonoBehaviour
{
    [SerializeField] float timeToDisable = 0.8f;
    float timer;

    void OnEnable()
    {
        timer = timeToDisable;
    }

    void LateUpdate()
    {
        timer -= Time.deltaTime;
        if(timer < 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
