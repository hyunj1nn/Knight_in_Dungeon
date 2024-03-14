using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDropWhipItem : MonoBehaviour
{
    public ItemWaypoint waypointController;

    private void Start()
    {
        if (waypointController != null)  // waypointController가 null이 아닐 때만 호출
        {
            waypointController.CreateWaypoint(transform.position);
        }
        else
        {
            Debug.LogWarning("WaypointController is not assigned in BossDropItem.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WhipWeapon whipWeapon = other.GetComponentInChildren<WhipWeapon>();
            if (whipWeapon != null)
            {
                whipWeapon.AcquireBossDropItem();
            }

            // BossWhipUi를 표시 
            BackWhipUi.instance.ShowBossDrop();

            Destroy(gameObject);
        }
    }
}
