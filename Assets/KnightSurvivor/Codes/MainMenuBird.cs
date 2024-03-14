using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBird : MonoBehaviour
{
    public float speed = 1.0f;
    public float distance = 5.0f;

    private Vector3 leftPosition;
    private Vector3 rightPosition;

    private void Start()
    {
        leftPosition = transform.position - new Vector3(distance / 2, 0, 0);
        rightPosition = transform.position + new Vector3(distance / 2, 0, 0);
        StartCoroutine(MoveBird());
    }

    IEnumerator MoveBird()
    {
        while (true)
        {
            yield return MoveToTarget(rightPosition); // 오른쪽 위치로 움직이기
            yield return MoveToTarget(leftPosition);  // 왼쪽 위치로 움직이기
        }
    }

    IEnumerator MoveToTarget(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}
