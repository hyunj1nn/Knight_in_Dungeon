using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 5.0f;  // �ڼ��� ȿ�� ����
    [SerializeField] private float magnetForce = 10.0f;  // �ڼ��� ������� ��

    public bool isActive = false;  // �ڼ� ȿ���� Ȱ��ȭ �Ǿ����� Ȯ���ϴ� ����

    private Transform playerTransform;
    private List<Transform> attractedGems = new List<Transform>();

    private void Start()
    {
        playerTransform = FindObjectOfType<Character>().transform;  // �÷��̾��� Transform�� ã���ϴ�.
    }

    private void Update()
    {
        if (!isActive) return;  // �ڼ� ȿ���� Ȱ��ȭ���� �ʾҴٸ� Update�� �����ϴ�.

        // �ֺ��� ��� GemPickUpObject�� ã���ϴ�.
        Collider2D[] gemsInRange = Physics2D.OverlapCircleAll(playerTransform.position, magnetRadius);

        foreach (Collider2D gemCollider in gemsInRange)
        {
            GemPickUpObject gem = gemCollider.GetComponent<GemPickUpObject>();
            if (gem != null)
            {
                Vector2 directionToPlayer = (playerTransform.position - gem.transform.position).normalized;
                gem.transform.position += (Vector3)directionToPlayer * magnetForce * Time.deltaTime;

                // GemPickUpObject�� OnTriggerEnter2D �޼��尡 ����Ǿ� �÷��̾�� �������� �� �� �ֵ���, 
                // gem�� ����� ��������� ����Ʈ���� �����մϴ�.
                if (Vector2.Distance(playerTransform.position, gem.transform.position) < 0.5f)
                {
                    attractedGems.Remove(gem.transform);
                }
                else if (!attractedGems.Contains(gem.transform))
                {
                    attractedGems.Add(gem.transform);
                }
            }
        }

        // ���� �ۿ� �ִ� GemPickUpObject�� ����Ʈ���� �����մϴ�.
        attractedGems.RemoveAll(gem => gem == null || !IsWithinMagnetRange(gem.position));
    }
    public void ActivateMagnet(float duration)
    {
        StartCoroutine(ActivateAfterDelay(1.0f, duration)); // 1�� �Ŀ� �ڼ� Ȱ��ȭ
    }

    private IEnumerator ActivateAfterDelay(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);  // 1�� ���
        isActive = true;
        StartCoroutine(DeactivateAfterDelay(duration));
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isActive = false;
    }

    private bool IsWithinMagnetRange(Vector2 position)
    {
        return Vector2.Distance(playerTransform.position, position) <= magnetRadius;
    }
}
