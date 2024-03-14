using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetItem : MonoBehaviour
{
    [SerializeField] private float magnetRadius = 5.0f;  // 자석의 효과 범위
    [SerializeField] private float magnetForce = 10.0f;  // 자석의 끌어당기는 힘

    public bool isActive = false;  // 자석 효과가 활성화 되었는지 확인하는 변수

    private Transform playerTransform;
    private List<Transform> attractedGems = new List<Transform>();

    private void Start()
    {
        playerTransform = FindObjectOfType<Character>().transform;  // 플레이어의 Transform을 찾습니다.
    }

    private void Update()
    {
        if (!isActive) return;  // 자석 효과가 활성화되지 않았다면 Update를 끝냅니다.

        // 주변의 모든 GemPickUpObject를 찾습니다.
        Collider2D[] gemsInRange = Physics2D.OverlapCircleAll(playerTransform.position, magnetRadius);

        foreach (Collider2D gemCollider in gemsInRange)
        {
            GemPickUpObject gem = gemCollider.GetComponent<GemPickUpObject>();
            if (gem != null)
            {
                Vector2 directionToPlayer = (playerTransform.position - gem.transform.position).normalized;
                gem.transform.position += (Vector3)directionToPlayer * magnetForce * Time.deltaTime;

                // GemPickUpObject의 OnTriggerEnter2D 메서드가 실행되어 플레이어에게 아이템을 줄 수 있도록, 
                // gem이 충분히 가까워지면 리스트에서 제거합니다.
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

        // 범위 밖에 있는 GemPickUpObject는 리스트에서 제거합니다.
        attractedGems.RemoveAll(gem => gem == null || !IsWithinMagnetRange(gem.position));
    }
    public void ActivateMagnet(float duration)
    {
        StartCoroutine(ActivateAfterDelay(1.0f, duration)); // 1초 후에 자석 활성화
    }

    private IEnumerator ActivateAfterDelay(float delay, float duration)
    {
        yield return new WaitForSeconds(delay);  // 1초 대기
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
