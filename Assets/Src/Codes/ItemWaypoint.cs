using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemWaypoint : MonoBehaviour
{
    public Camera mainCamera;
    public Image waypointPrefab;
    public Transform canvasTransform;
    private Dictionary<Image, Vector3> waypoints = new Dictionary<Image, Vector3>();

    void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    public void CreateWaypoint(Vector3 worldPosition)
    {
        Image newWaypoint = Instantiate(waypointPrefab, canvasTransform);
        waypoints[newWaypoint] = worldPosition;
    }

    private void Update()
    {
        foreach (var pair in waypoints)
        {
            UpdateWaypointPosition(pair.Key, pair.Value);
        }
    }

    private void UpdateWaypointPosition(Image waypoint, Vector3 worldPosition)
    {
        // ���� �����ǿ� �ش��ϴ� ���� ������Ʈ�� �����ϴ��� Ȯ��
        Collider2D hitObject = Physics2D.OverlapPoint(worldPosition); // 2D ������ ������� �Ѵٰ� ����

        // �ش� ��ġ�� ������Ʈ�� ������ (�������� �����Ǿ�����), ��������Ʈ ����
        if (hitObject == null)
        {
            waypoints.Remove(waypoint);
            Destroy(waypoint.gameObject);
            return; // �޼��� ����
        }

        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        // ȭ�� ���ο� �ִ��� �˻�
        bool isWithinScreen =
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height;

        if (isWithinScreen)
        {
            waypoint.enabled = false; // ȭ�� ���ο� ������ ��������Ʈ�� ����
        }
        else
        {
            waypoint.enabled = true; // ȭ�� �ٱ��� ������ ��������Ʈ�� ǥ��

            int margin = 100; // ������ �ֱ� ���� ��

            // ȭ�� ��� ���� ��������Ʈ ��ġ�� �����ϸ鼭 ������ ��
            if (screenPos.x < margin) screenPos.x = margin;
            if (screenPos.x > Screen.width - margin) screenPos.x = Screen.width - margin;
            if (screenPos.y < margin) screenPos.y = margin;
            if (screenPos.y > Screen.height - margin) screenPos.y = Screen.height - margin;

            waypoint.transform.position = screenPos;
        }
    }

    public void RemoveWaypoint(Vector3 worldPosition)
    {
        // ���� �������� ����Ͽ� ��������Ʈ�� ã�� ����
        Image waypointToRemove = null;
        foreach (var pair in waypoints)
        {
            if (Vector3.Distance(pair.Value, worldPosition) < 0.01f) // �Ÿ��� 0.01f���� ������ ������ ��ġ�� �Ǵ�
            {
                waypointToRemove = pair.Key;
                break;
            }
        }

        if (waypointToRemove)
        {
            waypoints.Remove(waypointToRemove);
            Destroy(waypointToRemove.gameObject);
        }
    }
}
