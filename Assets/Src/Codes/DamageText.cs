using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed;
    public float jumpForce;
    public float gravity = 9.8f;
    public float destroyTime;

    private TextMeshPro textMesh;
    private Vector3 moveDirection;
    private float verticalVelocity;

    //private Vector3 moveDirection;
    //private float verticalVelocity;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        transform.SetParent(DamageTextManager.Instance.transform);
    }

    public void ShowDamage(int damage)
    {
        textMesh.text = damage.ToString();
        StartCoroutine(MoveAndFadeOut());
    }

    private IEnumerator MoveAndFadeOut()
    {
        float timer = 0f;
        verticalVelocity = jumpForce;

        while (timer < destroyTime)
        {
            timer += Time.deltaTime;


            // 중력 적용
            verticalVelocity -= gravity * Time.deltaTime;

            // 이동
            moveDirection = new Vector3(0f, verticalVelocity, 0f);
            transform.position += moveDirection * Time.deltaTime * moveSpeed;


            yield return null;
        }

        // 파괴
        Destroy(gameObject);
    }
}
