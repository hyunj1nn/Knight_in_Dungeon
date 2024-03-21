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
        // 월드 포지션에 해당하는 게임 오브젝트가 존재하는지 확인
        Collider2D hitObject = Physics2D.OverlapPoint(worldPosition); // 2D 게임을 기반으로 한다고 가정

        // 해당 위치에 오브젝트가 없으면 (아이템이 삭제되었으면), 웨이포인트 삭제
        if (hitObject == null)
        {
            waypoints.Remove(waypoint);
            Destroy(waypoint.gameObject);
            return; // 메서드 종료
        }

        Vector2 screenPos = mainCamera.WorldToScreenPoint(worldPosition);

        // 화면 내부에 있는지 검사
        bool isWithinScreen =
            screenPos.x >= 0 && screenPos.x <= Screen.width &&
            screenPos.y >= 0 && screenPos.y <= Screen.height;

        if (isWithinScreen)
        {
            waypoint.enabled = false; // 화면 내부에 있으면 웨이포인트를 숨김
        }
        else
        {
            waypoint.enabled = true; // 화면 바깥에 있으면 웨이포인트를 표시

            int margin = 100; // 간격을 주기 위한 값

            // 화면 경계 내로 웨이포인트 위치를 제한하면서 간격을 줌
            if (screenPos.x < margin) screenPos.x = margin;
            if (screenPos.x > Screen.width - margin) screenPos.x = Screen.width - margin;
            if (screenPos.y < margin) screenPos.y = margin;
            if (screenPos.y > Screen.height - margin) screenPos.y = Screen.height - margin;

            waypoint.transform.position = screenPos;
        }
    }

    public void RemoveWaypoint(Vector3 worldPosition)
    {
        // 월드 포지션을 사용하여 웨이포인트를 찾고 제거
        Image waypointToRemove = null;
        foreach (var pair in waypoints)
        {
            if (Vector3.Distance(pair.Value, worldPosition) < 0.01f) // 거리가 0.01f보다 작으면 동일한 위치로 판단
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
