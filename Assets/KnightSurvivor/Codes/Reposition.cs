using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reposition : MonoBehaviour
{
    Collider2D coll;

    void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Area"))
            return;

        Vector3 playerPos = GameManager.instance.player.transform.position; // �÷��̾� ��ġ�� Ÿ�ϸ� ��ġ ����
        Vector3 mypos = transform.position; // myPos�� ����� �� ��ġ ����
        float diffX = Mathf.Abs(playerPos.x - mypos.x); // - ���� ����� Mathf ���
        float diffY = Mathf.Abs(playerPos.y - mypos.y);

        Vector3 playerDir = GameManager.instance.player.inputVec; // �÷��̾��� ���� �ľ�
        float dirX = playerDir.x < 0 ? -1 : 1;  //  x�� 0���� ������ -1 , 0���� Ŭ�� 1 ���
        float dirY = playerDir.y < 0 ? -1 : 1;

        switch (transform.tag)
        {
            case "Ground":
                if (Mathf.Abs(diffX - diffY) <= 0.1f)
                {
                    transform.Translate(Vector3.up * dirY * 80);
                    transform.Translate(Vector3.right * dirX * 80);
                }
                else if (diffX > diffY)
                {
                    transform.Translate(Vector3.right * dirX * 80);
                }
                else // diffX < diffY
                {
                    transform.Translate(Vector3.up * dirY * 80);
                }
                break;
            case "Enemy":
                if (coll.enabled)
                {
                    transform.Translate(playerDir * 60 + new Vector3(Random.Range(-3f, 3f), 0f));  // ���� �����
                }
                break;
        }
    }
}
