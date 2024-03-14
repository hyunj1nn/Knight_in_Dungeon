using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boomerang : MonoBehaviour
{
    [SerializeField] public GameObject attackObject;

    public int id;  // 무기 ID
    public int prefabId;  // 프리펩 ID
    public int damage;  // 무기 데미지
    public int count;  // 무기 갯수
    public float speed;  // 무기 속도
    public int projectileCount = 1;


    float timer;
    Player player;

    private bool isReturning = false; 
    private Vector3 firingDirection; 
    private float returnTime = 2.0f; 
    private float elapsedTime = 0.0f; 

    void Awake()
    {
        player = GameManager.instance.player;
    }


    void Update()
    {
        transform.position = player.transform.position;

        if (!isReturning) 
        {
            switch (id)
            {
                case 0:
                    transform.Rotate(Vector3.back * speed * Time.deltaTime);
                    break;
                default:
                    timer += Time.deltaTime;
                    if (timer > speed)
                    {
                        timer = 0f;
                        Fire();
                    }
                    break;
            }

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= returnTime)
            {
                StartReturning();
            }
        }
        else 
        {
            transform.position -= firingDirection * speed * Time.deltaTime;

            if (Vector3.Distance(transform.position, player.transform.position) <= 0.1f)
            {
                isReturning = false;
                elapsedTime = 0.0f;
            }
        }
    }

    public void LevelUp(float damage, int count, int projectileCount)
    {
        this.damage = (int)damage;
        this.count += count;
        this.projectileCount = projectileCount;

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    public void Init(ItemData data, int projectileCount)
    {
        name = "Boomerang" + data.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = data.itemId;
        damage = (int)data.baseDamage;
        count = data.baseCount;

        for (int index = 0; index < GameManager.instance.pool.prefabs.Length; index++)
        {
            if (data.projectile == GameManager.instance.pool.prefabs[index])
            {
                prefabId = index;
                break;
            }

        }
        switch (id)
        {
            default:
                speed = 5f;
                break;
        }

        player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Fire()
    {
        for (int i = 0; i < projectileCount; i++)
        {
            float randomAngle = Random.Range(0, 360);
            Vector3 dir = new Vector3(Mathf.Cos(randomAngle * Mathf.Deg2Rad), Mathf.Sin(randomAngle * Mathf.Deg2Rad), 0).normalized;
            firingDirection = dir;

            Transform ReturnBoomerang = GameManager.instance.pool.Get(prefabId).transform;
            if (ReturnBoomerang == null)
            {
                return;
            }

            ReturnBoomerang.position = transform.position;
            ReturnBoomerang.rotation = Quaternion.Euler(0, 0, randomAngle);
            ReturnBoomerang.GetComponent<ReturnBoomerang>().Init(damage, count, dir);
        }
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource != null && audioSource.clip != null)
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    void StartReturning()
    {
        isReturning = true;
        elapsedTime = 0.0f;
    }

    void ApplyDamage(Collider2D[] colliders)
    {
        for (int i = 0; i < colliders.Length; i++)
        {
            Enemy enemy = colliders[i].gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
